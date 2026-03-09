import datetime

import django_filters
from django.utils import timezone
from rest_framework import filters, status, response
from rest_framework.response import Response

from api.models.order import Order
from api.permissions import IsExpertEmployeeUser, IsDispositionUser
from api.selectors import search_connectable_orders_freetext
from api.serializers.dispo.DispoConnectOrderSerializers import DispoConnectOrderListSerializer
from api.paginators import FasterPaginator
from api.views.dispo.DispoViewSet import DispoView


class DispoConnectOrderViewSet(DispoView):
    queryset = Order.objects
    pagination_class = FasterPaginator
    filter_backends = [django_filters.rest_framework.DjangoFilterBackend, filters.OrderingFilter]
    serializer_class = DispoConnectOrderListSerializer

    def get_permissions(self):
        permissions = [IsDispositionUser()+IsExpertEmployeeUser()]
        return permissions

    def get_queryset(self, pk, search_word):
        displayed_days = datetime.timedelta(days=365)
        current_time = timezone.now()  # .astimezone(to_tz)
        date_limit = current_time - displayed_days
        qs = search_connectable_orders_freetext(queryset=self.queryset,date_limit=date_limit,primary_order_id=pk,search_word=search_word)
        qs = qs.order_by("-connected_order",
                         "-creation_date").prefetch_related("responsible_user")
        return qs

    def list(self, request):
        search_word = request.query_params.get('q')
        pk = request.query_params.get('orderId')
        if pk == 'undefined':
            return Response(status=status.HTTP_400_BAD_REQUEST)
        qs = self.get_queryset(pk, search_word)
        page = self.paginate_queryset(qs)
        if page is not None:
            output_serializer = self.serializer_class(page, many=True)
            return self.get_paginated_response(output_serializer.data)
        output_serializer = self.serializer_class(qs, many=True)

        return response.Response(output_serializer.data)