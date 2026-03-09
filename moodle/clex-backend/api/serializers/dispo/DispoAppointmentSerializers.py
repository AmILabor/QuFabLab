from rest_framework import serializers

from api.models.appointment import Appointment
from api.models.order import Order
from api.serializers.dispo.DispoAddressSerializers import AddressSerializer


class DispoOrderAppointmentSerializer(serializers.ModelSerializer):
    customer_short = serializers.SlugRelatedField(source="customer_id", read_only=True, slug_field="name1")
    expert_employee_firstname = serializers.SlugRelatedField(source="expert_employee", read_only=True,
                                                             slug_field="firstname")
    expert_employee_lastname = serializers.SlugRelatedField(source="expert_employee", read_only=True,
                                                            slug_field="lastname")
    inspection_address = AddressSerializer()
    class Meta:
        model = Order
        fields = ['id', 'creation_date', 'customer_short', 'inspection_address', 'appointment_date',
                  "appointment_time_start", "appointment_time_end"
            , 'customer', 'damage_number', 'order_type', 'inspection_address', "expert_employee_id",
                  "expert_employee_firstname", "expert_employee_lastname","tour_id"]


class DispoCalendarSerializer(serializers.ModelSerializer):
    expert_employee_firstname = serializers.SlugRelatedField(source="expert_employee_id", read_only=True,
                                                             slug_field="firstname")
    expert_employee_lastname = serializers.SlugRelatedField(source="expert_employee_id", read_only=True,
                                                            slug_field="lastname")

    class Meta:
        model = Appointment
        fields = ['id', 'creation_date', 'start', "end", "start_time", "end_time", "note", "comment", "reason",
                  "status", "expert_employee_id", "expert_employee_firstname", "expert_employee_lastname"]