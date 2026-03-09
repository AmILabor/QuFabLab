import django_filters
from django.contrib.auth.models import Group
from django.db import transaction
from rest_framework import filters, response, status
from rest_framework.generics import get_object_or_404
from rest_framework.viewsets import GenericViewSet

from moodle_api.filters.custom_filters import QuestionFilter
from moodle_api.models.mdl_question import MdlQuestion
from moodle_api.models.mdl_question_answer import MdlQuestionAnswers
from moodle_api.serializers.question_serializers import MdlQuestionSerializer,MdlQuestionAnswerSerializer
from django.utils.html import strip_tags

class QuestionViewSet(GenericViewSet):
    queryset = MdlQuestion.objects.all()
    serializer_class = MdlQuestionSerializer
    filter_backends = [django_filters.rest_framework.DjangoFilterBackend, filters.OrderingFilter]
    filterset_class = QuestionFilter
    ordering=["id"]

    def retrieve(self, request, pk=None):
        data = get_object_or_404(self.get_queryset(), pk=pk)
        serialized = self.get_serializer(data)
        return response.Response(serialized.data)

    def list(self,request):
        qs = self.get_queryset()
        qs = self.filter_queryset(qs)
        serialized = self.get_serializer(qs,many=True)
        return response.Response(status=status.HTTP_200_OK,data=serialized.data)

class QuestionsViewSet(GenericViewSet):
    queryset = MdlQuestionAnswers.objects.all()
    serializer_class = MdlQuestionAnswerSerializer
    filter_backends = [django_filters.rest_framework.DjangoFilterBackend, filters.OrderingFilter]
    ordering=["id"]

    def list(self,request,pk=None):
        serialized = self.get_serializer(self.get_queryset(),many=True)
        return response.Response(status=status.HTTP_200_OK,data=serialized.data)

