from django.db import models

from api.models.mandatemodel import MandateModel


class Workshop(MandateModel):
    creation_date = models.DateTimeField(db_column="anlagedatum",auto_created=True,auto_now_add=True)
    edit_date = models.DateTimeField(db_column="letzteaenderungdatum",auto_now=True)
    allways_valid = models.IntegerField(blank=True, null=True,db_column="dauergueltigkeit")
    company = models.CharField(max_length=256, blank=True, null=True,db_column="firma")
    name = models.CharField(max_length=256, blank=True, null=True,db_column="name")
    name2 = models.CharField(max_length=256, blank=True, null=True,db_column="name2")
    street_address = models.CharField(max_length=256, blank=True, null=True,db_column="strasse_haus_nr")
    zip = models.CharField(max_length=5, blank=True, null=True,db_column="plz")
    city = models.CharField(max_length=256, blank=True, null=True,db_column="ort")
    phone = models.CharField(max_length=64, blank=True, null=True,db_column="telefon")
    mobile = models.CharField(max_length=64, blank=True, null=True,db_column="handy")
    email = models.CharField(max_length=256, blank=True, null=True,db_column="email")
    internet = models.CharField(max_length=256, blank=True, null=True,db_column="internet")
    svs_mechanic = models.DecimalField(db_column='svsMechanik', max_digits=13, decimal_places=2, blank=True, null=True)  # Field name made lowercase.
    svs_electric = models.DecimalField(db_column='svsElektrik', max_digits=13, decimal_places=2, blank=True, null=True)  # Field name made lowercase.
    svs_paintwork = models.DecimalField(db_column='svsLackierung', max_digits=13, decimal_places=2, blank=True, null=True)  # Field name made lowercase.
    svs_body = models.FloatField(db_column='svsKarosserie', blank=True, null=True)  # Field name made lowercase.
    percentage_paintwork_material = models.DecimalField(db_column='prozLackiermaterial', max_digits=13, decimal_places=2, blank=True, null=True)  # Field name made lowercase.
    upe = models.DecimalField(max_digits=13, decimal_places=2, blank=True, null=True,db_column="upe")
    shipment = models.DecimalField(max_digits=13, decimal_places=2, blank=True, null=True,db_column="verbringung")
    zip_connection = models.CharField(db_column='plzZuordnung', max_length=5, blank=True, null=True)  # Field name made lowercasplzzuordnunge.
    make = models.CharField(max_length=255, blank=True, null=True,db_column="fabrikat")
    active = models.IntegerField(blank=True, null=True,db_column="aktiv")
    verified = models.IntegerField(blank=True, null=True,db_column="geprueft")

    class Meta:
        managed = False
        db_table = 'werkstattdef'