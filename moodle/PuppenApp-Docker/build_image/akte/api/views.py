from django.shortcuts import render
from django.core.exceptions import ObjectDoesNotExist
from rest_framework import viewsets,mixins, response
from api.serializers import *
from rest_framework.permissions import IsAuthenticated  # <-- Here
from django.utils.dateparse import parse_datetime
# Create your views here.
from django.http import HttpResponse
from django.views.decorators.http import require_http_methods
import json
from rest_framework.views import APIView
from django.contrib.auth import logout

def last_edit_check(self, request, pk=None):
    if "last_edit" not in request.data:
        return HttpResponse(b'{"state":"last_edit field not found."}', content_type='application/json', status=400)
    le = parse_datetime(request.data["last_edit"])
    try:
        p = self.queryset.get(id=pk)
    except ObjectDoesNotExist:
        return HttpResponse(b'{"state":"Object to edit not found."}', content_type='application/json', status=400)

    if p.last_edit != le:
        return HttpResponse(b'{"state":"Your data is outdated."}', content_type='application/json', status=400)
    return True

class HairColorViewSet(viewsets.ModelViewSet):
    permission_classes = (IsAuthenticated,)             # <-- And here
    queryset = HairColor.objects.all().order_by('name')
    serializer_class = HairColorSerializer
    http_method_names=['get']

class HandlerViewSet(viewsets.ModelViewSet):
    permission_classes = (IsAuthenticated,)             # <-- And here
    queryset = User.objects.all().order_by('username')
    serializer_class = UserSerializer
    http_method_names = ['get']

class ColorViewSet(viewsets.ModelViewSet):
    permission_classes = (IsAuthenticated,)             # <-- And here
    queryset = Color.objects.all().order_by('name')
    serializer_class = ColorSerializer
    http_method_names=['get']

class PuppetListViewSet(viewsets.ModelViewSet):
    permission_classes = (IsAuthenticated,)             # <-- And here
    queryset = Puppet.objects.all().order_by('shirt_name')
    serializer_class = PuppetListSerializer
    http_method_names = ['get']

class IssueViewSet(viewsets.ModelViewSet):
    permission_classes = (IsAuthenticated,)             # <-- And here
    queryset = Issue.objects.all().order_by('id')
    serializer_class = FullIssueSerializer

    def update(self, request, *args, **kwargs):
        r = last_edit_check(self, request=request,pk=kwargs["pk"])
        if r is True:
            return super(self.__class__, self).update(request, *args, **kwargs)
        else:
            return r

    def perform_create(self, serializer):
        serializer.save(creator=self.request.user)

class IssueViewSet(viewsets.ModelViewSet):
    permission_classes = (IsAuthenticated,)             # <-- And here
    queryset = Issue.objects.all().order_by('id')
    serializer_class = FullIssueSerializer

    def update(self, request, *args, **kwargs):
        r = last_edit_check(self, request=request,pk=kwargs["pk"])
        if r is True:
            return super(self.__class__, self).update(request, *args, **kwargs)
        else:
            return r

    def perform_create(self, serializer):
        serializer.save(creator=self.request.user)

class IssueDataViewSet(viewsets.ModelViewSet):
    permission_classes = (IsAuthenticated,)             # <-- And here
    queryset = IssueData.objects.all().order_by('id')
    serializer_class = IssueDataSerializer

class FullPuppetViewSet(viewsets.ModelViewSet):
    permission_classes = (IsAuthenticated,)             # <-- And here
    queryset = Puppet.objects.all().order_by('shirt_name')
    serializer_class = FullPuppetSerializer

    def update(self, request, *args, **kwargs):
        r = last_edit_check(self, request=request,pk=kwargs["pk"])
        if r is True:
            return super(self.__class__, self).update(request, *args, **kwargs)
        else:
            return r

    def create(self, request, *args, **kwargs):
        return super(self.__class__, self).create(request, args,kwargs)

@require_http_methods(["GET"])
def issue_autocompletion(request):
    query = request.GET["text"]
    result = Issue.objects.all().filter(description__icontains=query)
    return HttpResponse(status=200, content=json.dumps([x.description for x in result]))


class UserToken(APIView):
    permission_classes = [IsAuthenticated]

    def delete(self, request, *args, **kwargs):
        request.user.auth_token.delete()
        logout(request)
        return response.Response(status=204)