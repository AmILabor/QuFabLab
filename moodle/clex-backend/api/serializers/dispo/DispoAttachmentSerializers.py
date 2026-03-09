from rest_framework import serializers

from api.models.file import File


class AttachmentSerializer(serializers.ModelSerializer):
    class Meta:
        model = File
        fields = ['id', 'creation_date', 'name', 'extension', 'typ', 'order_id', 'appointment_id', 'invoice_id', 'path']


class AttachmentParameterSerializer(serializers.ModelSerializer):
    file = serializers.FileField()
    order_id = serializers.IntegerField()

    class Meta:
        model = File
        fields = ['file', 'order_id']


class AttachmentCreateSerializer(serializers.ModelSerializer):
    class Meta:
        model = File
        fields = ['name', 'extension', 'typ', 'order_id']