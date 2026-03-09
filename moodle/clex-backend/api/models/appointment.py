from django.db import models

from api.models.mandatemodel import MandateModel
from api.models.user import MandateUser


class Appointment(MandateModel):
    id = models.IntegerField(primary_key=True)
    guid = models.CharField(max_length=64, blank=True, null=True)
    creation_date = models.DateTimeField(db_column="anlagedatum")
    status = models.IntegerField(blank=True, null=True)
    start = models.DateTimeField(db_column="von",blank=True, null=True)
    end = models.DateTimeField(db_column="bis",blank=True, null=True)
    start_time = models.CharField(db_column="von_uhrzeit",max_length=32, blank=True, null=True)
    end_time = models.CharField(db_column="bis_uhrzeit",max_length=32, blank=True, null=True)
    note = models.CharField(db_column="vermerk",max_length=1024, blank=True, null=True)
    comment = models.CharField(db_column="bemerkung",max_length=10000, blank=True, null=True)
    reason = models.IntegerField(db_column="grund",blank=True, null=True)
    order_number = models.CharField(db_column="auftragsnrs",max_length=1024, blank=True, null=True)
    expert_employee_id = models.ForeignKey(to=MandateUser, db_column='sv_id', related_name="appointment_expert_employee", on_delete=models.DO_NOTHING, blank=True, null=True)

    class Meta:
        managed = False
        db_table = 'termin'