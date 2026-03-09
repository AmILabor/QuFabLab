from rest_framework import serializers
from rest_framework.permissions import IsAuthenticated

from api.models import *
from api.models.addresse import Address
from api.models.company import Company
from api.models.user import MandateUserView
from cleximages.models import *

class ClexImageRetrievalSerializer(serializers.ModelSerializer):
    class Meta:
        model=ClexImageUser
        fields=['id','name','expert_employee_id']

class APITokenSerializer(serializers.Serializer):
    token = serializers.SerializerMethodField()

    def get_token(self, obj):
        return f'Token {obj.key}'

class UserSerializer(serializers.ModelSerializer):
    permission_classes = [IsAuthenticated]
    class Meta:
        model=MandateUserView
        fields= ['id','firstnamen','lastnamen']

class AddressSerializer(serializers.ModelSerializer):
    class Meta:
        model = Address
        fields = ['id','company','name','name2','street_address','zip',"city",'spot','row','parking_space']

class AuftragListSerializer(serializers.ModelSerializer):
    address = AddressSerializer(many=True,read_only=True)
    class Meta:
        model = Order
        fields = ['id', 'address', 'appointment_date', 'customer_id', 'appointment_time_start', 'appointment_time_end', 'status', 'zip', "city",'street', 'license_plate', 'expert_employee_id', 'expert_backoffice_id','make','typ']
        #fields = ['id', 'creation_date', 'appointment_date', 'termin_uhrzeit', 'appointment_time_end', 'status', 'zip', "city", 'license_plate', 'expert_employee_id', 'expert_backoffice_id', 'responsible_user', 'responsible_area', 'customer_id', 'order_type']

class ClexImageSerializerPost(serializers.ModelSerializer):
    class Meta:
        model = AuftragBild
        fields = ['id','order_id','user_id','upload_datetime','image']

class ClexImageSerializer(serializers.ModelSerializer):
    class Meta:
        model = AuftragBild
        fields = ['id','order_id','user_id','upload_datetime','image','image_hash']

class FirmaSerializer(serializers.ModelSerializer):
    class Meta:
        model = Company
        fields = ['id', 'name1', 'name2', 'street', 'zip', "city", 'country', 'phone']
        #fields = ['id', 'creation_date', 'appointment_date', 'termin_uhrzeit', 'appointment_time_end', 'status', 'zip', "city", 'license_plate', 'expert_employee_id', 'expert_backoffice_id', 'responsible_user', 'responsible_area', 'customer_id', 'order_type']

