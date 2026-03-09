
import os

from django.core.wsgi import get_wsgi_application
_e = ""
try:
    os.environ.setdefault('DJANGO_SETTINGS_MODULE', 'akte.settings')
    application = get_wsgi_application()
except Exception as e:
    _e = e
    print(_e)

