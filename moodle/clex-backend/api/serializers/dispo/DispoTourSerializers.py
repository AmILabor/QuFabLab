from rest_framework import serializers

from api.models.tour import Tour
from api.models.user import MandateUser
from api.serializers.dispo.DispoAppointmentSerializers import DispoOrderAppointmentSerializer
from api.serializers.general.EmployeeSerializers import MandateUserSerializer

"""
class DispoTourStopSerializer(serializers.ModelSerializer):
    order_id = serializers.IntegerField()
    tour_id = serializers.IntegerField()
    order_appointment = DispoOrderAppointmentSerializer(source="order",read_only=True)
    class Meta:
        model = TourStop
        fields = ['id','order_id','tour_id','order_appointment']
"""

class DispoTourSwapSerializer(serializers.Serializer):
    expert_employee_1 = serializers.IntegerField(required=True)
    expert_employee_2 = serializers.IntegerField(required=True)
    tour_date = serializers.DateField(required=True)


class DispoTourCreateSerializer(serializers.ModelSerializer):
    tour_date = serializers.DateField(required=True)
    class Meta:
        model = Tour
        fields = ['tour_date','expert_employee']


    def create(self,validated_data):
        validated_data["sent_by"] = MandateUser.objects.get(pk=self.context.mandate_user_id)
        return super().create(validated_data)



class DispoTourSerializer(serializers.Serializer):
    id = serializers.IntegerField(read_only=True)
    sent_by = MandateUserSerializer(read_only=True)
    expert_employee = MandateUserSerializer(required=True)
    expert_employee_id = serializers.SlugRelatedField(source="expert_employee",slug_field="id",read_only=True)
    created_at = serializers.DateTimeField(read_only=True)
    sent_at = serializers.DateTimeField(read_only=True)
    tour_date = serializers.DateField(required=True)
    tour_stops = DispoOrderAppointmentSerializer(many=True)
    class Meta:
        model = Tour
        fields = ['id','sent_by','expert_employee','created_at','tour_stops','sent_at','tour_date','tour_stops']

