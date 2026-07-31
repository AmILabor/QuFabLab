#!/bin/bash
# Startskript für den QuCase-Server.
# Aktiviert die virtuelle Umgebung und startet main.py mit optionalen Argumenten (z. B. --testing).
cd /home/pi/qucase/
source venv/bin/activate
python main.py $1
