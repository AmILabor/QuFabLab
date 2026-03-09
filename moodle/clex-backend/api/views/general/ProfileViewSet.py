import django_filters
from django.contrib.auth.models import Group
from django.db import transaction
from rest_framework import filters, response, status
from rest_framework.generics import get_object_or_404
from rest_framework.viewsets import GenericViewSet

from api.filters.AdminFilters import UsersFilter
from api.models.user import User, MandateUser
from api.serializers.admin.AdminUserSerializers import GroupSerializer, AdminUserSerializer
from api.paginators import FasterPaginator
from api.serializers.general.ProfileSerializer import ProfileSerializer, ProfilePatchSerializer
from api.services import update_sales_ares_per_user_by_list, update_user_groups_by_list
from api.views.admin.AdminViewSet import AdminViewSet

class ProfileViewSet(GenericViewSet):
    queryset = User.objects.filter(mandate_user__isnull=False)
    pagination_class = FasterPaginator
    serializer_class = ProfileSerializer
    filter_backends = [django_filters.rest_framework.DjangoFilterBackend, filters.OrderingFilter]
    filterset_class = UsersFilter
    ordering=["-mandate_user__active","mandate_user__lastname"]
    ordering_fields= ["username","mandate_user__firstname","mandate_user__lastname","mandate_user__expert_number",
                      "mandate_user__zip","mandate_user__city",("mandate_user__company__name1","company_name1")]


    def list(self,request,pk=None):
        user_object = get_object_or_404(User.objects.all(),pk=request.user.id)
        serialized = self.get_serializer(user_object)
        return response.Response(status=status.HTTP_200_OK,data=serialized.data)

    def patch(self, request, pk=None):
        user_object = get_object_or_404(User.objects.all(),pk=request.user.id)
        salesAreas = request.data.get("salesareas", False)
        grps = request.data.get("groups", False)
        if salesAreas is not False: del request.data["salesareas"]
        if grps is not False: del request.data["groups"]
        serializer = ProfilePatchSerializer(user_object,data=request.data,partial=True,context=request.user)
        if serializer.is_valid(raise_exception=True):
            with transaction.atomic():
                instance = serializer.save()
                mandate_instance = MandateUser.objects.get(pk=instance.mandate_user_id)
                if salesAreas is not False: update_sales_ares_per_user_by_list(mandate_instance, salesAreas)
            return response.Response(status=status.HTTP_200_OK, data=request.data)

    def destroy(self):
        return response.Response(status=status.HTTP_403_FORBIDDEN)



    def destroy(self, request, pk, *args, **kwargs):
        obj = get_object_or_404(self.queryset, pk=pk)
        user_object = MandateUser.objects.get(pk=obj.mandate_user_id)
        user_object.active=0
        user_object.save()
        return response.Response(status=status.HTTP_200_OK)