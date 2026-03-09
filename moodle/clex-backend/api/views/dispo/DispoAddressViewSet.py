import django_filters
from rest_framework import response, status, filters
from rest_framework.response import Response

from api.filters.DispoFilters import AddressFilter
from api.models.addresse import Address
from api.models.order import Order
from api.paginators import FasterPaginator
from api.serializers.dispo.DispoAddressSerializers import AddressSerializer, DispoAddressPatchSerializer, \
    DispoAddressCreateSerializer
from api.serializers.general.SLASerializers import SLAReportCreateSerializer
from api.services import create_sla_entry, set_order_edit_date
from api.views.dispo.DispoViewSet import DispoView




class DispoAddressViewSet(DispoView):
    queryset = Address.objects
    pagination_class = FasterPaginator
    filter_backends = [django_filters.rest_framework.DjangoFilterBackend, filters.OrderingFilter]
    filterset_class = AddressFilter
    serializer_class = AddressSerializer

    def list(self,request,**kwargs):
        qs = self.get_queryset()
        qs = self.filter_queryset(qs)
        page = self.paginate_queryset(qs)
        if page is not None:
            output_serializer = self.serializer_class(page, many=True)
            return self.get_paginated_response(output_serializer.data)

        output_serializer = self.serializer_class(qs, many=True)
        return response.Response(output_serializer.data)

    def retrieve(self, request, pk=None):
        qs = self.get_queryset().filter(order_id=pk)
        output_serializer = self.serializer_class(qs, many=True)
        return response.Response(output_serializer.data)

    def create(self, request, *args, **kwargs):
        serializer = DispoAddressCreateSerializer(request.data, data=request.data,context=request)
        serializer.is_valid(raise_exception=True)
        order_object = Order.objects.get(pk=serializer.validated_data["order_id"])
        create_sla_entry(object=order_object, user_id=request.user.mandate_user.id, activity_id=2,
                         serializer_class=SLAReportCreateSerializer)
        set_order_edit_date(object=order_object)
        serializer.create(serializer.validated_data)
        return Response(status=status.HTTP_201_CREATED)

    def patch(self, request, pk=None):
        address_object = Address.objects.get(pk=pk)
        serializer = DispoAddressPatchSerializer(address_object, data=request.data, partial=True,context=request)
        if serializer.is_valid(raise_exception=True):
            serializer.save()
            order_object = Order.objects.get(pk=address_object.order_id)
            set_order_edit_date(object=order_object)
            create_sla_entry(object=order_object, user_id=request.user.mandate_user.id, activity_id=2,
                             serializer_class=SLAReportCreateSerializer)
            return response.Response(status=status.HTTP_200_OK, data=request.data)