import django_filters
from django.db.models import Q
from django.db.models.functions import Length
from rest_framework import viewsets, response

from api.filters.DispoFilters import CoWorkerFilter
from api.models.user import MandateUser
from api.serializers.general.EmployeeSerializers import MandateUserSerializer


class EmployeeViewSet(viewsets.GenericViewSet):
    queryset = MandateUser.objects
    serializer_class = MandateUserSerializer
    filter_backends = [django_filters.rest_framework.DjangoFilterBackend]
    filterset_class = CoWorkerFilter

    def list(self,request,*args,**kwargs):
        qs = self.get_queryset().filter(active=1)

        if "zip_prefix" in request.query_params:
            zip = request.query_params["zip_prefix"]
            if len(zip) > 0:
                qs_filter = Q(salesareas__salesarea__zip_prefix=zip[0])
                for prefix_idx in range(2, len(zip) + 1):
                    qs_filter = qs_filter | Q(salesareas__salesarea__zip_prefix=zip[:prefix_idx])
                qs = qs.filter(qs_filter).order_by(Length("salesareas__salesarea__zip_prefix").desc())
        qs = self.filter_queryset(qs)
        output_serializer = self.serializer_class(qs,many=True)
        return response.Response(output_serializer.data)