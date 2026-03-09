from rest_framework import viewsets, response, status
from api.models.orderstates import OrderStates
from api.serializers.general.OrderStatesSerializers import OrderStatesSerializer


class OrderStatusViewSet(viewsets.GenericViewSet):
    queryset = OrderStates.objects.all()
    serializer_class = OrderStatesSerializer

    def list(self,request,*args,**kwargs):
        qs = self.get_queryset()
        serialized = self.get_serializer(qs,many=True)
        return response.Response(status=status.HTTP_200_OK,data=serialized.data)