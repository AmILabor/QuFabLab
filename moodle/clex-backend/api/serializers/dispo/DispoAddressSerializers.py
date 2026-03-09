from rest_framework import serializers

from api.models.addresse import Address
from api.models.order import Order
from api.services import handle_order_history, set_order_history
from utils.OrderHistoryActions import OrderHistoryActionEnum


class AddressSerializer(serializers.ModelSerializer):
    class Meta:
        model = Address
        fields = ['id', 'company', 'name', 'name2', 'street_address', 'zip', "city", 'spot', 'role', 'row',
                  'parking_space', 'phone', 'mobile', 'email']


class DispoAddressPatchSerializer(serializers.ModelSerializer):
    class Meta:
        model = Address
        fields = ["company", "name", "name2", "street_address", "zip", "city", "phone", "mobile", "email", "internet",
                  "spot", "row", "parking_space"]

    def save(self, **kwargs):
        try:
            super().save(**kwargs)
        except Exception as e:
            print(e)
            raise(e)
        old_object = self.instance
        order_object = old_object.order
        user = self.context.user.mandate_user
        set_order_history(order_object=order_object, user=user,
                          order_history_action=OrderHistoryActionEnum.ORDER_SAVED)


class DispoAddressCreateSerializer(serializers.ModelSerializer):
    order_id = serializers.IntegerField()

    class Meta:
        model = Address
        fields = ["order_id",
                  "company", "name", "name2", "street_address", "zip", "city", "phone", "mobile",
                  "email", "internet", "spot", "row", "parking_space", "role"]

    def create(self, validated_data):
        order_id = validated_data["order_id"]
        order_object = Order.objects.get(pk=order_id)
        user = self.context.user.mandate_user
        set_order_history(order_object=order_object, user=user,
                          order_history_action=OrderHistoryActionEnum.ORDER_SAVED)
        return super().create(validated_data)
