"""ViewSets für Lektionen (Lessons) der Moodle-Datenbank."""
import django_filters
from rest_framework import filters, response, status
from rest_framework.generics import get_object_or_404
from rest_framework.viewsets import GenericViewSet

from moodle_api.filters.custom_filters import LessonFilter
from moodle_api.models.mdl_lesson import MdlLessonPages, MdlLesson
from moodle_api.serializers.lesson_serializer import MdlLessonSerializer

# ViewSet zum Abrufen und Filtern von Moodle-Lektionen.
class LessonViewSet(GenericViewSet):
    queryset = MdlLesson.objects.all()
    serializer_class = MdlLessonSerializer
    filter_backends = [django_filters.rest_framework.DjangoFilterBackend, filters.OrderingFilter]
    filterset_class = LessonFilter
    ordering=["id"]

    # Ruft eine einzelne Lektion anhand ihrer ID ab.
    def retrieve(self, request, pk=None):
        data = get_object_or_404(self.get_queryset(), pk=pk)
        serialized = self.get_serializer(data)
        return response.Response(serialized.data)

    # Listet alle Lektionen auf, optional gefiltert.
    def list(self,request):
        qs = self.get_queryset()
        qs = self.filter_queryset(qs)
        serialized = self.get_serializer(qs,many=True)
        return response.Response(status=status.HTTP_200_OK,data=serialized.data)
