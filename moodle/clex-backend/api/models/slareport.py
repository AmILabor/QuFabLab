from django.db import models

from api.models.company import Company
from api.models.mandatemodel import MandateModel
from api.models.user import MandateUser


class SLAActivities(MandateModel):
    activity=models.CharField(max_length=256,blank=False,null=False)
    class Meta:
        managed = False
        db_table = 'slaactivities'

class SLAsPerOrderType(MandateModel):
    order_type = models.CharField(max_length=255,db_column="order_type")
    sla_time = models.IntegerField(db_column="default_sla")
    activity = models.ForeignKey(SLAActivities,on_delete=models.DO_NOTHING)

    class Meta:
        managed = False
        db_table = 'sla_per_ordertype'

class SLAsPerCustomer(MandateModel):
    activity = models.ForeignKey(SLAActivities,on_delete=models.DO_NOTHING)
    sla_time = models.IntegerField()
    order_type = models.CharField(db_column="auftragsart",max_length=128)
    customer = models.ForeignKey(Company, on_delete=models.DO_NOTHING, related_name="slas", db_column="auftraggeber_id")
    class Meta:
        managed = False
        db_table = 'slapercustomer'

class SLAReport(MandateModel):
    order = models.ForeignKey('Order',on_delete=models.DO_NOTHING, db_column="auftrag_id",related_name="sla_report", primary_key=True)
    activity = models.ForeignKey(SLAActivities,on_delete=models.DO_NOTHING, db_column="activity_id")
    timestamp = models.DateTimeField(auto_now=True)
    user = models.ForeignKey(MandateUser, on_delete = models.DO_NOTHING, db_column="user_id")
    order_creation_date = models.DateTimeField(null=False)
    sla_limit = models.IntegerField(null=False)

    class Meta:
        managed = False
        db_table = 'slareport'
        constraints = [
            models.UniqueConstraint(fields=['order_id', 'user_id'], name='composite key')
        ]

