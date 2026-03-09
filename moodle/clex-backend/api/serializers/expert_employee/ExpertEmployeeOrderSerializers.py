from rest_framework import serializers

from api.models.order import Order
from api.models.user import MandateUserView, MandateUser
from api.serializers.dispo.DispoAddressSerializers import AddressSerializer
from api.serializers.dispo.DispoAttachmentSerializers import AttachmentSerializer
from api.serializers.dispo.DispoOrderSerializers import SVSerializer
from api.serializers.general.SLASerializers import SLAReportSerializer
from api.services import handle_order_history
from rest_framework.fields import CurrentUserDefault

class ExpertEmployeeOrderListSerializer(serializers.ModelSerializer):
    customer_short = serializers.SlugRelatedField(source="customer_id", read_only=True, slug_field="name1")
    responsible_user_firstname = serializers.SlugRelatedField(source="responsible_user", read_only=True,
                                                              slug_field="firstname")
    responsible_user_lastname = serializers.SlugRelatedField(source="responsible_user", read_only=True,
                                                             slug_field="lastname")
    inspection_address = AddressSerializer()
    address = AddressSerializer(many=True)

    class Meta:
        model = Order
        fields = ['id', 'creation_date', 'status', 'license_plate', 'customer_short', 'inspection_address'
            , 'customer', 'damage_number', 'order_type',
                  'responsible_user_firstname', 'responsible_user_lastname', 'sla_exceeded', 'address']


class ExpertEmployeeOrderSerializer(serializers.ModelSerializer):
    inspection_address = AddressSerializer()
    policyholder_address = AddressSerializer()
    ast_address = AddressSerializer()
    rf_address = AddressSerializer()
    attachments = AttachmentSerializer(many=True)
    expertEmployee = SVSerializer(source="expert_employee")
    expert_backoffice = SVSerializer()
    responsible_user = SVSerializer()
    status = serializers.SlugRelatedField(slug_field="name",read_only=True)
    customer = serializers.SlugRelatedField(source="customer_id", read_only=True, slug_field="name1")

    class Meta:
        model = Order
        fields = ['licensee_id','hidden','is_nfz','order_number_af','appointment_time_end', 'insurance_number', 'policyholder_address', 'customer_id', 'edit_date','customer',
                  'creation_date', 'customer_desired_date', 'old_damage', 'excess_tk', 'contract_number', 'type', 'city',
                  'excess_vk', 'day_of_damage', 'collective_inspection', 'invoice_amount', 'followup_date', 'utype',
                  'acquisition_expert', 'order_type', 'vehicle_type_id', 'damage_number', 'next_hu',
                  'expert_backoffice_id', 'zip', 'ride_fee', 'ast_address', 'annotation_internal', 'hsn',
                  'displacement', 'license_plate_opponent', 'power', 'vehicle_registration', 'engine_type_id',
                  'pre_damage', 'vvs', 'kba_id', 'body_type_id', 'expert_employee_id', 'id', 'status',
                  'vehicle_registration_last', 'connected_order','damage_area', 'appointment_time_start', 'license_plate',
                  'responsible_user', 'make', 'tsn', 'further_fee', 'ordering_number', 'rf_address', 'base_fee','expertEmployee',
                  'appointment_date', 'responsible_area', 'picture_fee', 'address', 'annotation','inspection_address','attachments','expert_backoffice']


class ExpertEmployeeOrderPatchSerializer(serializers.ModelSerializer):
    expert_employee_id = serializers.PrimaryKeyRelatedField(queryset=MandateUser.objects.all())
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
            handle_order_history(old_object, new_data, user)
        super().save()