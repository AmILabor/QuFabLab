from rest_framework import serializers

from api.models.slareport import SLAsPerOrderType, SLAsPerCustomer


class SLAPerOrderTypeSerializer(serializers.ModelSerializer):
    activity__activity = serializers.SlugRelatedField(source='activity',read_only=True,slug_field="activity")
    class Meta:
        model = SLAsPerOrderType
        fields = ['id','activity','activity__activity','sla_time','order_type']


class SLAsPerCustomerSerializer(serializers.ModelSerializer):
    activity__activity = serializers.SlugRelatedField(source='activity',read_only=True,slug_field="activity")
    customer__name1 = serializers.SlugRelatedField(source='customer',read_only=True,slug_field="name1")
    class Meta:
        model = SLAsPerCustomer
        fields = ['id','activity','activity__activity','sla_time','customer__name1','order_type','customer']