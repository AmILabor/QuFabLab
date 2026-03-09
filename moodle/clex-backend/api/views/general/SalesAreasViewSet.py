from rest_framework import viewsets, response
from rest_framework.generics import get_object_or_404

from api.models.salesareas import SalesAreas
from api.serializers.general.SalesAreasSerializer import SalesAreasSerializer


class SalesAreasViewSet(viewsets.GenericViewSet):
    queryset = SalesAreas.objects.all()
    serializer_class = SalesAreasSerializer

    def retrieve(self,request,pk=None):
        ob = get_object_or_404(self.get_queryset())
        output_serializer = self.serializer_class(ob)
        return response.Response(output_serializer.data)

    def list(self,request, *args,**kwargs):
        qs = self.get_queryset()
        output_serializer = self.serializer_class(qs,many=True)
        return response.Response(output_serializer.data)