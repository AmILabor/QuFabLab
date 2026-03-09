from rest_framework import serializers
from rest_framework.exceptions import ValidationError


class CustomModelRelatedField(serializers.RelatedField):
    def __init__(self,model,serializer,target_field,isMany=False,*args,**kwargs):
        self._model = model
        self._serializer = serializer
        self._target_field = target_field
        self._many= isMany
        super().__init__(**kwargs)

    def get_queryset(self):
        return self._model.objects.all()

    def to_representation(self,value):
        try:
            method = self.get_queryset().get
            if self._many: method = self.get_queryset().filter
            obj = method(**{self._target_field:value})
            serialized = self._serializer(obj, many=self._many)
            return serialized.data
        except Exception as e:
            return []

    def to_internal_value(self, data):
        if type(data) is dict and "id" in data:
            data = data["id"]
        if type(data) is not int:
            raise ValidationError(f"PK value should be an integer. It currently is {type(data)}")
        return data