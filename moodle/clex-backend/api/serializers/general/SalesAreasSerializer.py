from rest_framework import serializers

from api.models.salesareas import SalesAreas, SalesAreasPerUser


class SalesAreasSerializer(serializers.ModelSerializer):
    class Meta:
        model = SalesAreas
        fields = ("id","zip_prefix","name")


class SalesAreasPerUserSerializer(serializers.ModelSerializer):
    salesarea_id = serializers.SlugRelatedField(source="salesarea",slug_field="id", read_only=True)
    zip_prefix = serializers.SlugRelatedField(source="salesarea",slug_field="zip_prefix", read_only=True)
    name = serializers.SlugRelatedField(source="salesarea",slug_field="name", read_only=True)
    class Meta:
        model = SalesAreasPerUser
        fields = ("id","salesarea_id","zip_prefix","name","user")