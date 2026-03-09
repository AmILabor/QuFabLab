from django.urls import path, include
from rest_framework import routers
from cleximages.views import AuftragViewSet, APITokenView, AuftragImageViewSet

router = routers.DefaultRouter()
router.register(r'orders', AuftragViewSet)
router.register(r'images', AuftragImageViewSet)

urlpatterns = [
    path('', include(router.urls)),
    path('auth/', APITokenView.as_view(), name='auth'),
]
