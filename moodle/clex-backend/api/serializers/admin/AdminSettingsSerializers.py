from django.db import transaction
from rest_framework import serializers

from api.models.app_settings import SettingsModel, WorkFreeDatesModel

class WorkFreeDatesSerializer(serializers.ModelSerializer):
    settings_id = serializers.IntegerField()
    class Meta:
        model = WorkFreeDatesModel
        fields = ['id','day','month',"settings_id"]
        extra_kwargs = {
            'id': {'read_only': True},
            'settings_id':{'write_only':True}
        }

class AdminSettingsSerializer(serializers.ModelSerializer):
    work_free_dates = WorkFreeDatesSerializer(many=True)
    class Meta:
        model = SettingsModel
        fields = '__all__'
        extra_kwargs = {
            'email_password': {'write_only': True}
        }

    def update(self, instance, validated_data):
        with transaction.atomic():
            WorkFreeDatesModel.objects.all().delete()
            work_free_dates = validated_data.pop('work_free_dates')
            for work_free_date in work_free_dates:
                if "id" in work_free_date:
                    del work_free_date["id"]
                work_free_date["settings_id"] = instance.id
                serializer = WorkFreeDatesSerializer(data=work_free_date)
                if serializer.is_valid(raise_exception=True):
                    serializer.save()
            SettingsModel.objects.update_or_create(id=instance.id,defaults=validated_data)
        return SettingsModel.objects.get(id=instance.id)


