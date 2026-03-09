
from django.db import models

from api.models.mandatemodel import MandateModel
from api.models.order import Order
from api.models.tour import Tour

class TourStop(MandateModel):
    order = models.ForeignKey(Order, db_column="auftrag_id", on_delete=models.DO_NOTHING)
    #tour = models.ForeignKey(Tour, db_column="tour_id",on_delete=models.CASCADE,related_name="tour_stops")

    class Meta:
        managed = False
        db_table = "tour_stop"
