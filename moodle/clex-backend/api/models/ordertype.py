from django.db import models

from api.models.mandatemodel import MandateModel


class OrderType(MandateModel):
    name = models.CharField(max_length=256)
    class Meta:
        managed = False
        db_table = 'v_auftragsart'