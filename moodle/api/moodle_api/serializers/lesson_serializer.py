from rest_framework import serializers

from moodle_api.models.mdl_lesson import MdlLesson, MdlLessonPages
from moodle_api.serializers.fields import StripTagsSerializer


class MdlLessonPageSerializer(serializers.ModelSerializer):
    contents = StripTagsSerializer()

    class Meta:
        model = MdlLessonPages
        fields = ("id", "title", "contents")

class MdlLessonSerializer(serializers.ModelSerializer):
    intro = StripTagsSerializer()
    pages = MdlLessonPageSerializer(many=True,read_only=True)

    class Meta:
        model = MdlLesson
        fields = ("id", "name", "intro","pages")
