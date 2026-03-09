from rest_framework import serializers

from api.models.order import Order
from api.serializers.dispo.DispoAddressSerializers import AddressSerializer


class DispoConnectOrderListSerializer(serializers.ModelSerializer):
    customer_short = serializers.SlugRelatedField(source="customer_id", read_only=True, slug_field="name1")
    responsible_user_firstname = serializers.SlugRelatedField(source="responsible_user", read_only=True,
                                                              slug_field="firstname")
    responsible_user_lastname = serializers.SlugRelatedField(source="responsible_user", read_only=True,
                                                             slug_field="lastname")
    inspection_address = AddressSerializer()

    class Meta:
        model = Order
        fields = ['id', 'creation_date', 'status', 'license_plate', 'customer_short', 'inspection_address'
            , 'customer', 'damage_number', 'order_type', 'inspection_address',
                  'responsible_user_firstname', 'responsible_user_lastname', 'sla_exceeded', "connected_order"]