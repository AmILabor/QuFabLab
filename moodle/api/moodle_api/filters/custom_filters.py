"""Benutzerdefinierte Django-Filter für Lektionen und Fragen."""
import django_filters
from django.core.validators import EMPTY_VALUES
from django.db import models
from django.db.models import Q

from moodle_api.models.mdl_lesson import MdlLesson
from moodle_api.models.mdl_question import MdlQuestion
from moodle_api.selectors import get_questions_by_tag, get_lesson_by_tag


# Filter, der auf leere Strings und NULL-Werte prüft.
class EmptyStringFilter(django_filters.BooleanFilter):
    # Wendet Filter-Logik an: schließt leere/NULL-Werte ein oder aus.
    def filter(self, qs, value):
        if value in EMPTY_VALUES:
            return qs

        exclude = self.exclude ^ (value is False)
        method = qs.exclude if exclude else qs.filter
        or_condition = Q()
        or_condition.add(Q(**{self.field_name:""}),Q.OR)
        or_condition.add(Q(**{self.field_name:None}),Q.OR)
        return method(or_condition)


# FilterSet für Fragen, inkl. Tag-basierter Filterung.
class QuestionFilter(django_filters.FilterSet):
    tag = django_filters.CharFilter(field_name='tag',lookup_expr="iexact",method='tag_filter')

    class Meta:
        model = MdlQuestion
        fields = {}

    # Wendet die Tag-Filterung über den Selektor get_questions_by_tag an.
    def tag_filter(self, queryset, name, value):
        return get_questions_by_tag(value)


# FilterSet für Lektionen, inkl. Tag-basierter Filterung.
class LessonFilter(django_filters.FilterSet):
    tag = django_filters.CharFilter(field_name='tag',lookup_expr="iexact",method='tag_filter')

    class Meta:
        model = MdlLesson
        fields = {}

    # Wendet die Tag-Filterung über den Selektor get_lesson_by_tag an.
    def tag_filter(self, queryset, name, value):
        return get_lesson_by_tag(value)
