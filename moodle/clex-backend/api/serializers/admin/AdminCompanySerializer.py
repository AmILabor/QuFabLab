from rest_framework import serializers

from api.models.company import Company
from api.models.salesareas import SalesAreas, SalesAreasPerUser
from api.models.workshop import Workshop


class AdminCompanySerializer(serializers.ModelSerializer):
    id = serializers.IntegerField(read_only=True, required=False)
    creation_date = serializers.DateTimeField(required=False, read_only=True)
    class Meta:
        model = Company
        fields = ("__all__")