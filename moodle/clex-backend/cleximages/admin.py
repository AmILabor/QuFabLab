from django.contrib import admin

# Register your models here.
from cleximages.models import AuftragBild,ClexImageUser

admin.site.register(AuftragBild)
admin.site.register(ClexImageUser)
