from django.http import Http404
from rest_framework import viewsets, response

from api.models.slareport import SLAsPerCustomer
from api.serializers.general.SLASerializers import SLAsPerCustomerSerializer


class SLAsPerCustomerViewSet(viewsets.GenericViewSet):
    queryset = SLAsPerCustomer.objects.all()
    serializer_class = SLAsPerCustomerSerializer

    def list(self,request,*args,**kwargs):
        qs = self.get_queryset()
        serializer = self.serializer_class
        return response.Response(serializer(qs,many=True).data)

    def retrieve(self,request,pk=None):
        try:
            qs = self.get_queryset().filter(customer=pk)
        except:
            raise Http404
        output_serializer = self.serializer_class(qs,many=True)
        return response.Response(output_serializer.data)