from rest_framework import serializers

from api.models.order import Order


class OrderStatesSerializer(serializers.ModelSerializer):
    id = serializers.IntegerField(read_only=True)
    name = serializers.CharField(max_length=128,read_only=True)
    class Meta:
        model = Order
        fields=("name","id")
