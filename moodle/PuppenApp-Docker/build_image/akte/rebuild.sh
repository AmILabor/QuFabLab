#!/bin/bash
# Führt Migrationen durch und startet den Django-Entwicklungsserver
python3 manage.py makemigrations
python3 manage.py migrate
python3 manage.py runserver 0.0.0.0:8000 
