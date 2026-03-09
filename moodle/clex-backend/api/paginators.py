from django.utils.functional import cached_property
from rest_framework import pagination


class PageSizePaginator(pagination.PageNumberPagination):
    page_size_query_param = 'page_size'


class FasterPaginator(PageSizePaginator):
    @cached_property
    def count(self):
        return self.object_list.values('id').count()