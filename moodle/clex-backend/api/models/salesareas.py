from django.db import models

from api.models.mandatemodel import MandateModel
from api.models.user import MandateUser


class SalesAreas(MandateModel):
    zip_prefix = models.IntegerField(db_column="plz_prefix")
    name = models.CharField(db_column="bezeichnung",max_length=255)
    class Meta:
        managed = False
        db_table = "vertriebsgebiete"


class SalesAreasPerUser(MandateModel):
    salesarea = models.ForeignKey(SalesAreas, on_delete = models.PROTECT, related_name="salesarea", db_column="vertriebsgebiet_id")
    user = models.ForeignKey(MandateUser, on_delete=models.DO_NOTHING, related_name="salesareas", db_column="benutzer_id")
    class Meta:
        managed = False
        db_table = "vertriebsgebiete_per_user"