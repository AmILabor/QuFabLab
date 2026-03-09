from rest_framework import serializers

from api.models.order import Order


class OrderTypeSerializer(serializers.ModelSerializer):
    name = serializers.CharField(source="order_type")
    class Meta:
        model = Order
        fields=("name",)