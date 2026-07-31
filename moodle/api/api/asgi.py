"""
ASGI-Konfiguration für das api-Projekt.

Stellt die ASGI-Anwendung als modulweite Variable ``application`` bereit.
"""

import os

from django.core.asgi import get_asgi_application

os.environ.setdefault('DJANGO_SETTINGS_MODULE', 'api.settings')

application = get_asgi_application()
