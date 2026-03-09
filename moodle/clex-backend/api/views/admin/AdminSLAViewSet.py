import django_filters
from django.db import IntegrityError
from django.db.models import ProtectedError
from rest_framework import filters, response, status
from rest_framework.exceptions import ParseError
from rest_framework.generics import get_object_or_404

from api.filters.AdminFilters import SLAsPerCustomerFilter, SLAsPerOrderTypeFilter, SLAActivityFilter
from api.models.slareport import SLAActivities, SLAsPerCustomer, SLAsPerOrderType
from api.serializers.admin.AdminSLASerializers import SLAPerOrderTypeSerializer, SLAsPerCustomerSerializer
from api.serializers.general.SLASerializers import SLAActivitySerializer
from api.paginators import FasterPaginator
from api.views.admin.AdminViewSet import AdminViewSet


class SLAActivitiesViewSet(AdminViewSet):
    queryset = SLAActivities.objects
    pagination_class = FasterPaginator
    serializer_class = SLAActivitySerializer
    filter_backends = [django_filters.rest_framework.DjangoFilterBackend, filters.OrderingFilter]
    filterset_class = SLAActivityFilter
    ordering=["activity"]
    ordering_fields = ["id","activity"]

    def list(self, request, *args, **kwargs):
        qs = self.get_queryset().all()
        qs = self.filter_queryset(qs)
        page = self.paginate_queryset(qs)
        if page is not None:
            output_serializer = self.serializer_class(page, many=True)
            return self.get_paginated_response(output_serializer.data)

        output_serializer = self.serializer_class(qs, many=True)
        return response.Response(output_serializer.data)

    def patch(self, request, pk=None):
        activity_object = get_object_or_404(self.get_queryset(),pk=pk)
        serializer = self.serializer_class(activity_object,data=request.data,partial=True)
        if serializer.is_valid(raise_exception=True):
            serializer.save()
            return response.Response(status=status.HTTP_200_OK, data=request.data)

    def create(self, request, *args, **kwargs):
        serializer = self.serializer_class(request.data, data=request.data)
        serializer.is_valid(raise_exception=True)
        serializer.create(serializer.validated_data)
        return response.Response(status=status.HTTP_201_CREATED)

    def destroy(self,request,pk,*args,**kwargs):
        obj = get_object_or_404(self.queryset,pk=pk)
        try:
            obj.delete()
        except (ProtectedError,IntegrityError):
            exc = ParseError(detail="Der SLA wird noch referenziert.")
            raise(exc)
        return response.Response(status=status.HTTP_200_OK)

class SLADefaultsViewSet(AdminViewSet):
    queryset = SLAsPerOrderType.objects.select_related("activity")
    pagination_class = FasterPaginator
    serializer_class = SLAPerOrderTypeSerializer
    filter_backends = [django_filters.rest_framework.DjangoFilterBackend, filters.OrderingFilter]
    filterset_class = SLAsPerOrderTypeFilter
    ordering=["order_type","sla_time"]
    ordering_fields=["activity__activity","order_type","sla_time"]

    def list(self, request, *args, **kwargs):
        qs = self.get_queryset().all()
        qs = self.filter_queryset(qs)
        page = self.paginate_queryset(qs)

        if page is not None:
            output_serializer = self.serializer_class(page, many=True)
            return self.get_paginated_response(output_serializer.data)

        output_serializer = self.serializer_class(qs, many=True)
        return response.Response(output_serializer.data)

    def patch(self, request, pk=None):
        activity_object = get_object_or_404(self.get_queryset(),pk=pk)
        serializer = self.serializer_class(activity_object,data=request.data,partial=True)
        if serializer.is_valid(raise_exception=True):
            try:
                serializer.save()
            except IntegrityError as e:
                exc = ParseError(
                    detail=f"Der Standard-SLA ({serializer.validated_data['activity'].activity}/{serializer.validated_data['order_type']}) besteht bereits")
                raise (exc)
            return response.Response(status=status.HTTP_200_OK, data=request.data)

    def create(self, request, *args, **kwargs):
        serializer = self.serializer_class(request.data, data=request.data)

        serializer.is_valid(raise_exception=True)
        try:
            serializer.create(serializer.validated_data)
        except IntegrityError as e:
            exc = ParseError(detail=f"Der Standard-SLA ({serializer.validated_data['activity'].activity}/{serializer.validated_data['order_type']}) besteht bereits")
            raise(exc)
        return response.Response(status=status.HTTP_201_CREATED)

    def destroy(self,request,pk,*args,**kwargs):
        obj = get_object_or_404(self.queryset,pk=pk)
        try:
            obj.delete()
        except (ProtectedError,IntegrityError):
            exc = ParseError(detail="Der SLA wird noch referenziert.")
            raise(exc)
        return response.Response(status=status.HTTP_200_OK)

class SLAActivitiesPerCustomerViewSet(AdminViewSet):
    queryset = SLAsPerCustomer.objects
    pagination_class = FasterPaginator
    serializer_class = SLAsPerCustomerSerializer
    filter_backends = [django_filters.rest_framework.DjangoFilterBackend, filters.OrderingFilter]
    filterset_class = SLAsPerCustomerFilter
    ordering=["order_type","sla_time"]
    ordering_fields= ["order_type","sla_time","activity__activity","customer__name1"]


    def list(self, request, *args, **kwargs):
        qs = self.get_queryset()
        qs = self.filter_queryset(qs)
        page = self.paginate_queryset(qs)
        if page is not None:
            output_serializer = self.serializer_class(page, many=True)
            return self.get_paginated_response(output_serializer.data)

        output_serializer = self.serializer_class(qs, many=True)
        return response.Response(output_serializer.data)

    def patch(self, request, pk=None):
        sla_object = get_object_or_404(self.get_queryset(),pk=pk)
        serializer = self.serializer_class(sla_object,data=request.data,partial=True)
        if serializer.is_valid(raise_exception=True):
            try:
                serializer.save()
            except IntegrityError as e:
                exc = ParseError(
                        detail=f"Der Kunden-SLA ({serializer.validated_data['activity'].activity}/{serializer.validated_data['order_type']}/{serializer.validated_data['customer'].name1}) besteht bereits")
                raise (exc)
            return response.Response(status=status.HTTP_200_OK, data=request.data)

    def create(self, request, *args, **kwargs):
        serializer = self.serializer_class(request.data, data=request.data)

        serializer.is_valid(raise_exception=True)
        try:
            serializer.create(serializer.validated_data)
        except IntegrityError as e:
            exc = ParseError(detail=f"Der Kunden-SLA ({serializer.validated_data['activity'].activity}/{serializer.validated_data['order_type']}/{serializer.validated_data['customer'].name1}) besteht bereits")
            raise(exc)
        return response.Response(status=status.HTTP_201_CREATED)

    def destroy(self,request,pk,*args,**kwargs):
        obj = get_object_or_404(self.queryset,pk=pk)
        try:
            obj.delete()
        except (ProtectedError,IntegrityError):
            exc = ParseError(detail="Der SLA wird noch referenziert.")
            raise(exc)
        return response.Response(status=status.HTTP_200_OK)