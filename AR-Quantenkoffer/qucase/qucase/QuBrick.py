"""QuBrick — I2C-Kommunikation mit einem einzelnen QuBrick.

Liest Typ-, Einstellungs- und Rotationsregister eines Bausteins
über den I2C-Bus aus. Jeder QuBrick wird durch seine I2C-Adresse
und seine Feldposition (x, y) identifiziert.
"""

from config import *
import logging

logger = logging.getLogger("QuBrick")

# Repräsentiert einen einzelnen QuBrick auf dem Spielfeld
class QuBrick:
    # Initialisiert einen QuBrick mit I2C-Bus, Adresse und Position
    def __init__(self, i2c, address, xpos, ypos):
        self.i2c_bus = i2c
        self.address = address
        self.type = None
        self.setting = None
        self.rotation = None
        self.x = xpos
        self.y = ypos
        self.asd = None
        self.read_error = False

    # Liest eine bestimmte Anzahl Bytes aus einem I2C-Register; setzt read_error bei Fehler
    def read_register(self, register, bytesize):
        try:
            self.i2c_bus.writeto(self.address, bytes([register]))
            result = bytearray(bytesize)
            self.i2c_bus.readfrom_into(self.address, result)
        except Exception:
            self.read_error = True
            return False
        return result

    # Liest den Einstellungswert (signed 16-bit Little-Endian / 10)
    def read_setting(self):
        _setting_bytes = self.read_register(SETTING_REGISTER0, 2)
        setting = False
        if not self.read_error:
            setting = int.from_bytes(_setting_bytes, 'little', signed=True) / 10.0
        return setting

    # Liest den Baustein-Typ (0–3)
    def read_type(self):
        _type = self.read_register(TYPE_REGISTER, 1)
        if not self.read_error:
            return _type[0]
        return 0

    # Liest die Rotation (0–3, entspricht N/S/O/W)
    def read_rotation(self):
        _rot = self.read_register(ROTATION_REGISTER, 1)
        if not self.read_error:
            return _rot[0]
        return 0

    # Speichert die aktuellen Einstellungen dauerhaft auf dem Baustein
    def store_settings_to_brick(self):
        self.read_register(STORE_REGISTER, 1)

    # Liest alle Register aus und gibt True zurück, wenn sich ein Wert geändert hat
    def fetch(self):
        _type = self.read_type()
        _setting = self.read_setting()
        _rotation = self.read_rotation()
        if self.read_error:
            return False
        changed = _setting != self.setting or _rotation != self.rotation or _type != self.type

        self.setting = _setting
        self.rotation = _rotation
        self.type = _type
        if changed:
            logger.info(f"Fetching ({self.x},{self.y})@{self.address} \t Type={self.type} | Setting={self.setting} | Rotation={self.rotation}")
        return changed
