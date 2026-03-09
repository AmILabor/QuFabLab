from rest_framework import serializers

from api.models.company import Company


class CustomerSerializer(serializers.ModelSerializer):
    class Meta:
        model = Company
        fields = ("id","name1","name2","street","zip","city","phone","fax","internet","email","abbreviation")