from django.db import models

from api.models.mandatemodel import MandateModel


class Company(MandateModel):
    id = models.IntegerField(primary_key=True)
    creation_date = models.DateTimeField(db_column="anlagedatum", auto_now_add=True,auto_created=True)
    active = models.IntegerField(db_column="aktiv",blank=True, null=True)
    name1 = models.CharField(max_length=255, blank=True, null=True)
    name2 = models.CharField(max_length=255, blank=True, null=True)
    street = models.CharField(db_column="strasse",max_length=255, blank=True, null=True)
    zip = models.CharField(db_column="plz",max_length=255, blank=True, null=True)
    city = models.CharField(db_column="ort", max_length=255, blank=True, null=True)
    country = models.CharField(db_column="land",max_length=255, blank=True, null=True)
    phone = models.CharField(db_column="telefon", max_length=255, blank=True, null=True)
    fax = models.CharField(max_length=255, blank=True, null=True)
    internet = models.CharField(max_length=255, blank=True, null=True)
    email = models.CharField(max_length=255, blank=True, null=True)
    post_office_box = models.CharField(db_column="postfach",max_length=255, blank=True, null=True)
    abbreviation = models.CharField(db_column="kurzbezeichnung",max_length=255, blank=True, null=True)
    navision_number = models.CharField(db_column='navisionNr', max_length=100, blank=True, null=True)  # Field name made lowercase.
    ustid = models.CharField(db_column='ustId', max_length=100, blank=True, null=True)  # Field name made lowercase.
    department = models.CharField(db_column="abteilung",max_length=100, blank=True, null=True)

    class Meta:
        managed = False
        db_table = 'firma'
