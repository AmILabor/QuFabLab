import datetime

import django_filters
from django.core.exceptions import ObjectDoesNotExist
from django.db.models import Q
from django.utils import timezone
from rest_framework import filters, response, status
from rest_framework.generics import get_object_or_404

from api.filters.DispoFilters import DispoOrderFilter
from api.models.order import Order
from api.models.orderlock import OrderLock
from api.serializers.dispo.DispoOrderSerializers import DispoOrderListSerializer, DispoOrderSerializer, \
    DispoOrderPatchSerializer
from api.serializers.general.SLASerializers import SLAReportCreateSerializer
from api.services import create_sla_entry, generate_locked_error_message, set_order_history
from api.paginators import FasterPaginator
from api.views.dispo.DispoViewSet import DispoView
from utils.OrderHistoryActions import OrderHistoryActionEnum


class DispoAuftragViewSet(DispoView):
    queryset = Order.objects
    pagination_class = FasterPaginator
    serializer_class = DispoOrderListSerializer
    filter_backends = [django_filters.rest_framework.DjangoFilterBackend, filters.OrderingFilter]
    filterset_class = DispoOrderFilter

    def get_list_queryset(self, custom_filter_present=False, newNoAppointment=True):
        base_query = (Q(expert_employee_id=0) | Q(expert_employee_id__isnull=True))

        if newNoAppointment:
            base_query = (base_query | Q(appointment_date__isnull=True)) | Q(status=0)
        if not custom_filter_present:
            # TODO: Default display days to config
            displayed_days = datetime.timedelta(days=365)
            current_time = timezone.now()  # .astimezone(to_tz)
            date_limit = current_time - displayed_days
            base_query = base_query & Q(
                creation_date__gt=date_limit)
        return self.queryset.filter(base_query).order_by(
                "-creation_date").prefetch_related("expert_backoffice","responsible_user","expert_employee").distinct()

    def list(self, request, *args, **kwargs):
        newNoAppointment = True
        filter_present = False
        if "newNoAppointment" in request.query_params and request.query_params["newNoAppointment"] == "false":
            newNoAppointment = False
        if "startDate" in request.query_params:
            filter_present = True
        qs = self.get_list_queryset(custom_filter_present=filter_present, newNoAppointment=newNoAppointment)
        qs = self.filter_queryset(qs)
        page = self.paginate_queryset(qs)
        if page is not None:
            output_serializer = DispoOrderListSerializer(page, many=True)
            return self.get_paginated_response(output_serializer.data)

        output_serializer = DispoOrderListSerializer(qs, many=True)
        return response.Response(output_serializer.data)


    def retrieve(self, request, pk=None):
        qs = get_object_or_404(self.get_queryset(), pk=pk)
        output_serializer = DispoOrderSerializer(qs)
        create_sla_entry(object=qs,user_id = request.user.mandate_user.id,activity_id=1,serializer_class=SLAReportCreateSerializer)
        if request.query_params.get('noHistory',False) is False:
            set_order_history(order_object=output_serializer.instance, user=request.user.mandate_user,
                              order_history_action=OrderHistoryActionEnum.ORDER_VIEWED)
        return response.Response(output_serializer.data)

    def patch(self, request, pk=None):
        # Check if the order is currently locked by requests user if not return 400 with locked message
        order_object = Order.objects.get(pk=pk)
        lock_object = OrderLock.objects.filter(order_id=order_object.id)
        if len(lock_object)>0:
            if lock_object[0].user.id != request.user.mandate_user_id:
                generate_locked_error_message(lock_object[0])
                return
        serializer = DispoOrderPatchSerializer(order_object, data=request.data, partial=True,context=request)
        if serializer.is_valid(raise_exception=True):
            create_sla_entry(object=order_object, user_id=request.user.mandate_user.id, activity_id=2,serializer_class=SLAReportCreateSerializer)
            try:
                expert_employee_id = order_object.expert_employee_id
            except ObjectDoesNotExist as e:
                expert_employee_id=False
            if not expert_employee_id and "expert_employee_id" in serializer.validated_data:
                create_sla_entry(object=order_object, user_id=request.user.mandate_user.id, activity_id=4,serializer_class=SLAReportCreateSerializer)
            serializer.save()
            if order_object.appointment_date and order_object.appointment_time_start and order_object.appointment_time_end:
                create_sla_entry(object=order_object, user_id=request.user.mandate_user.id, activity_id=5,serializer_class=SLAReportCreateSerializer)
            return response.Response(status=status.HTTP_200_OK, data=request.data)