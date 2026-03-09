from django.contrib.auth.models import Group
from rest_framework import serializers

from api.models.user import MandateUserView, MandateUser, User
from api.serializers.general.SalesAreasSerializer import SalesAreasPerUserSerializer


class GroupSerializer(serializers.ModelSerializer):
    class Meta:
        model= Group
        fields = ('id','name')


class MandateUserViewSerializer(serializers.ModelSerializer):
    class Meta:
        model = MandateUserView
        fields = ['firstname','lastname']


class MandateUserSerializer(serializers.ModelSerializer):
    salesareas = SalesAreasPerUserSerializer(many=True)
    company_name = serializers.SlugRelatedField(source="company",slug_field="name1",read_only=True)
    class Meta:
        model= MandateUser
        fields = ("id","firstname","lastname","salesareas","company_name","zip","city","street","expert_number")


class UserSerializer(serializers.ModelSerializer):
    groups = GroupSerializer(many=True)
    userdata = MandateUserViewSerializer(source="mandate_user")
    class Meta:
        model = User
        fields = ('username','groups','userdata')