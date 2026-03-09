from django.contrib import admin
from api.models import *
from django.contrib.auth.admin import UserAdmin as BaseUserAdmin

from api.models.addresse import Address
from api.models.order import Order
from api.models.company import Company
from api.models.slareport import SLAActivities, SLAsPerCustomer, SLAReport
from api.models.appointment import Appointment
from api.models.user import MandateUserView, User


class CustomUserAdmin(BaseUserAdmin):
    list_display = ('username','mandate_user','first_name','last_name','email')
    fieldsets = (
        (None, {
            'fields': ('username', 'password','mandate_user')
        }),
        ('Personal info', {
            'fields': ('first_name', 'last_name', 'email')
        }),
        ('Permissions', {
            'fields': (
                'is_active', 'is_staff', 'is_superuser',
                'groups', 'user_permissions'
                )
        }),
        ('Important dates', {
            'fields': ('last_login', 'date_joined')
        }),
    )

admin.site.register(Order)
admin.site.register(Address)
admin.site.register(Company)
admin.site.register(SLAActivities)
admin.site.register(SLAsPerCustomer)
admin.site.register(SLAReport)
admin.site.register(Appointment)
admin.site.register(MandateUserView)
admin.site.register(User,CustomUserAdmin)
