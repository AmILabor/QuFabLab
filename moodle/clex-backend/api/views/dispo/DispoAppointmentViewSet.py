import django_filters
from django.db.models import Q
from rest_framework import filters, response

from api.filters.DispoFilters import DispoAppointmentFilter, TerminFilter
from api.models.order import Order
from api.models.appointment import Appointment
from api.serializers.dispo.DispoAppointmentSerializers import DispoOrderAppointmentSerializer, \
    DispoCalendarSerializer
from api.views.dispo.DispoViewSet import DispoView


class OrderAppointmentViewSet(DispoView):
    queryset = Order.objects
    filter_backends = [django_filters.rest_framework.DjangoFilterBackend, filters.OrderingFilter]
    filterset_class = DispoAppointmentFilter
    serializer_class = DispoOrderAppointmentSerializer
    ordering = ["tour_id","expert_employee__lastname","appointment_date","appointment_time_start","tour_id"]

    def get_queryset(self, query_params):
        qs = self.queryset.filter(~Q(hidden=1) & Q(connected_order__isnull=True))
        if "expert_employee_id" in query_params:
            expert_employee_ids = [x for x in query_params["expert_employee_id"].split(",") if x.isdigit()]
            return qs.filter(expert_employee_id__in=expert_employee_ids)
        return qs

    def list(self, request):
        qs = self.get_queryset(request.query_params)
        qs = self.filter_queryset(qs)
        output_serializer = self.serializer_class(qs, many=True)
        return response.Response(output_serializer.data)

class AppointmentViewSet(DispoView):
    queryset = Appointment.objects
    filter_backends = [django_filters.rest_framework.DjangoFilterBackend, filters.OrderingFilter]
    filterset_class = TerminFilter
    serializer_class = DispoCalendarSerializer

    def get_queryset(self, query_params):
        qs = self.queryset
        if "expert_employee_id" in query_params:
            expert_employee_ids = [x for x in query_params["expert_employee_id"].split(",") if x.isdigit()]
            return qs.filter(expert_employee_id__in=expert_employee_ids)
        return qs

    def list(self, request):
        qs = self.get_queryset(request.query_params)
        qs = self.filter_queryset(qs)
        output_serializer = self.serializer_class(qs, many=True)
        return response.Response(output_serializer.data)