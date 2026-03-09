import django_filters
from rest_framework import response, status, filters
from rest_framework.generics import get_object_or_404

from api.filters.AdminFilters import OrderLockFilter
from api.models.orderlock import OrderLock
from api.paginators import FasterPaginator
from api.serializers.admin.AdminOrderLockSerializer import OrderLockSerializer

from api.views.admin.AdminViewSet import AdminViewSet


class AdminOrderLockViewSet(AdminViewSet):
    queryset = OrderLock.objects.all()
    serializer_class = OrderLockSerializer
    pagination_class = FasterPaginator
    filter_backends = [django_filters.rest_framework.DjangoFilterBackend, filters.OrderingFilter]
    filterset_class = OrderLockFilter
    ordering=["timestamp"]
    ordering_fields= ["timestamp","order_id","user__firstname","user__lastname"]

    def list(self,request,*args,**kwargs):
        qs = self.get_queryset()
        qs = self.filter_queryset(qs)
        page = self.paginate_queryset(qs)
        if page is not None:
            output_serializer = self.serializer_class(page, many=True)
            return self.get_paginated_response(output_serializer.data)
        data = self.serializer_class(qs,many=True).data
        return response.Response(data=data)


    def destroy(self,request,pk,*args,**kwargs):
        locked_object = get_object_or_404(self.queryset,pk=pk)
        locked_object.delete()
        return response.Response(status=status.HTTP_200_OK)

