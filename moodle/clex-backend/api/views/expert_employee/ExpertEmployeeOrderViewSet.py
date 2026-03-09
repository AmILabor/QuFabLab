import django_filters
from rest_framework import filters, response, status
from rest_framework.generics import get_object_or_404

from api.filters.ExpertEmployeeFilters import ExpertEmployeeOrderFilter
from api.models.order import Order
from api.paginators import FasterPaginator
from api.serializers.expert_employee.ExpertEmployeeOrderSerializers import ExpertEmployeeOrderSerializer
from api.views.expert_employee.ExpertEmployeeViewSet import ExpertEmployeeViewSet


class ExpertEmployeeOrderViewSet(ExpertEmployeeViewSet):
    queryset = Order.objects.all()
    serializer_class = ExpertEmployeeOrderSerializer
    pagination_class = FasterPaginator
    filter_backends = [django_filters.rest_framework.DjangoFilterBackend, filters.OrderingFilter]
    filterset_class = ExpertEmployeeOrderFilter

    def get_queryset(self):
        qs = super().get_queryset()
        qs = qs.order_by('-creation_date')
        return qs

    def retrieve(self, request, pk=None):
        qs = self.get_queryset()#.filter(expert_employee_id__in=[0,None,request.user.mandate_user_id])
        qs = get_object_or_404(qs, pk=pk)
        output_serializer = self.serializer_class(qs)
        return response.Response(output_serializer.data)

    def list(self,request, *args,**kwargs):
        qs = self.get_queryset().filter(expert_employee_id=request.user.mandate_user_id)
        qs = self.filter_queryset(qs)
        page = self.paginate_queryset(qs)
        if page is not None:
            output_serializer = self.serializer_class(page, many=True)
            return self.get_paginated_response(output_serializer.data)
        output_serializer = self.serializer_class(qs, many=True)
        return response.Response(output_serializer.data)
