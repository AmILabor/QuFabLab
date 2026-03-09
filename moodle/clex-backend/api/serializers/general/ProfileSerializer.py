from django.contrib.auth.hashers import check_password
from django.contrib.auth.models import Group
from django.http import response
from rest_framework import serializers, status
from rest_framework.permissions import IsAuthenticated

from api.models.company import Company
from api.models.user import MandateUser, User
from api.serializers.admin.AdminUserSerializers import AdminUserSerializer, AdminMandateUserSerializer
from cleximages.serializers import FirmaSerializer
from utils.CustomModelRelatedField import CustomModelRelatedField
from utils.Exceptions import PasswordWrongException


class ProfileMandateUserSerializer(serializers.ModelSerializer):
    company = CustomModelRelatedField(model=Company, serializer=FirmaSerializer, target_field="id", source="company_id")

    class Meta:
        model = MandateUser
        fields = ("id","company","firstname","lastname","street","zip","city","country","birth_date","phone","mobil","fax","email","internet","post_office_box","expert_number","office_id","company_id","hidden","audanet_id","status","compensation_model","active")
        extra_kwargs = {
            'id': {'read_only': True}
        }

class ProfilePatchSerializer(AdminUserSerializer):
    mandate_user = AdminMandateUserSerializer()
    company__name1 = serializers.SerializerMethodField()
    passwordOld= serializers.CharField()
    class Meta:
        model = User
        fields = ("id", "username",
                  "company__name1", "mandate_user__active",
                  "mandate_user", "password","passwordOld")
        extra_kwargs = {
            'password': {'write_only': True, 'required': False},
            'id': {'read_only': True}

        }
    def get_permissions(self):
        return [IsAuthenticated()]

class ProfileSerializer(AdminUserSerializer):
    mandate_user = AdminMandateUserSerializer()
    mandate_user__firstname = serializers.SlugRelatedField(source="mandate_user", slug_field="firstname",
                                                           read_only=True)
    mandate_user__lastname = serializers.SlugRelatedField(source="mandate_user", slug_field="lastname", read_only=True)
    mandate_user__zip = serializers.SlugRelatedField(source="mandate_user", slug_field="zip", read_only=True)
    mandate_user__city = serializers.SlugRelatedField(source="mandate_user", slug_field="city", read_only=True)
    mandate_user__expert_number = serializers.SlugRelatedField(source="mandate_user", slug_field="expert_number",
                                                               read_only=True)
    mandate_user__hidden = serializers.SlugRelatedField(source="mandate_user", slug_field="hidden", read_only=True)
    mandate_user__status = serializers.SlugRelatedField(source="mandate_user", slug_field="status", read_only=True)
    mandate_user__active = serializers.SlugRelatedField(source="mandate_user", slug_field="active", read_only=True)
    company__name1 = serializers.SerializerMethodField()
    class Meta:
        model = User
        fields = ("id", "username", "mandate_user__firstname", "mandate_user__lastname"
                  , "mandate_user__zip", "mandate_user__city", "mandate_user__expert_number"
                  , "mandate_user__hidden", "mandate_user__status", "company__name1", "mandate_user__active",
                  "mandate_user")
        extra_kwargs = {
            'id': {'read_only': True}

        }
    def get_permissions(self):
        return [IsAuthenticated()]

    def update(self, instance, validated_data):
        print(validated_data["passwordOld"])
        result = self.context.check_password(validated_data["passwordOld"])
        if not result:
            raise PasswordWrongException()
        validated_data.pop("passwordOld")
        return super().update(instance,validated_data)