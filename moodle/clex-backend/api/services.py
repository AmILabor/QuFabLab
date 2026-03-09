import datetime
import os
import urllib

from django.contrib.auth.models import Group
from django.core.mail import EmailMessage
from django.core.mail.backends.smtp import EmailBackend
from django.db.models import Q, QuerySet
from django.template import Template, Context
from django.utils import timezone
from rest_framework.exceptions import APIException, ParseError, ValidationError

from api.models.app_settings import SettingsModel
from api.models.salesareas import SalesAreasPerUser
from api.selectors import get_customer_activity_sla, get_customer_slas
from clexbackend import settings
from utils.Exceptions import MailSendException
from utils.OrderHistoryActions import OrderHistoryActionEnum


def generate_gmaps_link(stops, expert_employee):
    url = "https://maps.google.com/maps/dir/"
    address_url_part = f"{expert_employee.street} {expert_employee.zip} {expert_employee.city}/"
    for stop in stops:
        inspection_address = stop.inspection_address()
        address_url_part += f"{inspection_address.street_address} {inspection_address.zip} {inspection_address.city}/"
    url = url + urllib.parse.quote(address_url_part)
    return url

def setup_backend(settings):
    try:
        backend = EmailBackend(host=settings.email_server, port=settings.email_server_port,
                               username=settings.email_user,
                               password=settings.email_password, use_tls=True, fail_silently=False)
    except Exception as e:
        raise MailSendException("Das Email-Backend ist nicht erreichbar. Einstellungen prüfen.")
    return backend

def send_mail(subject, mail_body, sender_mail,receiver):
    settings = SettingsModel.objects.get(pk=1)

    backend = setup_backend(settings)
    if settings.email_debug == True:
        receiver = [settings.email_debug_receiver]
    msg = EmailMessage(subject=subject, body=mail_body,
                       from_email=sender_mail,
                       to=receiver,
                       connection=backend)
    try:
        msg.send()
    except Exception as e:
        print(e)
        raise MailSendException("Die Email konnte nicht versendet werden.")


def send_mass_mail(subjects,mail_bodys,sender_mail,receivers):
    assert len(subjects) == len(mail_bodys) == len(receivers)
    settings = SettingsModel.objects.get(pk=1)
    backend = setup_backend(settings)
    messages = []
    for i,subject in enumerate(subjects):
        mail_body = mail_bodys[i]
        receiver =receivers[i]
        if settings.email_debug == True:
            receiver = [settings.email_debug_receiver]
        messages.append(EmailMessage(subject=subject, body=mail_body,
                           from_email=sender_mail,
                           to=receiver,
                           connection=backend))
    try:
        if len(messages)>0:
            backend.send_messages(messages)
    except Exception as e:
        print(e)
        raise MailSendException("Die Emails konnten nicht versendet werden.")


def send_tour_email(tour, sender_mail):
    mail_template = """
                    Hallo Hier ist deine Tour {{tour.expert_employee.firstname}} {{tour.expert_employee.lastname}}
                    {% for stop in stops %}
                        {{stop.order_type}}({{stop.id}}) {{stop.appointment_date|date:'d.m.Y'}} {{stop.appointment_time_start}} - {{stop.appointment_time_end|default:"??"}}
                            {{stop.inspection_address.street_address}} {{stop.inspection_address.zip}} {{stop.inspection_address.city}}
                    {% endfor %}

                    {{gmaps_link}}
                    """
    subject = f'[ClaimsExperts] Tour für den {tour.tour_date.strftime("%d.%m.%Y")}'
    tpl = Template(mail_template)
    stops = tour.tour_stops.all().order_by("order__appointment_time_start")

    context = Context(
        {"tour": tour, "stops": stops, "gmaps_link": generate_gmaps_link(stops, tour.expert_employee)})
    mail_body = tpl.render(context)
    send_mail(subject=subject, mail_body=mail_body, sender_mail=sender_mail,receiver=tour.expert_employee.email)


def send_tour_changed_email(tour,sender_mail):
    mail_template = """
                    Hallo {{tour.expert_employee.firstname}} {{tour.expert_employee.lastname}} deine Tour hat sich geändert.
                    {% for stop in stops %}
                        {{stop.order_type}}({{stop.id}}) {{stop.appointment_date|date:'d.m.Y'}} {{stop.appointment_time_start}} - {{stop.appointment_time_end|default:"??"}}
                            {{stop.inspection_address.street_address}} {{stop.inspection_address.zip}} {{stop.inspection_address.city}}
                    {% endfor %}

                    {{gmaps_link}}
                    """
    subject = f'[ClaimsExperts] Tour für den {tour.tour_date.strftime("%d.%m.%Y")}'
    tpl = Template(mail_template)
    stops = tour.tour_stops.all().order_by("order__appointment_time_start")

    context = Context(
        {"tour": tour, "stops": stops, "gmaps_link": generate_gmaps_link(stops, tour.expert_employee)})
    mail_body = tpl.render(context)
    send_mail(subject=subject, mail_body=mail_body, sender_mail=sender_mail,receiver=tour.expert_employee.email)

def send_tour_swap_email(tours, sender_mail):
    mail_template = """
                    Hallo {{tour.expert_employee.firstname}} {{tour.expert_employee.lastname}} deine Tour wurde getauscht!
                    {% for stop in stops %}
                        {{stop.order_type}}({{stop.id}}) {{stop.appointment_date|date:'d.m.Y'}} {{stop.appointment_time_start}} - {{stop.appointment_time_end|default:"??"}}
                            {{stop.inspection_address.street_address}} {{stop.inspection_address.zip}} {{stop.inspection_address.city}}
                    {% endfor %}

                    {{gmaps_link}}
                    """
    mail_bodys = []
    subjects = []
    receivers = []
    for tour in tours:
        subject = f'[ClaimsExperts] Tour für den {tour.tour_date.strftime("%d.%m.%Y")}'
        tpl = Template(mail_template)
        stops = tour.tour_stops.all().order_by("order__appointment_time_start")

        context = Context(
            {"tour": tour, "stops": stops, "gmaps_link": generate_gmaps_link(stops, tour.expert_employee)})
        mail_body = tpl.render(context)
        mail_bodys.append(mail_body)
        subjects.append(subject)
        receivers.append(tour.expert_employee.email)
    send_mass_mail(subjects=subjects,mail_bodys=mail_bodys,receivers=receivers,sender_mail=sender_mail)



def send_revocation_mail(tour, sender_mail):
    mail_template = """
                    Hallo  {{tour.expert_employee.firstname}} {{tour.expert_employee.lastname}} deine Tour für den {{tour.tour_date}} wurde abgesagt!
                    """
    subject = f'[ClaimsExperts] Tour für den {tour.tour_date.strftime("%d.%m.%Y")}'
    tpl = Template(mail_template)
    stops = tour.tour_stops.all().order_by("order__appointment_time_start")

    context = Context(
        {"tour": tour, "stops": stops, "gmaps_link": generate_gmaps_link(stops, tour.expert_employee)})
    mail_body = tpl.render(context)
    send_mail(subject=subject, mail_body=mail_body, sender_mail=sender_mail,receiver=tour.expert_employee.email)


def generate_locked_error_message(locked_object):
    locked_by = locked_object.user
    locked_by_id = locked_by.id
    locked_since = locked_object.timestamp
    locked_by_name = f"{locked_by.firstname} {locked_by.lastname}"
    msg = {'lockedByName': locked_by_name, 'lockedById': locked_by_id, 'since': locked_since}
    exc = ParseError(detail=msg)
    raise exc
    #return {'detail': msg}

def generate_file_path(file_name, file_extension):
    original_path = os.path.join(settings.ORDER_FILE_WRITE_PATH, f"{file_name}.{file_extension}")
    if not os.path.exists(original_path):
        file_name = "test"
        if file_extension in ["png", "jpg", "tif"]:
            file_extension = "png"
        elif file_extension in ["htm", "html"]:
            file_extension = "html"
        elif file_extension in ["txt", "csv"]:
            file_extension = "txt"
        elif file_extension.lower() in ['xml']:
            file_extension = "xml"
    file_path = os.path.join(settings.ORDER_FILE_PATH, f"{file_name}.{file_extension}")
    return file_path

def generate_file_representation(order_id,file_name):
    name = file_name.rsplit(".")[0]
    extension = file_name.rsplit(".", 1)[1]
    if name[:len(str(order_id))] != str(order_id):
        name = str(order_id) + "_" + name

    file_path = os.path.join(settings.ORDER_FILE_WRITE_PATH, f"{name}.{extension}")
    error_idx=0
    tmp_name = name
    while os.path.exists(file_path):
        tmp_name = f"{name}_{error_idx}"
        file_path = os.path.join(settings.ORDER_FILE_WRITE_PATH, f"{tmp_name}.{extension}")
        error_idx+=1

    return {"typ":0,"name":tmp_name,"extension":extension,'order_id':order_id}

def write_out_file(file_name,file_extension,file):
    #TODO: Security Considerations
    file_path = os.path.join(settings.ORDER_FILE_WRITE_PATH, f"{file_name}.{file_extension}")

    try:
        with open(file_path,'wb+') as df:
            for chunk in file.chunks():
                df.write(chunk)
    except Exception as e:
        print(e)
        return False
    return True

def preprocess_order_foreign_keys(order_object):
    if order_object.expert_employee_id == 0:
        order_object.expert_employee_id = None
    if order_object.expert_backoffice_id == 0:
        order_object.expert_backoffice_id = None
    if order_object.responsible_user_id ==0:
        order_object.responsible_user_id = None
    return order_object

def set_order_edit_date(object):
    object.edit_date = timezone.now()
    object.save()

def handle_order_history(order_object, order_object_new_values, user):
    if order_object_new_values is None:
        # Auftrag angelegt
        pass
    else:
        set_order_history(order_object=order_object, user=user,
                          order_history_action=OrderHistoryActionEnum.ORDER_SAVED)
    if order_object_new_values.get("followup_date",None) is not None and order_object.followup_date != order_object_new_values["followup_date"]:
        set_order_history(order_object=order_object,user=user,referenced_object=order_object_new_values,order_history_action=OrderHistoryActionEnum.FOLLOWUP_SET)
    if order_object_new_values.get("cancellation",None) is not None and order_object.cancellation != order_object_new_values["cancellation"] and order_object_new_values["cancellation"] == 1:
        set_order_history(order_object=order_object, user=user,
                          order_history_action=OrderHistoryActionEnum.ORDER_CANCELED)
    if order_object_new_values.get("cancellation",None) is not None and order_object.cancellation != order_object_new_values["cancellation"] and order_object_new_values["cancellation"] == 0:
        set_order_history(order_object=order_object, user=user,
                          order_history_action=OrderHistoryActionEnum.ORDER_CANCELATION_REVOKED)
    if order_object_new_values.get("status", None) is not None and order_object.status !=order_object_new_values["status"] and order_object_new_values["status"] == 1:
        set_order_history(order_object=order_object, user=user,
                          order_history_action=OrderHistoryActionEnum.ORDER_DONE_REVOKED)
    if order_object_new_values.get("status", None) is not None and order_object.status !=order_object_new_values["status"] and order_object_new_values["status"] == 2:
        set_order_history(order_object=order_object, user=user,
                          order_history_action=OrderHistoryActionEnum.DONE)
    if order_object_new_values.get("responsible_user", None) is not None and order_object.responsible_user !=order_object_new_values["responsible_user"]:
        set_order_history(order_object=order_object, user=user,
                          order_history_action=OrderHistoryActionEnum.RESPONSIBLE_EMPLOYEE_SET)
    if order_object_new_values.get("responsible_area", None) is not None and order_object.responsible_area !=order_object_new_values["responsible_area"]:
        set_order_history(order_object=order_object, user=user,
                          order_history_action=OrderHistoryActionEnum.AREA_ASSIGNED)
    if order_object_new_values.get("order_type", None) is not None and order_object.order_type in ["",0,None] and order_object.order_type !=order_object_new_values["order_type"]:
        set_order_history(order_object=order_object, user=user, referenced_object=order_object_new_values,
                          order_history_action=OrderHistoryActionEnum.ORDER_TYPE_SET)
    elif order_object_new_values.get("order_type", None) is not None and order_object.order_type !=order_object_new_values["order_type"]:
        set_order_history(order_object=order_object, user=user, referenced_object=order_object,
                          order_history_action=OrderHistoryActionEnum.ORDER_TYPE_CHANGED)

    print("")

def set_order_history(order_object,user,order_history_action,referenced_object = None):
    current_history = order_object.history
    new_history_string = ""
    user_string = f"{user.firstname} {user.lastname}"
    log_time = timezone.now().strftime("%d.%m.%Y %H:%M:%S")

    if order_history_action == OrderHistoryActionEnum.ORDER_VIEWED:
        new_history_string = f" Daten gesichtet."
    elif order_history_action == OrderHistoryActionEnum.ORDER_SAVED:
        new_history_string = f" Daten gespeichert."
    elif order_history_action == OrderHistoryActionEnum.ATTACHMENT_ADDED:
        new_history_string = f" Anlage {referenced_object['name']}.{referenced_object['extension']} hinzugefügt"
    elif order_history_action == OrderHistoryActionEnum.ORDER_DONE:
        new_history_string = f" Auftrag fertig gestellt."
    elif order_history_action == OrderHistoryActionEnum.ORDER_DONE_REVOKED:
        new_history_string = f" Fertigstellung aufgehoben."
    elif order_history_action == OrderHistoryActionEnum.ORDER_REWRITTEN:
        new_history_string = f" Auftrag umgeschrieben. Umschreibung Nr. {referenced_object.id}"
    elif order_history_action == OrderHistoryActionEnum.ORDER_CREATED_BY_REWRITE:
        new_history_string = f" Auftrag angelegt aus Umschreibung von {order_object.id}"
    elif order_history_action == OrderHistoryActionEnum.ORDER_CREATED:
        new_history_string = f" Auftrag angelegt."
    elif order_history_action == OrderHistoryActionEnum.ORDER_CREATED_BY_COPY:
        new_history_string = f" Auftrag angelegt als Kopie von Auftrag Nr. {referenced_object.id}"
    elif order_history_action == OrderHistoryActionEnum.ORDER_CANCELATION_REVOKED:
        new_history_string = f" Stornierung aufgehoben."
    elif order_history_action == OrderHistoryActionEnum.ORDER_CANCELED:
        new_history_string = f" Auftrag storniert."
    elif order_history_action == OrderHistoryActionEnum.ATTACHMENTS_DOWNLOADED:
        new_history_string = f" Auftragsanhänge heruntergeladen."
    elif order_history_action == OrderHistoryActionEnum.FOLLOWUP_SET:
        new_history_string = f" Wiedervorlage gesetzt - {referenced_object['followup_date'].strftime('%d.%m.%Y')}"
    elif order_history_action == OrderHistoryActionEnum.RESPONSIBLE_EMPLOYEE_SET:
        new_history_string = f" Bearbeiter {referenced_object.firstname}, {referenced_object.lastname} zugewiesen."
    elif order_history_action == OrderHistoryActionEnum.AREA_ASSIGNED:
        new_history_string = f" Bereich ({referenced_object.name}) zugewiesen."
    elif order_history_action == OrderHistoryActionEnum.ORDER_TYPE_SET:
        new_history_string = f" Auftragsart gesetzt: {referenced_object['order_type']}."
    elif order_history_action == OrderHistoryActionEnum.ORDER_TYPE_CHANGED:
        new_history_string = f" Auftragsart geändert von {order_object.order_type} nach {referenced_object['order_type']}."
    elif order_history_action == OrderHistoryActionEnum.TOUR_MAIL_SENT:
        new_history_string = f" Der Auftrag wurde per Email an {referenced_object.expert_employee.firstname} {referenced_object.expert_employee.lastname}({referenced_object.expert_employee.email}) versendet."
    elif order_history_action == OrderHistoryActionEnum.TOUR_MAIL_REVOKED:
        new_history_string = f" Die Tour dieses Auftrags wurde zurückgezogen und per Email an {referenced_object.expert_employee.firstname} {referenced_object.expert_employee.lastname}({referenced_object.expert_employee.email}) gemeldet."
    if current_history is None:
        current_history = ""
    current_history = f"{log_time}: {new_history_string} ({user_string})\n"+current_history
    order_object.history = current_history
    order_object.save()

def create_sla_entry(object , activity_id: int, user_id: int, serializer_class ):
    if len(object.sla_report.filter(activity_id=activity_id)) == 0:
        sla = get_customer_activity_sla(object.customer_id, object.order_type, activity_id=activity_id)
        if sla is None:
            print("There is no SLA here")
            return
        data = {"order_id": object.id, "activity_id": activity_id, "user_id": user_id,
                "order_creation_date": object.creation_date, "sla_limit": sla.sla_time}
        serializer = serializer_class(data=data)
        if serializer.is_valid():
            serializer.save()
        else:
            print("Not Valid?")


def get_pending_slas_for_customer(order_type: str, customer_id: int, order_id: int, slas_per_customer: QuerySet,
                                  sla_report: QuerySet):
    slas = get_customer_slas(order_type=order_type,customer_id=customer_id)
    slas_done = sla_report.filter(order_id=order_id).values('activity')
    done_sla_ids = [x["activity"] for x in slas_done]
    open_slas = [x for x in slas if x.activity_id not in done_sla_ids]
    return open_slas

def check_sla_for_overtime(sla,order_creation_date):
    settings = SettingsModel.objects.get(pk=1)
    start_worktime = settings.working_hours_start
    end_worktime = settings.working_hours_end

    current_sla_note = None

    sla_hours = int(sla.sla_time/60)
    sla_minutes = sla.sla_time-(sla_hours*60)

    _sla_time = datetime.timedelta(hours=sla_hours, minutes=sla_minutes)

    _current_time = timezone.now()

    order_creation_minutes = order_creation_date.hour * 60 + order_creation_date.minute
    worktime_start_minutes = start_worktime.hour * 60 + start_worktime.minute
    worktime_end_minutes = end_worktime.hour * 60 + end_worktime.minute

    _sla_exceeded_time = _sla_time + order_creation_date
    _sla_exceeded_minutes = _sla_exceeded_time.hour * 60 + _sla_exceeded_time.minute
    if order_creation_minutes < worktime_start_minutes:
        order_creation_date = timezone.datetime(order_creation_date.year, order_creation_date.month, order_creation_date.day,
                                         start_worktime.hour,
                                         start_worktime.minute)
        _sla_exceeded_time = _sla_time + order_creation_date
    elif _sla_exceeded_minutes > worktime_end_minutes:
        workday_end = timezone.datetime(order_creation_date.year, order_creation_date.month, order_creation_date.day,
                                        end_worktime.hour, end_worktime.minute)
        next_workday_start = timezone.datetime(order_creation_date.year, order_creation_date.month, order_creation_date.day,
                                               start_worktime.hour, start_worktime.minute) \
                             + datetime.timedelta(days=1)
        workday_end_difference = workday_end - order_creation_date
        workday_end_difference = max(workday_end_difference, datetime.timedelta(hours=0, minutes=0))
        _sla_exceeded_time = next_workday_start + _sla_time - workday_end_difference

    _sla_exceeded_weekday = _sla_exceeded_time.weekday()

    if _sla_exceeded_weekday > settings.day_of_week_work_end:
        day_difference = 7-_sla_exceeded_weekday
        _sla_exceeded_time = _sla_exceeded_time + datetime.timedelta(days=day_difference)
    for date in settings.work_free_dates:
        if _sla_exceeded_time.day == date.day and _sla_exceeded_time.month== date.month:
            _sla_exceeded_time = _sla_exceeded_time + datetime.timedelta(days=1)
    if _sla_exceeded_weekday > settings.day_of_week_work_end:
        day_difference = 7 - _sla_exceeded_weekday
        _sla_exceeded_time = _sla_exceeded_time + datetime.timedelta(days=day_difference)

    if _current_time > _sla_exceeded_time:
        exceeded = _sla_exceeded_time
        short = ".".join([x[0] for x in sla.activity.activity.split(" ")])
        current_sla_note = {"sla": short, "exceeded": exceeded}
    return current_sla_note

def update_user_groups_by_list(user_instance,groups):
    current_active_groups = Group.objects.filter(user=user_instance.id)
    correct_groups = []
    for active_group in current_active_groups:
        if active_group.name in groups:
            correct_groups.append(active_group.name)
            continue
        user_instance.groups.remove(active_group)
    for group_to_add in groups:
        if group_to_add in correct_groups:
            continue
        if type(group_to_add) is dict and "id" in group_to_add:
            group_to_add = group_to_add["id"]
        if type(group_to_add) is not int:
            raise ValidationError("Provided Group type should be integer")
        try:
            group_instance = Group.objects.get(pk=group_to_add)
        except:
            raise ValidationError(f"Could not find a group provided by id {group_to_add}")
        user_instance.groups.add(group_instance)

def update_sales_ares_per_user_by_list(user_instance,salesAreas):
    current_active_salesAreas = SalesAreasPerUser.objects.filter(user_id=user_instance.id)
    correct_salesareas = []
    for active_sa in current_active_salesAreas:
        if active_sa.id in salesAreas:
            correct_salesareas.append(active_sa.id)
            continue
        active_sa.delete()

    for sa_to_add in salesAreas:
        if sa_to_add in correct_salesareas:
            continue
        if type(sa_to_add) is dict and "id" in sa_to_add:
            sa_to_add = sa_to_add["id"]
        if type(sa_to_add) is not int:
            raise ValidationError("Provided Group type should be integer")
        try:
            sapu_instance = SalesAreasPerUser.objects.create(**{"user_id":user_instance.id,"salesarea_id":sa_to_add})
        except:
            raise ValidationError(f"Could not find a group provided by id {sa_to_add}")
        user_instance.salesareas.add(sapu_instance)
