from django.db import IntegrityError
from django.db.models import Q
from django.http import Http404
from rest_framework import viewsets, response, status
from rest_framework.generics import get_object_or_404

from api.models.order import Order
from api.models.slareport import SLAReport, SLAsPerCustomer, SLAsPerOrderType
from api.serializers.general.SLASerializers import SLAReportSerializer, SLAReportParameterSerializer, \
    SLAReportCreateSerializer


class SLAReportViewSet(viewsets.GenericViewSet):
    queryset = SLAReport.objects.all()
    serializer_class = SLAReportSerializer

    def list(self,request,*args,**kwargs):
        qs = self.get_queryset()
        data = self.serializer_class(qs,many=True).data
        return response.Response(data)


    def create(self, request, *args, **kwargs):
        serializer = SLAReportParameterSerializer(data=request.data)
        serializer.is_valid(raise_exception=True)
        serialized_data = serializer.data
        user_id = request.user.mandate_user.id
        order_id = serialized_data["order_id"]
        activity_id = serialized_data["activity_id"]
        order = get_object_or_404(Order, pk=order_id)
        order_creation_date = order.creation_date
        sla_object_filter = Q(customer_id=order.customer_id) & Q(activity_id=activity_id)
        sla_objects = SLAsPerCustomer.objects.filter(sla_object_filter)
        if len(sla_objects)<1:
            sla_objects = SLAsPerOrderType.objects.filter(Q(order_type=order.order_type) & Q(activity_id=activity_id))
        if len(sla_objects)<1:
            return response.Response(status.HTTP_200_OK)
        sla_object = sla_objects[0]
        serialized_data["user_id"] = user_id
        serialized_data["sla_limit"] = sla_object.sla_time
        serialized_data["order_creation_date"] = order_creation_date
        creation_serializer = SLAReportCreateSerializer(data=serialized_data)
        is_serialized =  creation_serializer.is_valid()
        if is_serialized is True:
            try:
                creation_serializer.save()
            except IntegrityError as e:
                pass
        return response.Response(status.HTTP_201_CREATED)

    def retrieve(self,request,pk=None):
        try:
            qs = self.get_queryset().filter(order_id=pk)
        except:
            raise Http404
        output_serializer = self.serializer_class(qs,many=True)
        return response.Response(output_serializer.data)