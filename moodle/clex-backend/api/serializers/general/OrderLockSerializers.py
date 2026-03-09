from rest_framework import serializers

from api.models.orderlock import OrderLock


class OrderLockSerializer(serializers.ModelSerializer):
    class Meta:
        model = OrderLock
        fields = ['order','user','timestamp']


class OrderLockModifySerializer(serializers.ModelSerializer):
    class Meta:
        model = OrderLock
        fields = ['order']