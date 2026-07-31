"""Benutzerdefinierte Datenbank-Manager für Moodle-Modelle."""
from django.db import models


# Manager, der Abfragen automatisch auf die richtige Datenbank-Route umleitet.
class CustomDBManager(models.Manager):
    # Überschreibt get_queryset, um die Datenbank-Verbindung (using) zu setzen.
    def get_queryset(self):
        qs = super().get_queryset()
        if hasattr(self.model, '_db'):
            qs = qs.using(self.model._db)
        return qs

