from django.db import models

from api.managers import CustomDBManager


class MandateModel(models.Model):
    _db = 'mandate'
    objects = CustomDBManager()

    class Meta:
        abstract = True