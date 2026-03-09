from moodle_api.models.custom_db_model import CustomDBModel
from django.db import models

class MdlQuestion(CustomDBModel):
    id = models.BigAutoField(primary_key=True)
    parent = models.BigIntegerField()
    name = models.CharField(max_length=255)
    questiontext = models.TextField()
    questiontextformat = models.IntegerField()
    generalfeedback = models.TextField()
    generalfeedbackformat = models.IntegerField()
    defaultmark = models.DecimalField(max_digits=12, decimal_places=7)
    penalty = models.DecimalField(max_digits=12, decimal_places=7)
    qtype = models.CharField(max_length=20)
    length = models.BigIntegerField()
    stamp = models.CharField(max_length=255)
    timecreated = models.BigIntegerField()
    timemodified = models.BigIntegerField()
    createdby = models.BigIntegerField(blank=True, null=True)
    modifiedby = models.BigIntegerField(blank=True, null=True)

    class Meta:
        managed = False
        db_table = 'mdl_question'
