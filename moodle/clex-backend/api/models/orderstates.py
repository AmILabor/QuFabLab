from django.db import models

from api.models.mandatemodel import MandateModel


class OrderStates(MandateModel):
    id = models.IntegerField(primary_key=True)
    name = models.CharField(max_length=128, blank=True, null=True)

    class Meta:
        managed = False
        db_table = 'auftrag_status'