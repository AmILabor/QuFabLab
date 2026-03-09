from rest_framework import serializers

from api.models.slareport import SLAsPerCustomer, SLAActivities, SLAReport


class SLAsPerCustomerSerializer(serializers.ModelSerializer):
    activity_name = serializers.SlugRelatedField(source='activity',read_only=True,slug_field="activity")
    customer_bezeichnung = serializers.SlugRelatedField(source='customer',read_only=True,slug_field="name2")
    class Meta:
        model = SLAsPerCustomer
        fields = ['id','activity','activity_name','sla_time','customer_bezeichnung','order_type']


class SLAActivitySerializer(serializers.ModelSerializer):
    class Meta:
        model = SLAActivities
        fields = ['id','activity']


class SLAReportSerializer(serializers.ModelSerializer):
    activity_name = serializers.SlugRelatedField(source='activity',read_only=True,slug_field="activity")
    user_firstname = serializers.SlugRelatedField(source='user',read_only=True,slug_field="firstname")
    user_lastname = serializers.SlugRelatedField(source='user',read_only=True,slug_field="lastname")
    class Meta:
        model = SLAReport
        fields = ['order_id','timestamp','activity_id','user_id','activity_name','user_firstname','user_lastname']


class SLAReportParameterSerializer(serializers.ModelSerializer):
    order_id = serializers.IntegerField(required=True)
    activity_id = serializers.IntegerField(required=True)
    class Meta:
        model = SLAReport
        fields = ['order_id','activity_id']


class SLAReportCreateSerializer(serializers.ModelSerializer):
    order_id = serializers.IntegerField(required=True)
    activity_id = serializers.IntegerField(required=True)
    user_id = serializers.IntegerField(required=True)
    class Meta:
        model = SLAReport
        fields = ['order_id','activity_id','user_id','order_creation_date','sla_limit']