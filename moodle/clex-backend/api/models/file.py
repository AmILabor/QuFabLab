from django.db import models

from api.models.order import Order
from api.models.mandatemodel import MandateModel
from api.services import generate_file_path


class File(MandateModel):
    guid = models.CharField(max_length=64, blank=True, null=True)
    creation_date = models.DateTimeField(db_column="anlagedatum",auto_now_add=True)
    name = models.CharField(max_length=512, blank=True, null=True)
    extension = models.CharField(db_column="erweiterung",max_length=512, blank=True, null=True)
    order_id = models.ForeignKey(to=Order, on_delete=models.DO_NOTHING, db_column="auftrag_id", related_name='attachments')
    typ = models.IntegerField(blank=True, null=True)
    appointment_id = models.IntegerField(db_column="termin_id",blank=True, null=True)
    invoice_id = models.IntegerField(db_column="rechnung_id",blank=True, null=True)

    class Meta:
        managed = False
        db_table = 'datei'

    @property
    def path(self):
        return generate_file_path(self.name,self.extension)