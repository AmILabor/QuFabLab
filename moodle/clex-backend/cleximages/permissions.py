from django.contrib.auth.models import AnonymousUser
from rest_framework import status
from rest_framework.permissions import IsAuthenticated
from rest_framework.response import Response

from api.permissions import PermissionMixin
from cleximages.models import ClexImageUser


class IsClexImagesUser(PermissionMixin):
    @staticmethod
    def has_permission(request, _):
        user = getattr(request,"user")
        if user is None:
            return False
        if not hasattr(user,"mandate_user"):
            return False
        mandate_user = user.mandate_user
        if mandate_user is None:
            return False
        is_clex_user = ClexImageUser.objects.filter(user__mandate_user__id=mandate_user.id)
        if len(is_clex_user)==0:
            return False
        return True