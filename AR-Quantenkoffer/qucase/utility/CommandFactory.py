"""Befehlsfabrik für QuCase.

Erzeugt WebSocket-Nachrichten (dicts) zum Hinzufügen, Entfernen,
Einstellen und Starten von Lasern. Enthält ausserdem eine DummyBrick-Dataclass
für Testzwecke.
"""

import dataclasses

from qucase.QuBrick import QuBrick
from .enums import Commands


# Hilfsklasse für Tests: simuliert einen QuBrick ohne I2C-Hardware
@dataclasses.dataclass
class DummyBrick():
    x: int
    y: int
    rotation: int
    type: int
    setting: int


# Erzeugt die JSON-kodierbaren Dictionaries für WebSocket-Befehle
class CommandFactory:
    # Erzeugt einen "place"-Befehl zum Hinzufügen eines Bausteins
    @staticmethod
    def generate_add_command(brick: QuBrick) ->dict:
        return {"command": Commands.Place, "posX": brick.x, "posY": brick.y, "rotation": brick.rotation, "type": brick.type}

    # Erzeugt einen "start"-Befehl zum Starten des Lasers
    @staticmethod
    def generate_start_laser_command():
        return {"command": Commands.StartLaser, "posX": -1, "posY": -1}

    # Erzeugt einen "remove"-Befehl zum Entfernen eines Bausteins
    @staticmethod
    def generate_remove_command(brick: QuBrick)->dict:
        return {"command": Commands.Remove, "posX": brick.x, "posY": brick.y}

    # Erzeugt einen "setting"-Befehl zum Aktualisieren der Einstellung eines Bausteins
    @staticmethod
    def generate_setting_command(brick: QuBrick)->dict:
        return {"command": Commands.Setting, "posX": brick.x, "posY": brick.y, "value": brick.setting, "type":brick.type,
                "rotation": brick.rotation}
