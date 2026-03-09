from django.db import models

from api.models.order import Order
from api.models.mandatemodel import MandateModel
from api.models.user import MandateUser


class OrderLock(MandateModel):
    order = models.OneToOneField(Order, on_delete=models.DO_NOTHING, db_column="auftrag_id", primary_key=True, related_name="lock")
    user = models.ForeignKey(MandateUser, on_delete=models.DO_NOTHING, db_column="benutzer_id", null=False)
    timestamp = models.DateTimeField(auto_now_add=True,null=False)

    class Meta:
        managed = False
        db_table = 'auftrag_sperre'