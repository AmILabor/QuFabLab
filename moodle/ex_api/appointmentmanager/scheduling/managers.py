"""Custom database manager that routes queries to a model-specific database alias."""
from django.db import models


class CustomDBManager(models.Manager):
    # Manager that uses the model's `_db` attribute to select the database.
    def get_queryset(self):
        qs = super().get_queryset()
        if hasattr(self.model, '_db'):
            qs = qs.using(self.model._db)
        return qs
