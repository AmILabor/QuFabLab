from django.db import models


class CustomDBManager(models.Manager):
    def get_queryset(self):
        qs = super().get_queryset()
        if hasattr(self.model, '_db'):
            qs = qs.using(self.model._db)
        return qs

