"""
WSGI-Konfiguration für das api-Projekt.

Stellt die WSGI-Anwendung als modulweite Variable ``application`` bereit.
"""

import os

from django.core.wsgi import get_wsgi_application

os.environ.setdefault('DJANGO_SETTINGS_MODULE', 'api.settings')

application = get_wsgi_application()
