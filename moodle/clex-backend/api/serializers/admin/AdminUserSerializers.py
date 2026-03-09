from django.contrib.auth.models import Group
from rest_framework import serializers
from rest_framework.exceptions import ValidationError

from api.models.company import Company
from api.models.user import MandateUser, User, MandateUserView
from cleximages.serializers import FirmaSerializer
from utils.CustomModelRelatedField import CustomModelRelatedField


class AdminMandateUserSerializer(serializers.ModelSerializer):
    company = CustomModelRelatedField(model=Company, serializer=FirmaSerializer, target_field="id", source="company_id")

    class Meta:
        model = MandateUser
        fields = ("id","company","firstname","lastname","street","zip","city","country","birth_date","phone","mobil","fax","email","internet","post_office_box","expert_number","office_id","company_id","hidden","audanet_id","status","compensation_model","active")
        extra_kwargs = {
            'id': {'read_only': True}
        }


class GroupSerializer(serializers.ModelSerializer):
    class Meta:
        model = Group
        fields= ('name','id')


class AdminUserSerializer(serializers.ModelSerializer):
    mandate_user = AdminMandateUserSerializer()
    mandate_user__firstname = serializers.SlugRelatedField(source="mandate_user",slug_field="firstname",read_only=True)
    mandate_user__lastname = serializers.SlugRelatedField(source="mandate_user",slug_field="lastname",read_only=True)
    mandate_user__zip = serializers.SlugRelatedField(source="mandate_user",slug_field="zip",read_only=True)
    mandate_user__city = serializers.SlugRelatedField(source="mandate_user",slug_field="city",read_only=True)
    mandate_user__expert_number = serializers.SlugRelatedField(source="mandate_user",slug_field="expert_number",read_only=True)
    mandate_user__hidden = serializers.SlugRelatedField(source="mandate_user",slug_field="hidden",read_only=True)
    mandate_user__status = serializers.SlugRelatedField(source="mandate_user",slug_field="status", read_only=True)
    mandate_user__active = serializers.SlugRelatedField(source="mandate_user",slug_field="active", read_only=True)
    company__name1 = serializers.SerializerMethodField()
    groups = GroupSerializer(many=True, required=False)
    class Meta:
        model = User
        fields = ("id","groups","username","mandate_user__firstname","mandate_user__lastname"
                  ,"mandate_user__zip","mandate_user__city","mandate_user__expert_number"
                  ,"mandate_user__hidden","mandate_user__status","company__name1","mandate_user__active","mandate_user","password")
        extra_kwargs = {
            'password': {'write_only': True,'required':False},
            'id': {'read_only':True}

        }


    def check_password_sanity(self,password):
        password = password.replace(" ","")
        if len(password)< 8:
            raise ValidationError("Das Passwort muss 8 Zeichen (Keine leerzeichen) oder mehr haben.")
        return True

    def create(self, validated_data):
        if "password" not in validated_data:
            raise ValidationError("Es muss beim erstellen ein Passwort angegeben werden.")
        mandate_user_data = validated_data.pop('mandate_user')
        mandate_user = MandateUser.objects.create(**mandate_user_data)
        internal_user = MandateUserView.objects.get(pk=mandate_user.id)
        validated_data["mandate_user"] = internal_user
        django_user = User.objects.create(**validated_data)
        if self.check_password_sanity(validated_data["password"]):
            django_user.set_password(validated_data["password"])
        django_user.save()
        return django_user

    def update(self, instance, validated_data):
        mandate_user_data = validated_data.pop('mandate_user')
        instance_user = instance.mandate_user
        mandate_user = MandateUser.objects.get(pk=instance_user.id)
        for k in mandate_user_data:
            setattr(mandate_user,k,mandate_user_data[k])
        mandate_user.save()
        for k in validated_data:
            setattr(instance,k,validated_data[k])
        if "password" in validated_data  and "password" != False:
            if self.check_password_sanity(validated_data["password"]):
                instance.set_password(validated_data["password"])
        instance.save()
        return instance

    def get_company__name1(self,obj):
        if not hasattr(obj,"mandate_user"):
            return None
        if not hasattr(obj.mandate_user,"company_id"):
            return None
        try:
            _company = Company.objects.get(id=obj.mandate_user.company_id)
        except Exception as e:
            print(e)
            return None

        return _company.name1