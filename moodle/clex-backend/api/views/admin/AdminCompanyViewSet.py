import django_filters
from django.contrib.auth.models import Group
from django.db import transaction
from rest_framework import filters, response, status
from rest_framework.generics import get_object_or_404

from api.filters.AdminFilters import UsersFilter, WorkshopFilter, CompanyFilter
from api.models.company import Company
from api.models.user import User, MandateUser
from api.models.workshop import Workshop
from api.serializers.admin.AdminCompanySerializer import AdminCompanySerializer
from api.serializers.admin.AdminUserSerializers import GroupSerializer, AdminUserSerializer
from api.paginators import FasterPaginator
from api.serializers.general.WorkshopSerializer import WorkshopSerializer
from api.services import update_sales_ares_per_user_by_list, update_user_groups_by_list
from api.views.admin.AdminViewSet import AdminViewSet


class AdminCompanyViewSet(AdminViewSet):
    queryset = Company.objects
    pagination_class = FasterPaginator
    serializer_class = AdminCompanySerializer
    filter_backends = [django_filters.rest_framework.DjangoFilterBackend, filters.OrderingFilter]
    filterset_class = CompanyFilter
    ordering=["-active","name1"]

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
        object = get_object_or_404(self.get_queryset(),pk=pk)
        serializer = self.serializer_class(object,data=request.data,partial=True)
        if serializer.is_valid(raise_exception=True):
            serializer.save()
            return response.Response(status=status.HTTP_200_OK, data=request.data)

    def create(self, request, *args, **kwargs):
        serializer = self.serializer_class(data=request.data)
        serializer.is_valid(raise_exception=True)
        with transaction.atomic():
            instance = serializer.create(serializer.validated_data)

        return response.Response(status=status.HTTP_201_CREATED)

    def destroy(self, request, pk, *args, **kwargs):
        obj = get_object_or_404(self.queryset, pk=pk)
        obj.delete()
        return response.Response(status=status.HTTP_200_OK)