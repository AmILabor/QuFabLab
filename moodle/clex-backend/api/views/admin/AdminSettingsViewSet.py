
from django.core.cache import cache
from rest_framework import  response, status
from rest_framework.generics import get_object_or_404

from api.models.app_settings import SettingsModel, APP_SETTINGS_WORKDAY_CACHE_KEY, \
    APP_SETTINGS_CACHE_KEY
from api.serializers.admin.AdminSettingsSerializers import AdminSettingsSerializer
from api.paginators import FasterPaginator
from api.views.admin.AdminViewSet import AdminViewSet

class AdminSettingsViewSet(AdminViewSet):
    queryset = SettingsModel.objects
    pagination_class = FasterPaginator
    serializer_class = AdminSettingsSerializer

    def list(self, request, *args, **kwargs):
        qs = self.get_queryset().get(pk=1)
        output_serializer = self.serializer_class(qs)
        return response.Response(output_serializer.data)

    def retrieve(self,request,*args,**kwargs):
        qs = self.get_queryset().get(pk=1)
        output_serializer = self.serializer_class(qs)
        return response.Response(output_serializer.data)

    def patch(self, request, pk=None):
        cache.delete(APP_SETTINGS_WORKDAY_CACHE_KEY)
        cache.delete(APP_SETTINGS_CACHE_KEY)
        settings_object = get_object_or_404(self.get_queryset(),pk=pk)
        serializer = self.serializer_class(settings_object,data=request.data,partial=True)
        if serializer.is_valid(raise_exception=True):
            serializer.save()
            return response.Response(status=status.HTTP_200_OK, data=request.data)

    def destroy(self,pk):
        pass