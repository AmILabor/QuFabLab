from django.db import models

from api.models.mandatemodel import MandateModel


class Address(MandateModel):
    id = models.IntegerField(primary_key=True)
    guid = models.CharField(max_length=64, blank=True, null=True)
    creation_date = models.DateTimeField(db_column="anlagedatum",auto_now_add=True)
    company = models.CharField(db_column="firma",max_length=256, blank=True, null=True)
    name = models.CharField(max_length=256, blank=True, null=True)
    name2 = models.CharField(max_length=256, blank=True, null=True)
    street_address = models.CharField(db_column="strasse_haus_nr",max_length=256, blank=True, null=True)
    zip = models.CharField(db_column="plz", max_length=5, blank=True, null=True)
    city = models.CharField(db_column="ort", max_length=256, blank=True, null=True)
    phone = models.CharField(db_column="telefon", max_length=64, blank=True, null=True)
    mobile = models.CharField(db_column="handy", max_length=64, blank=True, null=True)
    email = models.CharField(max_length=256, blank=True, null=True)
    internet = models.CharField(max_length=256, blank=True, null=True)
    spot = models.CharField(db_column="platz",max_length=256, blank=True, null=True)
    row = models.CharField(db_column="reihe",max_length=256, blank=True, null=True)
    parking_space = models.CharField(db_column="stellplatz",max_length=256, blank=True, null=True)
    role = models.IntegerField(db_column="rolle",blank=True, null=True)
    order = models.ForeignKey(to='Order', on_delete=models.DO_NOTHING,db_column="auftrag_id",related_name='address')

    class Meta:
        managed = False
        db_table = 'adresse'