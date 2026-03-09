import django_filters
from django.core.validators import EMPTY_VALUES
from django.db import models
from django.db.models import Q

from moodle_api.models.mdl_lesson import MdlLesson
from moodle_api.models.mdl_question import MdlQuestion
from moodle_api.selectors import get_questions_by_tag, get_lesson_by_tag


class EmptyStringFilter(django_filters.BooleanFilter):
    def filter(self, qs, value):
        if value in EMPTY_VALUES:
            return qs

        exclude = self.exclude ^ (value is False)
        method = qs.exclude if exclude else qs.filter
        or_condition = Q()
        or_condition.add(Q(**{self.field_name:""}),Q.OR)
        or_condition.add(Q(**{self.field_name:None}),Q.OR)
        return method(or_condition)


class QuestionFilter(django_filters.FilterSet):
    tag = django_filters.CharFilter(field_name='tag',lookup_expr="iexact",method='tag_filter')

    class Meta:
        model = MdlQuestion
        fields = {}

    def tag_filter(self, queryset, name, value):
        return get_questions_by_tag(value)


class LessonFilter(django_filters.FilterSet):
    tag = django_filters.CharFilter(field_name='tag',lookup_expr="iexact",method='tag_filter')

    class Meta:
        model = MdlLesson
        fields = {}

    def tag_filter(self, queryset, name, value):
        return get_lesson_by_tag(value)
