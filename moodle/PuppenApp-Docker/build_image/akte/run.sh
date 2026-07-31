#!/bin/bash
# Startet den Django-Entwicklungsserver in einer screen-Session namens "akte"
screen -dmS akte python3 manage.py runserver 0.0.0.0:8000
