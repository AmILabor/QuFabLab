"""
Haupt-URL-Konfiguration des api-Projekts.

Leitet Anfragen an das Admin-Interface und die moodle_api-URLs weiter.
"""
from django.contrib import admin
from django.urls import path, include

urlpatterns = [
    path('admin/', admin.site.urls),
    path('api/', include('moodle_api.urls')),
]

