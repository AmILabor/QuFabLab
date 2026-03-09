from datetime import datetime

from django.db.models import Q, QuerySet
from api.models.company import Company
from api.models.slareport import SLAsPerCustomer, SLAsPerOrderType
from itertools import chain

def get_customer_slas(customer_id, order_type):

    default_slas = SLAsPerOrderType.objects.filter(Q(order_type=order_type)).order_by('sla_time')

    customer_slas = SLAsPerCustomer.objects.filter(
        Q(customer_id=customer_id) & Q(order_type=order_type)).order_by('sla_time')

    customer_sla_activities = [x.activity for x in customer_slas]
    default_slas = default_slas.filter(~Q(activity__in=customer_sla_activities))

    sla = list(chain(default_slas,customer_slas))

    return sla

def get_customer_activity_sla(customer_id, order_type, activity_id):
    sla = SLAsPerCustomer.objects.filter(
        Q(customer_id=customer_id) & Q(order_type=order_type) & Q(
            activity=activity_id))
    if len(sla)>0:
        return sla[0]
    default_slas = SLAsPerOrderType.objects.filter(Q(order_type=order_type, activity=activity_id))
    if len(default_slas)>0:
        return default_slas[0]
    return None


def search_connectable_orders_freetext(queryset: QuerySet, date_limit: datetime, primary_order_id: int, search_word: [str, int]):
    base_filter = Q(creation_date__gt=date_limit) & ~Q(id=primary_order_id)

    if search_word:
        base_filter = base_filter & (Q(connected_order__isnull=True) | Q(connected_order=primary_order_id))
        if search_word.isdigit():
            base_filter = base_filter & (Q(id=int(search_word)) | Q(insurance_number=search_word))
        else:
            customer_qs = Company.objects.all().filter(
                Q(name1__icontains=search_word) | Q(name2__icontains=search_word) | Q(
                    abbreviation__icontains=search_word))

            base_filter = base_filter & (Q(license_plate__icontains=search_word
                                           ) | Q(
                searchtext__icontains=search_word) | Q(customer_id__in=[x.id for x in customer_qs]))

    else:
        base_filter = base_filter & (Q(connected_order=primary_order_id))
    return queryset.filter(base_filter)
