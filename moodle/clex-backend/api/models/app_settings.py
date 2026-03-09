from django.db import models
from django.core.cache import cache

APP_SETTINGS_CACHE_KEY = 'APP_SETTINGS'
APP_SETTINGS_WORKDAY_CACHE_KEY = 'APP_SETTINGS_WORKDAYS'

class SettingsQuerySet(models.query.QuerySet):
    def get(self,  *args,**kwargs):
        cached = cache.get(APP_SETTINGS_CACHE_KEY,default=False)
        if cached is False:
            cached = super().get(*args,**kwargs)
            cache.set(APP_SETTINGS_CACHE_KEY,cached)
        return cached

    def update(self,**kwargs):
        cache.delete(APP_SETTINGS_CACHE_KEY)
        return super().update(**kwargs)

class WorkFreeDatesModel(models.Model):
    day = models.IntegerField(null=False)
    month = models.IntegerField(null=False)
    settings = models.ForeignKey('SettingsModel',on_delete=models.DO_NOTHING,default=1)

class SettingsModel(models.Model):
    objects = SettingsQuerySet.as_manager()
    working_hours_start = models.TimeField(null=False)
    working_hours_end = models.TimeField(null=False)
    day_of_week_work_start = models.IntegerField(null=False)
    day_of_week_work_end = models.IntegerField(null=False)
    email_server_port = models.CharField(max_length=255, null=False)
    email_server = models.CharField(max_length=255, null=False)
    email_user = models.CharField(max_length=255, null=False)
    email_password = models.CharField(max_length=255, null=False)
    email_debug = models.BooleanField(null=False,default=True)
    email_debug_receiver = models.CharField(max_length=255,null=False,default="me@wintersections.de")

    def save(self,*args,**kwargs):
        cache.delete(APP_SETTINGS_CACHE_KEY)
        return super().save(**kwargs)

    @property
    def work_free_dates(self):
        """
        Cached Version of the reverse relation between work days and settings.
        """
        cached = cache.get(APP_SETTINGS_WORKDAY_CACHE_KEY,default=False)
        if cached is False:
            cached =list(WorkFreeDatesModel.objects.filter(settings=self.id))
            cache.set(APP_SETTINGS_WORKDAY_CACHE_KEY,cached)
        return cached


