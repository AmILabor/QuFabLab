import hashlib

from django.db import models
from django.conf import settings

from api.models.order import Order
from api.models.mandatemodel import MandateModel
from api.models.user import User, MandateUser


class ClexImageUser(models.Model):
    user = models.OneToOneField(User, on_delete=models.CASCADE)

    def __str__(self):
        return f"{self.user.mandate_user.firstname} {self.user.mandate_user.lastname} (SV_ID:{self.user.mandate_user.id})"


class AuftragBild(MandateModel):
    id = models.IntegerField(primary_key=True,blank=True)
    order_id = models.ForeignKey(to=Order, on_delete = models.DO_NOTHING, db_column="auftrag_id")
    user_id = models.ForeignKey(to=MandateUser, on_delete = models.DO_NOTHING, db_column="benutzer_id")
    upload_datetime = models.DateTimeField(auto_now=True)
    image = models.ImageField(upload_to=settings.CLEX_IMAGES_UPLOAD_PATH)
    image_hash = models.CharField(max_length=16, editable=False,blank=True)

    class Meta:
        managed = False
        db_table = 'auftrag_bilder'

    def save(self,*args,**kwargs):
        if not self.pk:
            md5 = hashlib.md5()
            for chunk in self.image.chunks():
                md5.update(chunk)
            self.image_hash = md5.hexdigest()
            self.bild_path = self.image.path
            super(AuftragBild,self).save(*args,**kwargs)

    def delete(self,using=None, keep_parents = False):
        self.image.storage.delete(self.image.name)
        super().delete()