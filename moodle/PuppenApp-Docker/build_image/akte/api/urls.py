from django.contrib.staticfiles.urls import staticfiles_urlpatterns
from django.urls import path,include
from django.conf.urls.static import static
from django.views.generic import TemplateView
from rest_framework import routers
from django.conf import settings

from . import views
router = routers.DefaultRouter()
router.register(r'colors',views.ColorViewSet,basename="colors")
router.register(r'haircolors',views.HairColorViewSet,basename="haircolors")
router.register(r'puppets',views.PuppetListViewSet,basename="puppets")
router.register(r'puppet',views.FullPuppetViewSet,basename="puppet")
router.register(r'issues',views.IssueViewSet,basename="issues")
router.register(r'issuedata',views.IssueDataViewSet,basename="issuedata")
router.register(r'handlers',views.HandlerViewSet,basename="handlers")

urlpatterns = [
      path('',include(router.urls)),
      path("autocompletion",views.issue_autocompletion),
      path("session", views.UserToken.as_view())
]
