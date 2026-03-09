from rest_framework import viewsets

from api.permissions import IsDispositionUser, IsAdminUser


class DispoView(viewsets.GenericViewSet):
    def get_permissions(self):
        return [IsDispositionUser()]