import django_filters
from django.db import IntegrityError
from django.db.models import ProtectedError
from rest_framework import response, status, filters
from rest_framework.exceptions import ParseError
from rest_framework.generics import get_object_or_404

from api.filters.AdminFilters import SalesAreasFilter, SalesAreasPerUserFilter
from api.models.salesareas import SalesAreas, SalesAreasPerUser
from api.paginators import FasterPaginator
from api.serializers.admin.AdminSalesAreasSerializers import SalesAreasSerializer, SalesAreasPerUserSerializer

from api.views.admin.AdminViewSet import AdminViewSet


class AdminSalesAreasViewSet(AdminViewSet):
    queryset = SalesAreas.objects.all()
    serializer_class = SalesAreasSerializer
    pagination_class = FasterPaginator
    filter_backends = [django_filters.rest_framework.DjangoFilterBackend, filters.OrderingFilter]
    filterset_class = SalesAreasFilter
    ordering=["zip_prefix"]
    ordering_fields = ['name','zip_prefix']


    def list(self,request,*args,**kwargs):
        qs = self.get_queryset()
        qs = self.filter_queryset(qs)
        page = self.paginate_queryset(qs)
        if page is not None:
            output_serializer = self.serializer_class(page, many=True)
            return self.get_paginated_response(output_serializer.data)
        data = self.serializer_class(qs,many=True).data
        return response.Response(data=data)

    def patch(self, request, pk=None):
        object = self.get_queryset().get(pk=pk)
        serialized = self.serializer_class(object,data=request.data,partial=True)
        if serialized.is_valid(raise_exception=True):
            serialized.save()
        return response.Response(status=status.HTTP_200_OK)

    def create(self,request,*args,**kwargs):
        serialized = self.serializer_class(data=request.data)
        if serialized.is_valid(raise_exception=True):
            try:
                serialized.create(serialized.validated_data)
            except IntegrityError as e:
                obj = self.get_queryset().get(zip_prefix=serialized.validated_data["zip_prefix"])
                msg = f'Der Eintrag mit dem PLZ-Prefix {obj.zip_prefix} besteht schon. ({obj.name})'
                exc = ParseError(detail=msg)
                raise exc
        return response.Response(status=status.HTTP_200_OK)

    def destroy(self,request,pk,*args,**kwargs):
        obj = get_object_or_404(self.queryset,pk=pk)
        try:
            obj.delete()
        except ProtectedError:
            exc = ParseError(detail="Das Vertriebsgebiet wird noch referenziert.")
            raise(exc)
        return response.Response(status=status.HTTP_200_OK)


class AdminSalesAreasPerUser(AdminViewSet):
    queryset = SalesAreasPerUser.objects.all()
    serializer_class = SalesAreasPerUserSerializer
    pagination_class = FasterPaginator
    filter_backends = [django_filters.rest_framework.DjangoFilterBackend, filters.OrderingFilter]
    filterset_class = SalesAreasPerUserFilter
    ordering=["user"]
    ordering_fields = ["salesarea__name","salesarea__zip_prefix","user__firstname","user__lastname"]

    def list(self,request,*args,**kwargs):
        qs = self.get_queryset()
        qs = self.filter_queryset(qs)
        page = self.paginate_queryset(qs)
        if page is not None:
            output_serializer = self.serializer_class(page, many=True)
            return self.get_paginated_response(output_serializer.data)
        data = self.serializer_class(qs,many=True).data
        return response.Response(data=data)

    def patch(self, request, pk=None):
        object = self.get_queryset().get(pk=pk)
        serialized = self.serializer_class(object, data=request.data, partial=True)
        if serialized.is_valid(raise_exception=True):
            try:
                serialized.save()
            except IntegrityError as e:
                firstname = serialized.validated_data["user"].firstname
                lastname = serialized.validated_data["user"].lastname
                salesArea = serialized.validated_data["salesarea"].name
                msg = f'Die Zuordnung besteht bereits {firstname} {lastname} / {salesArea}'
                exc = ParseError(detail=msg)
                raise exc
        return response.Response(status=status.HTTP_200_OK)

    def create(self, request, *args, **kwargs):
        serialized = self.serializer_class(data=request.data)
        if serialized.is_valid(raise_exception=True):
            try:
                serialized.create(serialized.validated_data)
            except IntegrityError as e:
                firstname = serialized.validated_data["user"].firstname
                lastname = serialized.validated_data["user"].lastname
                salesArea = serialized.validated_data["salesarea"].name
                msg = f'Die Zuordnung besteht bereits {firstname} {lastname} / {salesArea}'
                exc = ParseError(detail=msg)
                raise exc
        return response.Response(status=status.HTTP_200_OK)

    def destroy(self, request, pk, *args, **kwargs):
        obj = get_object_or_404(self.queryset, pk=pk)
        try:
            obj.delete()
        except ProtectedError:
            exc = ParseError(detail="Wird noch referenziert.")
            raise (exc)
        return response.Response(status=status.HTTP_200_OK)