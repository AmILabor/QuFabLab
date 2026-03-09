from rest_framework import viewsets

from api.permissions import IsAdminUser


class AdminViewSet(viewsets.GenericViewSet):
    def get_permissions(self):
        return [IsAdminUser()]