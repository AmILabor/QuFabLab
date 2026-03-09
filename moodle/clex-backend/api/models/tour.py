from django.db import models
from api.models.mandatemodel import MandateModel
from api.models.user import MandateUser

class Tour(MandateModel):
    sent_by = models.ForeignKey(MandateUser, on_delete=models.DO_NOTHING, db_column="sender_id")
    expert_employee = models.ForeignKey(MandateUser, on_delete=models.DO_NOTHING,db_column="benutzer_id", related_name="tours")
    sent_at = models.DateTimeField(default=None, db_column="gesendet_timestamp")
    created_at = models.DateTimeField(auto_created=True,auto_now_add=True, db_column="erstellung_timestamp")
    tour_date = models.DateTimeField(db_column="tour_datum")

    class Meta:
        managed = False
        db_table = "touren"

