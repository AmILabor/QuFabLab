from django.db import models

from moodle_api.managers import CustomDBManager


class CustomDBModel(models.Model):
    _db = 'moodle'
    objects = CustomDBManager()

    class Meta:
        abstract = True