import os
from dataclasses import dataclass
from django.core.management.base import BaseCommand
from scheduling.models import Auftrag,SentEmailSettings,SentEmails,CustomerTemplate, Benutzer, Firma, Adresse, AuftragsartBlacklist
from django.core.mail.backends.smtp import EmailBackend
from django.utils.html import strip_tags
import datetime
import pytz
from django.core.mail import EmailMultiAlternatives
from ics import Calendar, Event
from time import sleep
import re
from enum import Enum
import dns.resolver
import dns.exception

@dataclass
class EmailDetailsOrder:
    mitarbeiterID:int
    auftragsID:int
    auftraggeberID:int
    auftraggeber:str
    storno:int
    storno_grund:str
    fertiggestellt_am:str
    termin_tag:str
    termin_datum:str
    termin_uhrzeit:str
    termin_uhrzeit_bis:str
    grund_des_termins:str
    objekt_bezeichnung:str
    objekt_bezeichnung_1:str
    email:str
    # newsletterLink:str

@dataclass
class EmailDetailsAddress:
    empfangseMail:str
    besichtigungsname:str
    besichtigungsadresse:str


@dataclass
class EmailDetailsUser:
    mitarbeiterID:int
    emailMitarbeiter:str
    vornameMitarbeiter:str
    nachnameMitarbeiter:str
    telefonMitarbeiter:str
    mobilMitarbeiter:str


@dataclass
class Email:
    recipient:str
    sender:str
    subject:str
    text:str
    html_text:str
    attachment:str

@dataclass
class EmailSettings:
    smtp_server:str
    smtp_port:str
    smtp_user:str
    smtp_password:str
    smtp_sender:str
    use_ssl:bool
    use_tls:bool
    calendar_subject:str
    calendar_description:str
    cc:str
    test_mailing:bool
    mail_error_receiver:str
    mail_errors:bool


class Terminart(Enum):
    Confirmation = 1
    Reminder = 2


TERMIN_SHORT = {
    Terminart.Confirmation: 'confirmation',
    Terminart.Reminder: 'reminder',
}


TERMIN_LONG = {
    Terminart.Confirmation: 'Terminbestätigung',
    Terminart.Reminder: 'Terminerinnerung',
}


@dataclass
class LogEntry:
    order: int
    message: str
    level: str


class SkipOrderError(Exception):
    pass


class TemplateMissingError(SkipOrderError):
    pass


class PastOrderError(SkipOrderError):
    pass


class BlacklistedOrderError(SkipOrderError):
    pass


class AddressEmptyError(SkipOrderError):
    pass


class MailingError(Exception):
    pass


class AddressParseError(MailingError):
    pass


class MailSendingError(MailingError):
    pass


class Command(BaseCommand):
    help = "Sends emails based on SentEmailSettings for Table Auftrag"
    template_tag = "%"
    tz = pytz.timezone('Europe/Berlin')
    logfile_dir = ""
    smtp_settings = None
    mail_template = ''
    # currently not implemented
    # newsletter_link = ""
    # newsletter_link_text = ""

    @staticmethod
    def get_template_dict(dataclass_object):
        return {x: getattr(dataclass_object, x) for x in dir(dataclass_object) if x[:2] != "__"}

    def send_mail(self,mail):
        try:
            if self.smtp_settings.use_ssl and self.smtp_settings.use_tls:
                # Default to ssl if both are active
                self.use_tls = False
            connection = EmailBackend(host=self.smtp_settings.smtp_server, port=self.smtp_settings.smtp_port, username=self.smtp_settings.smtp_user,
                                      password=self.smtp_settings.smtp_password,use_tls=self.smtp_settings.use_tls,use_ssl=self.smtp_settings.use_ssl)
            msg = EmailMultiAlternatives(subject=mail.subject, body=strip_tags(mail.text), from_email=mail.sender, connection=connection)

            if len(self.smtp_settings.cc.strip()) > 0:
                msg.cc = [self.smtp_settings.cc.strip()]
            if self.smtp_settings.test_mailing is False:
                msg.to = mail.recipient if isinstance(mail.recipient, list) else [mail.recipient]

            if len(mail.attachment) > 0:
                msg.attach(filename='Termin.ics', content=mail.attachment, mimetype='text/calendar')
            if len(mail.html_text) > 0:
                msg.attach_alternative(content=mail.html_text, mimetype='text/html')
            msg.send(fail_silently=False)
            sleep(5)
        except Exception as e:
            return str(e)
        return True

    @staticmethod
    def check_mx_record(mail_address):
        provider = mail_address.split('@')[1]
        try:
            records = dns.resolver.resolve(provider, 'MX')
            assert records[0].exchange is not None
            return True
        except (AssertionError, dns.exception.DNSException) as e:
            raise AddressParseError(f'Der Provider "{provider}" ist ungültig, da kein MX Record hinterlegt ist: {str(e)}')

    def get_sanitized_mail_recipient_list(self, candidates):
        splitted = candidates.replace(' ', '').split(';')
        for addr in splitted:
            if not self.is_mail_valid(addr):
                raise AddressParseError(f"Es wurde keine gültige E-Mail-Adresse für den Empfänger angegeben. Die fehlerhafte Adresse lautet: {addr}")
            self.check_mx_record(addr)
        return splitted

    def log_entry(self, entry: LogEntry):
        today = datetime.datetime.now(tz=self.tz).strftime("%Y-%d-%m")
        _time = datetime.datetime.now(tz=self.tz).strftime("%H:%M:%S")
        filename = os.path.join(self.logfile_dir, f"{today}.txt")
        try:
            if not os.path.exists(filename):
                with open(filename, "w") as fd:
                    fd.write('Timestamp | Order-ID | Log Level | Message\n')

            with open(filename, "a") as fd:
                line = f"{_time} | {entry.order} | {entry.level} | {'TESTING: ' if self.smtp_settings.test_mailing is True else ''}{entry.message}\n"
                fd.write(line)
        except:
            print(f"Could not write to log file {filename}")

    def get_template_var_map(self, order):
        """
        Retrieves a template variable map for an order {template_variable_name:value}
        """
        translation_map = {
            'Monday': 'Montag',
            'Tuesday': 'Dienstag',
            'Wednesday': 'Mittwoch',
            'Thursday': 'Donnerstag',
            'Friday': 'Freitag',
            'Saturday': 'Samstag',
            'Sunday': 'Sonntag'
        }
        cleared_termin_date = order.termin_datum.strftime("%d.%m.%Y")
        termin_tag = order.termin_datum.strftime("%A")
        if termin_tag in translation_map:
            termin_tag = translation_map[termin_tag]

        try:
            adresse = Adresse.objects.filter(auftrag_id=order.id, rolle=1)[0]
            adresse_details = EmailDetailsAddress(empfangseMail=adresse.email,
                                                  besichtigungsname=f"{adresse.firma} {adresse.name2} {adresse.name}",
                                                  besichtigungsadresse=f"{adresse.strasse_haus_nr}, {adresse.plz} {adresse.ort}")
        except Exception as e:
            raise AddressParseError(f"Es konnte keine Adresse aus der Datenbank ausgelesen werden. Existiert ein Eintrag für diese Auftrag-ID mit der Rolle 1?")

        if adresse_details.empfangseMail is None or not len(adresse_details.empfangseMail.strip()):
            raise AddressEmptyError(f"Es wurde keine E-Mail-Adresse für den Empfänger angegeben.")

        adresse_details.empfangseMail = self.get_sanitized_mail_recipient_list(adresse_details.empfangseMail)
        if len(order.termin_uhrzeit_bis.strip())==0 or order.termin_uhrzeit_bis:
            termin_uhrzeit_bis=None
        else:
            termin_uhrzeit_bis = self.build_appointment_end_string(order.termin_datum, order.termin_uhrzeit, order.termin_uhrzeit_bis)

        if termin_uhrzeit_bis is None:
            termin_uhrzeit_bis = f"um {order.termin_uhrzeit}"
        else:
            termin_uhrzeit_bis = f"von {order.termin_uhrzeit} - {termin_uhrzeit_bis}"

        order_details = EmailDetailsOrder(mitarbeiterID=order.sv_id, auftragsID=order.id, auftraggeberID=order.auftraggeber_id, auftraggeber=order.auftraggeber,
                                          storno=order.storno, storno_grund=order.storno_grund, fertiggestellt_am=order.fertiggestellt_am, termin_datum=cleared_termin_date,
                                          termin_uhrzeit=order.termin_uhrzeit, termin_uhrzeit_bis=termin_uhrzeit_bis, grund_des_termins=order.auftragsart,
                                          objekt_bezeichnung=order.fabrikat, objekt_bezeichnung_1=order.kennzeichen, email=adresse_details.empfangseMail, termin_tag=termin_tag)
                                          # newsletterLink="")

        try:
            user = Benutzer.objects.get(id=order_details.mitarbeiterID)
            user_details=EmailDetailsUser(mitarbeiterID=user.id, emailMitarbeiter=user.email, vornameMitarbeiter=user.vorname,
                                          nachnameMitarbeiter=user.nachname, telefonMitarbeiter=user.telefon, mobilMitarbeiter="")
        except Exception as e:
            user_details=EmailDetailsUser(mitarbeiterID=0, emailMitarbeiter="", vornameMitarbeiter="",
                                          nachnameMitarbeiter="", telefonMitarbeiter="", mobilMitarbeiter="")
            self.log_entry(LogEntry(order=order_details.mitarbeiterID, level='WARNING', message=f'Mitarbeiter ID {order_details.mitarbeiterID} for order ID {order_details.auftragsID} was not found - Mitarbeiter variables might not work: {str(e)}'))

        # Currently not yet implemented
        # if len(self.newsletter_link) > 0:
        #     if len(self.newsletter_link_text) == 0:
        #         self.newsletter_link_text = "Newsletter Anmeldung"
        #     order_details.newsletterLink = f"<a href='{self.newsletter_link}?auftrag={order_details.auftragsID}&email={order_details.email}'>{self.newsletter_link_text}</a>"
        template_map_adresse = self.get_template_dict(adresse_details)
        template_map_user = self.get_template_dict(user_details)
        template_map = self.get_template_dict(order_details)
        template_map.update(template_map_user)
        template_map.update(template_map_adresse)

        return template_map

    def build_appointment_end_string(self, start_date, start_time_string, end_time_string=''):
        start_datetime = self.build_proper_appointment_start_datetime(start_date, start_time_string)
        end_datetime = self.build_appointment_end_datetime(start_datetime, end_time_string)
        return end_datetime.strftime('%H:%M')

    def build_appointment_end_datetime(self, start_datetime, end_string):
        default_candidate = datetime.timedelta(hours=1) + start_datetime
        end_candidate = self.build_proper_appointment_start_datetime(start_datetime, end_string)
        if end_candidate > default_candidate:
            return end_candidate
        return default_candidate

    def generate_ics_file(self, date, begin, end=None):
        """
        Generates an .ics file to add to the mail
        """
        c = Calendar()
        e = Event(name=self.smtp_settings.calendar_subject)

        if len(self.smtp_settings.calendar_description):
            e.description = self.smtp_settings.calendar_description

        start_date = self.build_proper_appointment_start_datetime(date, begin)
        end_date = self.build_appointment_end_datetime(start_date, end)

        e.begin = (start_date, "Europe/Berlin")
        e.end = (end_date, "Europe/Berlin")

        c.events.add(e)
        return str(c)

    def substitute_template(self, template, template_var_map):
        """
        Substitutes a template with the template_var_map
        """
        for tpl_identifier in template_var_map:
            template = template.replace(f"{self.template_tag}{tpl_identifier}{self.template_tag}",str(template_var_map[tpl_identifier]))
        return template

    def generate_email(self, order, appointment_data, content, subject):
        template_map = self.get_template_var_map(order)
        substituted_template = self.substitute_template(template=appointment_data, template_var_map=template_map)
        html_mailtext = self.substitute_template(self.mail_template, template_var_map={
            'appointment_data': f"<pre>{substituted_template}</pre>",
            'content': content
        })
        attachment = self.generate_ics_file(date=order.termin_datum, begin=order.termin_uhrzeit, end=order.termin_uhrzeit_bis)
        mail = Email(recipient=template_map['email'], subject=subject, text=substituted_template,html_text=html_mailtext,sender=self.smtp_settings.smtp_sender, attachment=attachment)
        return mail

    def send_email_confirmation(self, mail,order_id):
        state = self.send_mail(mail)
        if state is True and self.smtp_settings.test_mailing is False:
            auftrag = Auftrag.objects.get(id=order_id)
            sent_confirmation = SentEmails.objects.create(auftrag_id=order_id,
                                                          sent_confirmation_datetime=datetime.datetime.now(tz=self.tz).replace(tzinfo=pytz.utc),
                                                          auftrag_date=self.build_proper_appointment_start_datetime(auftrag.termin_datum, auftrag.termin_uhrzeit))
            sent_confirmation.save()
            return True
        return state

    def send_email_reminder(self, mail,order_id):
        state = self.send_mail(mail)
        if state is True and self.smtp_settings.test_mailing is False:
            sent_confirmation = SentEmails.objects.get(auftrag_id=order_id)
            sent_confirmation.sent_reminder_datetime = datetime.datetime.now(tz=self.tz).replace(tzinfo=pytz.utc)
            sent_confirmation.save()
            return True
        return state

    def get_rescheduled_orders(self, sent_confirmations):
        to_send = []
        for sent in sent_confirmations.iterator():
            if sent.auftrag_date is None:
                continue
            order = Auftrag.objects.get(id=sent.auftrag_id)
            auftrag_datetime = self.build_proper_appointment_start_datetime(order.termin_datum, order.termin_uhrzeit)
            if auftrag_datetime != sent.auftrag_date:
                to_send.append(order.id)
        SentEmails.objects.filter(auftrag_id__in=to_send).delete()
        return set(to_send)

    def get_order_confirmations(self, orders):
        due_order_ids = [x.id for x in orders]
        confirmation_send = SentEmails.objects.filter(auftrag_id__in=due_order_ids)
        rescheduled_order_ids = self.get_rescheduled_orders(confirmation_send)
        confirmation_to_send = set(due_order_ids).difference([x.auftrag_id for x in confirmation_send]).union(rescheduled_order_ids)
        return confirmation_to_send

    @staticmethod
    def is_auftragsart_blacklisted(auftraggeber_id, auftragsart):
        if auftraggeber_id == 46 and auftragsart == 'Minderwertgutachten':
            return True
        try:
            entry = AuftragsartBlacklist.objects.get(auftragsart=auftragsart)
            return entry.blacklisted
        except AuftragsartBlacklist.DoesNotExist:
            return False

    @staticmethod
    def is_mail_valid(mail_address):
        regex = r'\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b'
        return re.fullmatch(regex, mail_address) is not None

    @staticmethod
    def build_proper_appointment_start_datetime(start_datetime,start_time_string):
        try:
            start_time = datetime.datetime.strptime(start_time_string[:5], "%H:%M")
        except:
            # Use start_datetime from termin_datum if there is no start_time present
            return start_datetime
        start_day = start_datetime.replace(hour=0,minute=0,second=0)
        start_time_delta = datetime.timedelta(hours=start_time.hour,minutes=start_time.minute,seconds=start_time.second)
        return start_day + start_time_delta

    def get_order_reminders(self,orders,due_reminder_datetime):
        order_reminder_mapping = [(order.id,self.build_proper_appointment_start_datetime(order.termin_datum,order.termin_uhrzeit)) for order in orders]
        due_order_ids = [x[0] for x in order_reminder_mapping if x[1]<=due_reminder_datetime]
        reminders_not_done = SentEmails.objects.filter(auftrag_id__in=due_order_ids).filter(
            sent_reminder_datetime__isnull=True).filter(
            sent_confirmation_datetime__lt=datetime.datetime.now().replace(hour=0, minute=0, second=0, microsecond=0).replace(tzinfo=pytz.utc))
        reminder_to_send = set(due_order_ids).intersection([x.auftrag_id for x in reminders_not_done])
        return reminder_to_send

    def notify_error_recipient(self, order_id, error):
        recipient = self.smtp_settings.mail_error_receiver
        subject = f'Terminerinnerung Fehler - Auftrag {order_id}'
        content = f"Für den Auftrag {order_id} konnte keine Benachrichtigung versendet werden. Ist die E-Mail-Adresse korrekt?\n\nFehlernachricht: {error}"
        message = Email(recipient=recipient, sender=self.smtp_settings.smtp_sender, subject=subject, text=content, html_text='', attachment='')
        mail_sent = self.send_mail(message)
        if mail_sent is True:
            self.log_entry(LogEntry(order=order_id, level='INFO', message=f'Notified {recipient} about mailing error: {error}'))
        else:
            self.log_entry(LogEntry(order=order_id, level='ERROR', message=f'While trying to notify {recipient} about mailing error "{error}" another error occurred: {mail_sent}'))

    def process_order(self, order, company_template_map, terminart):
        try:
            try:
                email_template = company_template_map[order.auftraggeber_id]
            except KeyError:
                raise TemplateMissingError(f'No {TERMIN_SHORT[terminart]} template')
            if datetime.datetime.now(tz=self.tz).replace(tzinfo=pytz.utc) > self.build_proper_appointment_start_datetime(order.termin_datum, order.termin_uhrzeit):
                raise PastOrderError('Appointment date is in the past')
            if self.is_auftragsart_blacklisted(auftraggeber_id=order.auftraggeber_id, auftragsart=order.auftragsart):
                raise BlacklistedOrderError(f'Auftragsart {order.auftragsart} for Auftraggeber {order.auftraggeber_id} is blacklisted')

            mail = self.generate_email(order, email_template[TERMIN_SHORT[terminart]], email_template['content'], subject=f"Ihre {TERMIN_LONG[terminart]}")
            if terminart == Terminart.Confirmation:
                mail_sent = self.send_email_confirmation(mail, order_id=order.id)
            else:
                mail_sent = self.send_email_reminder(mail, order_id=order.id)

            if mail_sent is not True:
                raise MailSendingError(mail_sent)

            self.log_entry(LogEntry(order=order.id, level='INFO', message=f'Sent appointment {TERMIN_SHORT[terminart]} to {mail.recipient}'))
        except SkipOrderError as e:
            self.log_entry(LogEntry(order=order.id, level='INFO', message=f'Skipping: {str(e)}'))
        except MailingError as e:
            if self.smtp_settings.mail_errors is True:
                self.notify_error_recipient(order.id, str(e))
            self.log_entry(LogEntry(order=order.id, level='ERROR', message=f'Could not send {TERMIN_SHORT[terminart]}: {str(e)}'))
        except Exception as e:
            self.log_entry(LogEntry(order=order.id, level='CRITICAL', message=f'Unknown error processing {TERMIN_SHORT[terminart]}: {str(e)}'))

    def process_order_confirmations(self, orders, company_template_map):
        confirmations_to_send = self.get_order_confirmations(orders)
        for order in orders.filter(id__in=confirmations_to_send):
            self.process_order(order, company_template_map, Terminart.Confirmation)

    def process_order_reminders(self, orders, company_template_map, due_reminder_datetime):
        reminders_to_send = self.get_order_reminders(orders,due_reminder_datetime)
        for order in orders.filter(id__in=reminders_to_send):
            self.process_order(order, company_template_map, Terminart.Reminder)

    def handle(self, *args, **options):
        try:
            setting_object = SentEmailSettings.objects.filter(remind_interval_hours__isnull=False)[0]
        except Exception as e:
            print(f"No usable entry in SentEmailSettings-Table {str(e)}")
            exit(1)
        self.logfile_dir = setting_object.logfile_dir
        # self.newsletter_link = setting_object.newsletter_link
        # self.newsletter_link_text = setting_object.newsletter_link_text
        self.mail_template = setting_object.mail_template
        self.smtp_settings = EmailSettings(smtp_server=setting_object.smtp_server,smtp_port=setting_object.smtp_port,
                                           smtp_user=setting_object.smtp_user,smtp_password=setting_object.smtp_password,use_tls=setting_object.use_tls,
                                           use_ssl=setting_object.use_ssl, calendar_subject=setting_object.calendar_subject, calendar_description=setting_object.calendar_description,
                                           test_mailing=setting_object.test_mailing, cc=setting_object.cc_target, smtp_sender=setting_object.smtp_sender,
                                           mail_error_receiver=setting_object.mail_error_receiver, mail_errors=setting_object.mail_errors)

        reminder_interval = setting_object.remind_interval_hours
        # Replace timezone information to utc without conversion (gets +00 from database)
        due_reminder_datetime = datetime.datetime.now(tz=self.tz) + datetime.timedelta(hours=reminder_interval)
        due_reminder_datetime = due_reminder_datetime.replace(tzinfo=pytz.utc)
        # We can handle tz info here "normally" cause we filter for appointments from yesterday on...
        # doesnt matter if the mail is sent at 00:00 or at 02:00
        filter_date = datetime.datetime.now(tz=self.tz).replace(hour=0, minute=0, second=0, microsecond=0)
        orders = Auftrag.objects.filter(termin_datum__gte=filter_date)
        company_template_map = {t.firma:{"confirmation":t.confirmation,"reminder":t.reminder, "content": t.content} for t in CustomerTemplate.objects.all()}
        self.process_order_confirmations(orders,company_template_map)
        self.process_order_reminders(orders,company_template_map,due_reminder_datetime)