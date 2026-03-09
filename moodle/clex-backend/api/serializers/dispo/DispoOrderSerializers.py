from rest_framework import serializers

from api.models.order import Order
from api.models.user import MandateUserView, MandateUser
from api.serializers.dispo.DispoAddressSerializers import AddressSerializer
from api.serializers.dispo.DispoAttachmentSerializers import AttachmentSerializer
from api.serializers.general.SLASerializers import SLAReportSerializer
from api.services import handle_order_history
from rest_framework.fields import CurrentUserDefault

class SVSerializer(serializers.ModelSerializer):
    class Meta:
        model = MandateUserView
        fields = ['id', 'firstname', 'lastname',"fullname"]


class DispoOrderListSerializer(serializers.ModelSerializer):
    customer_short = serializers.SlugRelatedField(source="customer_id", read_only=True, slug_field="name1")
    responsible_user_firstname = serializers.SlugRelatedField(source="responsible_user", read_only=True,
                                                              slug_field="firstname")
    responsible_user_lastname = serializers.SlugRelatedField(source="responsible_user", read_only=True,
                                                             slug_field="lastname")
    responsible_user = SVSerializer()
    inspection_address = AddressSerializer()
    status = serializers.SlugRelatedField(slug_field="name",read_only=True)
    address = AddressSerializer(many=True)

    class Meta:
        model = Order
        fields = ['id', 'creation_date', 'status', 'license_plate', 'customer_short', 'inspection_address'
            , 'customer', 'damage_number', 'order_type',
                  'responsible_user_firstname', 'responsible_user_lastname', 'sla_exceeded', 'address', 'responsible_user']


class DispoOrderSerializer(serializers.ModelSerializer):
    inspection_address = AddressSerializer()
    policyholder_address = AddressSerializer()
    ast_address = AddressSerializer()
    rf_address = AddressSerializer()
    attachments = AttachmentSerializer(many=True)
    expertEmployee = SVSerializer(source="expert_employee")
    responsible_user = SVSerializer()
    expert_backoffice = SVSerializer()
    customer = serializers.SlugRelatedField(source="customer_id", read_only=True, slug_field="name1")
    sla_report = SLAReportSerializer(many=True)

    class Meta:
        model = Order
        fields = ['id', 'customer_id', 'order_type', 'expertEmployee', 'expert_employee_id', 'expert_backoffice',
                  'expert_backoffice_id', 'annotation', 'annotation_internal', 'followup_date', 'edit_date',
                  'creation_date', 'appointment_date',
                  'appointment_time_start', 'appointment_time_end', 'collective_inspection', 'acquisition_expert',
                  'customer_desired_date', 'status',
                  'zip', "city", 'license_plate', 'inspection_address', 'day_of_damage', 'expert_employee',
                  'expert_backoffice',
                  'license_plate_opponent', 'insurance_number', 'damage_number', 'ordering_number', 'history',
                  'responsible_user', 'responsible_area', 'attachments', 'connected_order', 'customer', 'sla_report',
                  'policyholder_address',
                  'ast_address', 'rf_address', "lock"]


class DispoOrderPatchSerializer(serializers.ModelSerializer):
    expert_employee_id = serializers.PrimaryKeyRelatedField(queryset=MandateUser.objects.all(),allow_null=True)
    expert_backoffice_id = serializers.PrimaryKeyRelatedField(queryset=MandateUser.objects.all())

    class Meta:
        model = Order
        fields = ['id', 'customer_id', 'order_type', 'expert_employee_id', 'expert_backoffice_id', 'annotation',
                  'annotation_internal', 'followup_date', 'edit_date', 'creation_date', 'appointment_date',
                  'appointment_time_start', 'appointment_time_end', 'collective_inspection', 'acquisition_expert',
                  'customer_desired_date', 'status',
                  'zip', "city", 'license_plate', 'address', 'day_of_damage',
                  'license_plate_opponent', 'insurance_number', 'damage_number', 'ordering_number',
                  'responsible_user', 'responsible_area', 'connected_order']

    def save(self, **kwargs):
        new_data = self.validated_data
        if len(new_data.keys()) > 0:
            old_object = self.instance
            user = self.context.user.mandate_user
            handle_order_history(old_object,new_data,user)
        super().save()