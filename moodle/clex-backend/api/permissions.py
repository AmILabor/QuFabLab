from django.contrib.auth.models import Group
from rest_framework import permissions

class PermissionMixin(permissions.BasePermission):

    def _is_permitted(self,request):
        if not hasattr(self,"groups"):
            raise NotImplementedError
        return permission_builder(request, self.groups)

    def has_permission(self, request, view):
        return self._is_permitted(request)

    def has_object_permission(self, request, view, obj):
        return self._is_permitted(request)

    def __add__(self, other):
        if not hasattr(self,"groups"):
            raise NotImplementedError
        if not hasattr(other,"groups"):
            raise NotImplementedError
        _mixin = PermissionMixin()
        _mixin.groups = self.groups+other.groups
        return _mixin

class IsAnonymous(PermissionMixin):
    @staticmethod
    def _is_permitted(request):
        return request.auth is None


def is_in_group(user, group_name):
    """
    Takes a user and a group name, and returns `True` if the user is in that group.
    """
    try:
        return Group.objects.get(name=group_name).user_set.filter(id=user.id).exists()
    except Group.DoesNotExist:
        return None

def permission_builder(request,groups,mode="OR"):
    if request.auth is None:
        return False
    permitted_list = [is_in_group(request.user,group) for group in groups]
    if mode =="OR":
        return any(permitted_list)
    return all(permitted_list)


class IsAdminUser(PermissionMixin):
    groups=["admin"]

class IsDispositionUser(PermissionMixin):
    groups = ["admin", "dispo"]

class IsExpertEmployeeUser(PermissionMixin):
    groups =["admin","expert"]
