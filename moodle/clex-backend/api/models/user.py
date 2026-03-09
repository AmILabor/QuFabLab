from django.contrib.auth.models import AbstractUser
from django.db import models

from api.models.company import Company
from api.models.mandatemodel import MandateModel

class MandateUserView(models.Model):
    id = models.IntegerField(primary_key=True)
    guid = models.CharField(max_length=64, blank=True, null=True)
    creation_date = models.DateTimeField(db_column="anlagedatum",auto_now_add=True,auto_created=True,)
    active = models.IntegerField(db_column="aktiv",blank=True, null=True)
    user = models.CharField(db_column="benutzer",max_length=256, blank=True, null=True)
    #kennwort = models.CharField(max_length=256, blank=True, null=True)
    firstname = models.CharField(db_column="vorname", max_length=256, blank=True, null=True)
    lastname = models.CharField(db_column="nachname", max_length=256, blank=True, null=True)
    street = models.CharField(db_column="strasse",max_length=256, blank=True, null=True)
    zip = models.CharField(db_column="plz",max_length=5, blank=True, null=True)
    city = models.CharField(db_column="ort",max_length=256, blank=True, null=True)
    country = models.CharField(db_column="land",max_length=256, blank=True, null=True)
    birth_date = models.DateField(db_column="geburtsdatum",blank=True, null=True)
    phone = models.CharField(db_column="telefon", max_length=32, blank=True, null=True)
    mobil = models.CharField(max_length=32, blank=True, null=True)
    fax = models.CharField(max_length=32, blank=True, null=True)
    email = models.CharField(max_length=256, blank=True, null=True)
    internet = models.CharField(max_length=256, blank=True, null=True)
    post_office_box = models.CharField(db_column="postfach",max_length=32, blank=True, null=True)
    expert_number = models.CharField(db_column="svnummer",max_length=32, blank=True, null=True)
    office_id = models.IntegerField(db_column="buero_id",blank=True, null=True)
    company_id = models.IntegerField(db_column="firma_id",blank=True, null=True)
    hidden = models.IntegerField(db_column="unsichtbar",blank=True, null=True)
    audanet_id = models.CharField(max_length=256, blank=True, null=True)
    status = models.IntegerField(blank=True, null=True)
    compensation_model = models.IntegerField(db_column="verguetungsmodel",blank=True, null=True)

    class Meta:
        managed=False
        db_table = 'api_benutzer'

    def __str__(self):
        return f"{self.firstname} {self.lastname} (SV_ID: {self.id})"

    @property
    def fullname(self):
        return f"{self.firstname} {self.lastname}"

class MandateUser(MandateModel):
    id = models.BigAutoField(primary_key=True)
    guid = models.CharField(max_length=64, blank=True, null=True)
    creation_date = models.DateTimeField(db_column="anlagedatum",auto_now_add=True,auto_created=True)
    active = models.IntegerField(db_column="aktiv",blank=True, null=True)
    user = models.CharField(db_column="benutzer",max_length=256, blank=True, null=True)
    #kennwort = models.CharField(max_length=256, blank=True, null=True)
    firstname = models.CharField(db_column="vorname", max_length=256, blank=True, null=True)
    lastname = models.CharField(db_column="nachname",max_length=256, blank=True, null=True)
    street = models.CharField(db_column="strasse",max_length=256, blank=True, null=True)
    zip = models.CharField(db_column="plz",max_length=5, blank=True, null=True)
    city = models.CharField(db_column="ort", max_length=256, blank=True, null=True)
    country = models.CharField(db_column="land",max_length=256, blank=True, null=True)
    birth_date = models.DateField(db_column="geburtsdatum",blank=True, null=True)
    phone = models.CharField(db_column="telefon", max_length=32, blank=True, null=True)
    mobil = models.CharField(max_length=32, blank=True, null=True)
    fax = models.CharField(max_length=32, blank=True, null=True)
    email = models.CharField(max_length=256, blank=True, null=True)
    internet = models.CharField(max_length=256, blank=True, null=True)
    post_office_box = models.CharField(db_column="postfach",max_length=32, blank=True, null=True)
    expert_number = models.CharField(db_column="svnummer",max_length=32, blank=True, null=True)
    office_id = models.IntegerField(db_column="buero_id",blank=True, null=True)
    company = models.ForeignKey(to=Company,db_column="firma_id", blank=True, null=True, on_delete=models.DO_NOTHING)
    hidden = models.IntegerField(db_column="unsichtbar",blank=True, null=True)
    audanet_id = models.CharField(max_length=256, blank=True, null=True)
    status = models.IntegerField(blank=True, null=True)
    compensation_model = models.IntegerField(db_column="verguetungsmodel", blank=True, null=True)

    class Meta:
        managed = False
        db_table = 'benutzer'

    @property
    def fullname(self):
        return f"{self.firstname} {self.lastname}"


class User(AbstractUser):
    mandate_user = models.ForeignKey(to=MandateUserView, on_delete=models.DO_NOTHING, null=True, blank=True, related_name="mandate_user")
