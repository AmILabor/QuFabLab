from rest_framework import viewsets

from api.permissions import IsDispositionUser, IsAdminUser, IsExpertEmployeeUser


class ExpertEmployeeViewSet(viewsets.GenericViewSet):
    def get_permissions(self):
        return [IsExpertEmployeeUser()]