from rest_framework import serializers

from api.models.orderlock import OrderLock


class OrderLockSerializer(serializers.ModelSerializer):
    user__firstname = serializers.SlugRelatedField(source='user', read_only=True, slug_field="firstname")
    user__lastname = serializers.SlugRelatedField(source='user', read_only=True, slug_field="lastname")
    class Meta:
        model = OrderLock
        fields = ['order_id','user','user__firstname','user__lastname','timestamp']