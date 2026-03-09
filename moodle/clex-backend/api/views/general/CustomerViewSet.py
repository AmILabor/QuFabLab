from rest_framework import viewsets, response
from rest_framework.generics import get_object_or_404

from api.models.company import Company
from api.serializers.general.CustomerSerializers import CustomerSerializer


class CustomerViewSet(viewsets.GenericViewSet):
    queryset = Company.objects.all()

    def list(self,request,*args,**kwargs):
        qs = self.get_queryset()
        output_serializer = CustomerSerializer(qs, many=True)
        return response.Response(output_serializer.data)

    def retrieve(self,request,pk=None):
        qs = self.get_queryset()
        qs = get_object_or_404(qs,pk=pk)
        output_serializer = CustomerSerializer(qs)
        return response.Response(output_serializer.data)