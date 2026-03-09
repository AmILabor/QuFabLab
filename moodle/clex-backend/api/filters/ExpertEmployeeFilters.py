import django_filters
from django.core.validators import EMPTY_VALUES
from django.db import models
from django.db.models import Q

from api.models.addresse import Address
from api.models.order import Order
from api.models.appointment import Appointment
from api.models.tour import Tour
from api.models.user import MandateUser


class EmptyStringFilter(django_filters.BooleanFilter):
    def filter(self, qs, value):
        if value in EMPTY_VALUES:
            return qs

        exclude = self.exclude ^ (value is False)
        method = qs.exclude if exclude else qs.filter
        or_condition = Q()
        or_condition.add(Q(**{self.field_name:""}),Q.OR)
        or_condition.add(Q(**{self.field_name:None}),Q.OR)
        return method(or_condition)


class ExpertEmployeeOrderFilter(django_filters.FilterSet):
    order_type__not = django_filters.CharFilter(field_name='order_type',exclude=True)
    order_type__contains = django_filters.CharFilter(field_name='order_type',lookup_expr="icontains")
    license_plate = django_filters.CharFilter(field_name='license_plate',lookup_expr="exact")
    zip_prefix = django_filters.CharFilter(field_name='address__zip',lookup_expr="startswith",method='zip_prefix_filter')
    class Meta:
        model = Order
        fields = {'status': ['exact','gt','lt','lte','gte'], 'creation_date': ['gte', 'lte'], 'id': ['exact'], 'customer': ['exact'],
                  'responsible_user':['exact'],'order_type':['exact'],'searchtext':['exact'],'customer_id':['exact']}
        filter_overrides = {
            models.CharField: {
                'filter_class': django_filters.CharFilter,
                'extra': lambda f: {
                    'lookup_expr': 'icontains',
                },
            }
        }

    def zip_prefix_filter(self, queryset, name, value):
        return queryset.filter(**{
            "address__zip__startswith": value,
            "address__role":1
        })


class DispoAppointmentFilter(django_filters.FilterSet):
    appointment_date = django_filters.DateFromToRangeFilter()
    class Meta:
        model = Order
        fields = ["appointment_date"]

class DispoTourFilter(django_filters.FilterSet):
    tour_date = django_filters.DateFilter()
    expert_employee_id = django_filters.NumberFilter()
    class Meta:
        model = Tour
        fields = ["tour_date","expert_employee_id"]

class TerminFilter(django_filters.FilterSet):
    start = django_filters.DateFromToRangeFilter()
    end = django_filters.DateFromToRangeFilter()
    class Meta:
        model = Appointment
        fields = ["start","end"]

class CoWorkerFilter(django_filters.FilterSet):
    expert_number__isempty = EmptyStringFilter(field_name='expert_number')
    zip_prefix__startswith = django_filters.NumberFilter(lookup_expr="startswith",field_name="salesareas__salesarea__zip_prefix")
    class Meta:
        model = MandateUser
        fields = {'expert_number':['exact']}

class AddressFilter(django_filters.FilterSet):
    name = django_filters.CharFilter(lookup_expr='icontains')
    name2 = django_filters.CharFilter(lookup_expr='icontains')
    company = django_filters.CharFilter(lookup_expr='icontains')
    street_address = django_filters.CharFilter(lookup_expr='icontains')
    zip = django_filters.CharFilter(lookup_expr='startswith')
    city = django_filters.CharFilter(lookup_expr='icontains')
    phone = django_filters.CharFilter(lookup_expr='icontains')
    mobile = django_filters.CharFilter(lookup_expr='icontains')
    class Meta:
        model = Address
        fields = {'role':['exact']}