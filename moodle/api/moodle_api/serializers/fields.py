from rest_framework import serializers
from moodle_api.services import strip_tags


class StripTagsSerializer(serializers.CharField):
    def to_representation(self, value):
        res = super().to_representation(value)
        return strip_tags(res)
