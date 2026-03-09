from django.db.models import Q, Subquery, OuterRef
from api.models.addresse import Address
from django.utils.timezone import localtime
import datetime
from django.db.models import QuerySet

from api.models.order import Order


def select_orders_by_plate(*, license_plate: str) -> QuerySet:
    return Order.objects.filter(Q(license_plate__iexact=license_plate))


def select_orders_for_today(*, expert_employee_id) -> QuerySet:
    today_date = "2012-06-01 00:00"  # localtime().date()
    today_date = datetime.datetime.strptime(today_date, "%Y-%m-%d %H:%M")
    tomorrow_date = today_date + datetime.timedelta(days=1)
    qs = Order.objects.filter(Q(appointment_date__gte=today_date), Q(appointment_date__lt=tomorrow_date), Q(expert_employee_id=expert_employee_id))
    return qs


def select_address_by_order(*, order_id) -> QuerySet:
    return Address.objects.filter(Q(order_id=order_id))

