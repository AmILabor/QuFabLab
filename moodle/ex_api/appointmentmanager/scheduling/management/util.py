from datetime import datetime


def set_auftrag_dates(apps, *args, **kwargs):
    SentEmails = apps.get_model('scheduling', 'SentEmails')
    Auftrag = apps.get_model('scheduling', 'Auftrag')

    for sent_mail in SentEmails.objects.all().iterator():
        try:
            auftrag = Auftrag.objects.get(id=sent_mail.auftrag_id)
            uhrzeit = datetime.strptime(auftrag.termin_uhrzeit[:5], '%H:%M')
            sent_mail.auftrag_date = auftrag.termin_datum.replace(hour=uhrzeit.hour, minute=uhrzeit.minute, second=0)
            sent_mail.save()
        except Exception as e:
            print(f'Could not find auftrag ID {sent_mail.auftrag_id}: {str(e)}')


def reverse_auftrag_dates(apps, *args, **kwargs):
    SentEmails = apps.get_model('scheduling', 'SentEmails')

    for sent_mail in SentEmails.objects.all().iterator():
        try:
            sent_mail.auftrag_date = None
            sent_mail.save()
        except Exception as e:
            print(f'Could not reverse {sent_mail.auftrag_id}: {str(e)}')


def set_auftragsart_blacklist(apps, *args, **kwargs):
    AuftragsartBlacklist = apps.get_model('scheduling', 'AuftragsartBlacklist')
    entries = ('Anfahrtkostenpauschale', 'ASS Terminanfrage', 'ASS Terminbestätigung', 'Carcheck', 'Datenblatt', 'Datenrückversand', 'Erinnerung', 'Europcar Schadenkalkulation BW', 'Freigabe', 'GDV Daten', 'GE Daten und Datenspeicher löschen', 'Gutschrift', 'Kalkulation', 'Kalkulation Six', 'Korrekturanforderung', 'Leerfahrt', 'Leerfahrt mit Berechnung', 'Lichtbildanlage', 'Lichtbildanlage mit Berechnung', 'Nachtrag', 'Nachtrag mit Berechnung', 'Onlinekalkulation (beide)', 'Prüfung', 'Prüfbericht', 'Rechnungsprüfungen', 'Sammelrechnungen', 'Schadenkalkulationen', 'Spam', 'Stellungnahmen', 'Nachbesichtigung Europa Service', 'Onlinekalkulation', 'Stationsaudit', 'Volvo Audit', 'Zustandsprotokoll')

    for entry in entries:
        auftragsart_blacklist, _ = AuftragsartBlacklist.objects.get_or_create(auftragsart=entry)
        auftragsart_blacklist.blacklisted = True
        auftragsart_blacklist.save()


def reverse_auftragsart_blacklist(apps, *args, **kwargs):
    AuftragsartBlacklist = apps.get_model('scheduling', 'AuftragsartBlacklist')

    for auftragsart_blacklist in AuftragsartBlacklist.objects.all().iterator():
        auftragsart_blacklist.blacklisted = False
        auftragsart_blacklist.save()
