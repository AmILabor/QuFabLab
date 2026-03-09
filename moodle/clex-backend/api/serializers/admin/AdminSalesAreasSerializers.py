from rest_framework import serializers

from api.models.salesareas import SalesAreas, SalesAreasPerUser


class SalesAreasSerializer(serializers.ModelSerializer):
    class Meta:
        model = SalesAreas
        fields = ("id","zip_prefix","name")


class SalesAreasPerUserSerializer(serializers.ModelSerializer):
    salesarea_id = serializers.SlugRelatedField(source="salesarea",slug_field="id", read_only=True)
    salesarea__zip_prefix = serializers.SlugRelatedField(source="salesarea",slug_field="zip_prefix", read_only=True)
    salesarea__name = serializers.SlugRelatedField(source="salesarea",slug_field="name", read_only=True)
    user__firstname = serializers.SlugRelatedField(source="user",slug_field="firstname", read_only=True)
    user__lastname= serializers.SlugRelatedField(source="user",slug_field="lastname", read_only=True)
    class Meta:
        model = SalesAreasPerUser
        fields = ("id","salesarea__zip_prefix","salesarea","salesarea__name","user__firstname","user__lastname","salesarea_id","user")