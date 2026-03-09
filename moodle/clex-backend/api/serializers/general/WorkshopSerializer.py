from rest_framework import serializers

from api.models.salesareas import SalesAreas, SalesAreasPerUser
from api.models.workshop import Workshop


class WorkshopSerializer(serializers.ModelSerializer):
    creation_date = serializers.DateTimeField(required=False)
    edit_date = serializers.DateTimeField(required=False)
    class Meta:
        model = Workshop
        fields = ("__all__")