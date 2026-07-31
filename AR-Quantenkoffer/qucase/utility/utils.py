"""Hilfsfunktionen für QuCase.

Enthält Werkzeuge zur LED-Steuerung, Timer-Verwaltung,
QR-Code-Generierung und Netzwerk-Erkennung.
"""

import os
import time
import logging
from typing import Callable, Optional
import RPi.GPIO as GPIO

REGISTRATION_TIMEOUT = 1.0  # seconds - adjust as needed
logger = logging.getLogger("QuBoard")


# Schaltet eine LED ein (GPIO HIGH)
def led_on(led_pin):
    GPIO.output(led_pin, GPIO.HIGH)


# Schaltet eine LED aus (GPIO LOW)
def led_off(led_pin):
    GPIO.output(led_pin, GPIO.LOW)


# Gibt True zurück, wenn die angegebene Zeitspanne seit `start` verstrichen ist
def elapsed(start: Optional[float], timeout: float, led_on: Callable[[], None], led_off: Callable[[], None],) -> bool:
    if start is None:
        led_off()
        return False
    if time.time() - start >= timeout:
        led_off()
        return True
    led_on()
    return False


# Erzeugt einen ASCII-QR-Code für die angegebene WebSocket-URI
def generate_connect_qrcode(text: str):
    import io
    import qrcode
    text = text.strip()
    qr = qrcode.QRCode(version=4, box_size=2, border=2)
    qr.add_data(text)
    f = io.StringIO()
    qr.print_ascii(out=f,invert=True)
    f.seek(0)
    lines = f.readlines()
    _lines = ""
    for l in lines:
        _lines+=f"\t\t\t   {l}"
    return "\n"+_lines+ f"\n\t\t\t\t    {text}    "


# Liest die IPv4-Adresse des wlan0-Interfaces aus und gibt sie mit Port 8123 zurück
def get_ip_port():
    ipv4 = os.popen(
        'ip addr show wlan0 | grep "\<inet\>" | awk \'{ print $2 }\' | awk -F "/" \'{ print $1 }\'').read().strip()
    return ipv4, 8123
