from rest_framework import response
from rest_framework.generics import get_object_or_404

from api.models.order import Order
from api.serializers.dispo.DispoOrderSerializers import DispoOrderSerializer
from api.views.dispo.DispoViewSet import DispoView


class DispoOrderHistoryViewSet(DispoView):
    queryset = Order.objects
    serializer_class = DispoOrderSerializer

    def retrieve(self, request, pk=None):
        qs = get_object_or_404(self.get_queryset(), pk=pk)
        output_serializer = self.serializer_class(qs)
        return response.Response(output_serializer.data)