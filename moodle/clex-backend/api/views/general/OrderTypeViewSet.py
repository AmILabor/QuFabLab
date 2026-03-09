from rest_framework import viewsets, response

from api.models.order import Order
from api.serializers.general.OrderTypeSerializers import OrderTypeSerializer


class OrderTypeViewSet(viewsets.GenericViewSet):

    queryset = Order.objects.values('order_type').distinct()

    def list(self, request, *args, **kwargs):
        qs = self.get_queryset()
        output_serializer = OrderTypeSerializer(qs, many=True)
        return response.Response(output_serializer.data)