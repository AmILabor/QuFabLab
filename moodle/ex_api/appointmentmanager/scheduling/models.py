from django.db import models
from .managers import CustomDBManager


class ReadOnlyModel(models.Model):
    _db = 'prod'
    objects = CustomDBManager()

    class Meta:
        abstract = True

    def save(self, *args, **kwargs):
        return

    def delete(self, *args, **kwargs):
        return


class Auftrag(ReadOnlyModel):
    _db = 'prod'
    objects = CustomDBManager()

    id = models.IntegerField(primary_key=True)
    guid = models.CharField(max_length=64, blank=True, null=True)
    anlagedatum = models.DateTimeField()
    auftraggeber = models.CharField(max_length=128, blank=True, null=True)
    auftrags_nr_mac = models.CharField(max_length=128, blank=True, null=True)
    auftrags_nr_af = models.CharField(max_length=128, blank=True, null=True)
    strasse = models.CharField(max_length=256, blank=True, null=True)
    plz = models.CharField(max_length=5, blank=True, null=True)
    email = models.CharField(max_length=128, blank=True, null=True)
    ort = models.CharField(max_length=128, blank=True, null=True)
    status = models.IntegerField(blank=True, null=True)
    auftragsart = models.CharField(max_length=128, blank=True, null=True)
    termin_datum = models.DateTimeField(blank=True, null=True)
    termin_tageszeit = models.CharField(max_length=32, blank=True, null=True)
    termin_uhrzeit = models.CharField(max_length=32, blank=True, null=True)
    termin_uhrzeit_bis = models.CharField(max_length=32, blank=True, null=True)
    fabrikat = models.CharField(max_length=128, blank=True, null=True)
    typ = models.CharField(max_length=128, blank=True, null=True)
    utyp = models.CharField(max_length=128, blank=True, null=True)
    kennzeichen = models.CharField(max_length=32, blank=True, null=True)
    kennzeichen_unfgeg = models.CharField(max_length=32, blank=True, null=True)
    fzgidnr = models.CharField(max_length=64, blank=True, null=True)
    kbanr = models.CharField(max_length=32, blank=True, null=True)
    storno = models.IntegerField(blank=True, null=True)
    leistung = models.IntegerField(blank=True, null=True)
    hubraum = models.IntegerField(blank=True, null=True)
    schadentag = models.DateTimeField(blank=True, null=True)
    selbstbet_vk = models.FloatField(blank=True, null=True)
    selbstbet_tk = models.FloatField(blank=True, null=True)
    naechstehu = models.CharField(max_length=32, blank=True, null=True)
    vorschaden = models.CharField(max_length=32, blank=True, null=True)
    altschaden = models.CharField(max_length=1000, blank=True, null=True)
    hsn = models.CharField(max_length=32, blank=True, null=True)
    tsn = models.CharField(max_length=32, blank=True, null=True)
    schadenbereich = models.CharField(max_length=1000, blank=True, null=True)
    motor = models.CharField(max_length=128, blank=True, null=True)
    lackierung = models.CharField(max_length=128, blank=True, null=True)
    getriebe = models.CharField(max_length=128, blank=True, null=True)
    bruttoliste = models.FloatField(blank=True, null=True)
    wv_am = models.DateTimeField(db_column='WV_am', blank=True, null=True)  # Field name made lowercase.
    do_not_read_att = models.IntegerField(blank=True, null=True)
    do_not_read_anh = models.IntegerField(blank=True, null=True)
    verantw_bereich = models.IntegerField(blank=True, null=True)
    verantw_mitarb = models.IntegerField(blank=True, null=True)
    is_nfz = models.IntegerField(db_column='is_NFZ', blank=True, null=True)  # Field name made lowercase.
    historie = models.CharField(max_length=20000, blank=True, null=True)
    vvs = models.CharField(max_length=128, blank=True, null=True)
    storno_grund = models.CharField(max_length=256, blank=True, null=True)
    rwb_bis = models.DateTimeField(blank=True, null=True)
    fertiggestellt_am = models.DateTimeField(blank=True, null=True)
    bestelltext = models.CharField(max_length=1000, blank=True, null=True)
    anmerkung = models.CharField(max_length=2000, blank=True, null=True)
    anmerkung_intern = models.CharField(max_length=2000, blank=True, null=True)
    schadennummer = models.CharField(max_length=128, blank=True, null=True)
    versicherungs_nr = models.CharField(max_length=128, blank=True, null=True)
    bestell_nr = models.CharField(max_length=128, blank=True, null=True)
    vertrags_nr = models.CharField(max_length=128, blank=True, null=True)
    auftraggeber_id = models.IntegerField(blank=True, null=True)
    motorart_id = models.IntegerField(blank=True, null=True)
    fahrzeugart_id = models.IntegerField(blank=True, null=True)
    angelegt_von_id = models.IntegerField(blank=True, null=True)
    sv_id = models.IntegerField(blank=True, null=True)
    sv_innen_id = models.IntegerField(blank=True, null=True)
    kopie_von_id = models.IntegerField(blank=True, null=True)
    aufbauart_id = models.IntegerField(blank=True, null=True)
    sammelbesichtigung = models.IntegerField(blank=True, null=True)
    zulassung = models.DateTimeField(blank=True, null=True)
    letztezulassung = models.DateTimeField(blank=True, null=True)
    grundhonorar = models.FloatField(blank=True, null=True)
    fahrtkosten = models.FloatField(blank=True, null=True)
    fotokosten = models.FloatField(blank=True, null=True)
    sonstigekosten = models.FloatField(blank=True, null=True)
    rechnungsbetrag = models.FloatField(blank=True, null=True)
    beim_pdl = models.IntegerField(blank=True, null=True)
    in_rwb = models.IntegerField(blank=True, null=True)
    versteckt = models.IntegerField(blank=True, null=True)
    exportierenaf = models.IntegerField(db_column='exportierenAF', blank=True, null=True)  # Field name made lowercase.
    kundenwunschtermin = models.IntegerField(blank=True, null=True)
    akquisesv = models.IntegerField(db_column='akquiseSV', blank=True, null=True)  # Field name made lowercase.
    bearbeitungdatum = models.DateTimeField(blank=True, null=True)
    express = models.IntegerField(blank=True, null=True)
    searchtext = models.CharField(max_length=1024, blank=True, null=True)
    userstatus = models.IntegerField(blank=True, null=True)
    fahrzeit = models.IntegerField(blank=True, null=True)
    fahrkm = models.IntegerField(blank=True, null=True)
    cst_nr = models.IntegerField(blank=True, null=True)
    gerechtfertigt = models.IntegerField(blank=True, null=True)
    lizensnehmer_id = models.IntegerField(blank=True, null=True)
    svsmechanik = models.FloatField(db_column='svsMechanik', blank=True, null=True)  # Field name made lowercase.
    svselektrik = models.FloatField(db_column='svsElektrik', blank=True, null=True)  # Field name made lowercase.
    svslackierung = models.FloatField(db_column='svsLackierung', blank=True, null=True)  # Field name made lowercase.
    prozlackiermaterial = models.FloatField(db_column='prozLackiermaterial', blank=True, null=True)  # Field name made lowercase.
    upe = models.FloatField(blank=True, null=True)
    verbringung = models.FloatField(blank=True, null=True)
    svskarosserie = models.FloatField(db_column='svsKarosserie', blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'auftrag'


class Benutzer(ReadOnlyModel):
    _db = 'prod'
    objects = CustomDBManager()

    id = models.IntegerField(primary_key=True)
    guid = models.CharField(max_length=64, blank=True, null=True)
    anlagedatum = models.DateTimeField()
    aktiv = models.IntegerField(blank=True, null=True)
    benutzer = models.CharField(max_length=256, blank=True, null=True)
    kennwort = models.CharField(max_length=256, blank=True, null=True)
    vorname = models.CharField(max_length=256, blank=True, null=True)
    nachname = models.CharField(max_length=256, blank=True, null=True)
    strasse = models.CharField(max_length=256, blank=True, null=True)
    plz = models.CharField(max_length=5, blank=True, null=True)
    ort = models.CharField(max_length=256, blank=True, null=True)
    land = models.CharField(max_length=256, blank=True, null=True)
    geburtsdatum = models.DateField(blank=True, null=True)
    telefon = models.CharField(max_length=32, blank=True, null=True)
    mobil = models.CharField(max_length=32, blank=True, null=True)
    fax = models.CharField(max_length=32, blank=True, null=True)
    email = models.CharField(max_length=256, blank=True, null=True)
    internet = models.CharField(max_length=256, blank=True, null=True)
    postfach = models.CharField(max_length=32, blank=True, null=True)
    svnummer = models.CharField(max_length=32, blank=True, null=True)
    buero_id = models.IntegerField(blank=True, null=True)
    firma_id = models.IntegerField(blank=True, null=True)
    unsichtbar = models.IntegerField(blank=True, null=True)
    audanet_id = models.CharField(max_length=256, blank=True, null=True)
    status = models.IntegerField(blank=True, null=True)
    verguetungsmodel = models.IntegerField(blank=True, null=True)

    class Meta:
        managed = False
        db_table = 'benutzer'


class Firma(ReadOnlyModel):
    _db = 'prod'
    objects = CustomDBManager()

    id = models.IntegerField(primary_key=True)
    guid = models.CharField(max_length=64, blank=True, null=True)
    anlagedatum = models.DateTimeField()
    aktiv = models.IntegerField(blank=True, null=True)
    name1 = models.CharField(max_length=255, blank=True, null=True)
    name2 = models.CharField(max_length=255, blank=True, null=True)
    strasse = models.CharField(max_length=255, blank=True, null=True)
    plz = models.CharField(max_length=255, blank=True, null=True)
    ort = models.CharField(max_length=255, blank=True, null=True)
    land = models.CharField(max_length=255, blank=True, null=True)
    telefon = models.CharField(max_length=255, blank=True, null=True)
    fax = models.CharField(max_length=255, blank=True, null=True)
    internet = models.CharField(max_length=255, blank=True, null=True)
    email = models.CharField(max_length=255, blank=True, null=True)
    postfach = models.CharField(max_length=255, blank=True, null=True)
    kurzbezeichnung = models.CharField(max_length=255, blank=True, null=True)
    navisionnr = models.CharField(db_column='navisionNr', max_length=100, blank=True, null=True)  # Field name made lowercase.
    ustid = models.CharField(db_column='ustId', max_length=100, blank=True, null=True)  # Field name made lowercase.
    abteilung = models.CharField(max_length=100, blank=True, null=True)

    class Meta:
        managed = False
        db_table = 'firma'


class Adresse(ReadOnlyModel):
    _db = 'prod'
    objects = CustomDBManager()

    id = models.IntegerField(primary_key=True)
    guid = models.CharField(max_length=64, blank=True, null=True)
    anlagedatum = models.DateTimeField()
    firma = models.CharField(max_length=256, blank=True, null=True)
    name = models.CharField(max_length=256, blank=True, null=True)
    name2 = models.CharField(max_length=256, blank=True, null=True)
    strasse_haus_nr = models.CharField(max_length=256, blank=True, null=True)
    plz = models.CharField(max_length=5, blank=True, null=True)
    ort = models.CharField(max_length=256, blank=True, null=True)
    telefon = models.CharField(max_length=64, blank=True, null=True)
    handy = models.CharField(max_length=64, blank=True, null=True)
    email = models.CharField(max_length=256, blank=True, null=True)
    internet = models.CharField(max_length=256, blank=True, null=True)
    platz = models.CharField(max_length=256, blank=True, null=True)
    reihe = models.CharField(max_length=256, blank=True, null=True)
    stellplatz = models.CharField(max_length=256, blank=True, null=True)
    rolle = models.IntegerField(blank=True, null=True)
    auftrag_id = models.IntegerField(blank=True, null=True)

    class Meta:
        managed = False
        db_table = 'adresse'


class CustomerTemplate(models.Model):
    firma = models.IntegerField(primary_key=True)
    confirmation = models.TextField(default='Bestätigung')
    reminder = models.TextField(default='Erinnerung')
    content = models.TextField(blank=True)


class SentEmailSettings(models.Model):
    test_mailing = models.BooleanField(default=True, help_text='If enabled, removes the mail recipient and only sends to CC')
    cc_target = models.EmailField(max_length=255, default="", blank=True, help_text='If set, adds this address to CC')
    remind_interval_hours = models.IntegerField(default=24, help_text="Duration in hours before a reminder is sent")
    logfile_dir = models.CharField(max_length=255,default=r"C:\logs")
    newsletter_link = models.CharField(max_length=255,default=r"", blank=True, help_text="Not in use yet")
    newsletter_link_text = models.CharField(max_length=255,default=r"", blank=True, help_text="Not in use yet")
    smtp_server = models.CharField(max_length=255,default=r"")
    smtp_user = models.CharField(max_length=255,default=r"")
    smtp_password = models.CharField(max_length=255,default=r"")
    smtp_port = models.CharField(max_length=8,default=r"")
    smtp_sender = models.CharField(max_length=255, default="auftrag@claimsexperts.de", help_text="Sender mail address")
    mail_error_receiver = models.CharField(max_length=255, default='schaden@claimsexperts.de', blank=True, help_text='Default receiving e-mail in case reminder/confirmation cannot be sent')
    mail_errors = models.BooleanField(default=False, help_text='If enabled, sends failed mail deliveries to respective Mitarbeiter, or to mail error receiver if Mitarbeiter is not set')
    use_tls = models.BooleanField(default=False)
    use_ssl = models.BooleanField(default=False)
    default_confirmation = models.TextField(default='Bestätigung')
    default_reminder = models.TextField(default='Erinnerung')
    mail_template = models.TextField(blank=True, help_text="""HTML mail content. Possible placeholders:
        - %appointment_data% (includes the customer's date, time, ...)
        - %content% (dynamic content included at the bottom of the mail)
    """)
    calendar_subject = models.CharField(max_length=255, default="Ihr Termin", help_text=".ics file subject")
    calendar_description = models.TextField(default="", blank=True, help_text=".ics file description")


class SentEmails(models.Model):
    auftrag_id = models.IntegerField(primary_key=True)
    sent_confirmation_datetime = models.DateTimeField()
    sent_reminder_datetime = models.DateTimeField('Reminder sent datetime',null=True, blank=True)
    auftrag_date = models.DateTimeField(null=True, blank=True, help_text='Used to check if Auftrag date was changed')


class Newsletter(models.Model):
    email = models.CharField(max_length=255, unique=True)
    date = models.DateTimeField()


class AuftragsartBlacklist(models.Model):
    auftragsart = models.CharField(max_length=255, unique=True)
    blacklisted = models.BooleanField(default=True, help_text='If set, Auftragsart will be skipped for mail delivery')

    def __str__(self):
        return f'{self.auftragsart}: {self.blacklisted}'
