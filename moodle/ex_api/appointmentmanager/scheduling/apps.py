"""App configuration for the scheduling app."""
from django.apps import AppConfig


class SchedulingConfig(AppConfig):
    # App configuration for the scheduling app.
    default_auto_field = 'django.db.models.BigAutoField'
    name = 'scheduling'
