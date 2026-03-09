from rest_framework import viewsets, response, status
from rest_framework.generics import get_object_or_404
from rest_framework.permissions import IsAuthenticated

from api.models.orderlock import OrderLock
from api.serializers.general.OrderLockSerializers import OrderLockSerializer, OrderLockModifySerializer
from api.services import generate_locked_error_message


class OrderLockViewSet(viewsets.GenericViewSet):
    queryset = OrderLock.objects.all()
    serializer_class = OrderLockSerializer
    permission_classes = [IsAuthenticated]

    def list(self,request,*args,**kwargs):
        qs = self.get_queryset()
        data = self.serializer_class(qs,many=True).data
        return data

    def create(self,request,*args,**kwargs):
        serialized = OrderLockModifySerializer(data=request.data)
        error_code = None
        try:
            serialized.is_valid(raise_exception=True)
        except Exception as e:
            error_code = e.get_codes()["order"][0]
            if error_code=="unique":
                locked_object = self.queryset.get(order=serialized["order"].value)
                if locked_object.user.id == request.user.mandate_user_id:
                    return response.Response(status.HTTP_200_OK)
                generate_locked_error_message(locked_object)
                return
            raise e

        user_id = request.user.mandate_user_id
        serialized_data = serialized.data
        serialized_data['user']=user_id
        serializer = self.serializer_class(OrderLock(), data=serialized_data)
        serializer.is_valid(raise_exception=True)
        serializer.save()
        return response.Response(status.HTTP_200_OK)

    def destroy(self,request,pk,*args,**kwargs):
        locked_object = get_object_or_404(self.queryset,pk=pk)
        user_id = request.user.mandate_user_id
        if locked_object.user_id != user_id:
            locked_by = locked_object.user
            locked_by_id = locked_by.id
            locked_since = locked_object.timestamp
            locked_by_name = f"{locked_by.firstname} {locked_by.lastname}"
            msg = {'lockedByName': locked_by_name, ' lockedById': locked_by_id, 'since': locked_since}
            return response.Response(status=status.HTTP_403_FORBIDDEN, data={'detail': msg})
        locked_object.delete()
        return response.Response(status=status.HTTP_200_OK)