from django.contrib import admin
from .models import CustomerTemplate, SentEmailSettings, SentEmails, Newsletter, AuftragsartBlacklist

# Register your models here.

admin.site.register(CustomerTemplate)
admin.site.register(SentEmailSettings)
admin.site.register(SentEmails)
admin.site.register(AuftragsartBlacklist)
# admin.site.register(Newsletter)
