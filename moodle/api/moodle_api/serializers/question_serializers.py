from rest_framework import serializers

from moodle_api.models.mdl_question import MdlQuestion
from moodle_api.models.mdl_question_answer import MdlQuestionAnswers
from moodle_api.serializers.fields import StripTagsSerializer




class MdlAnswerSerializer(serializers.ModelSerializer):
    answer = StripTagsSerializer()

    class Meta:
        model = MdlQuestionAnswers
        fields = ("id", "question", "answer", "answerformat", "fraction")

class MdlQuestionSerializer(serializers.ModelSerializer):
    questiontext = StripTagsSerializer()
    answers = MdlAnswerSerializer(many=True,read_only=True)
    class Meta:
        model = MdlQuestion
        fields = ("id", "name", "questiontext", "qtype","answers")


class MdlQuestionAnswerSerializer(serializers.ModelSerializer):
    question = MdlQuestionSerializer()
    answer = StripTagsSerializer()

    class Meta:
        model = MdlQuestionAnswers
        fields = ("id", "question", "answer", "answerformat", "fraction")
