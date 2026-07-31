"""Aufzählungen für QuCase.

Definiert Konstanten für Befehle (Commands), Ausrichtungen (Rotations)
und Baustein-Typen (QuBrickTypes).
"""

# Befehle für die WebSocket-Kommunikation mit der Unity-Anwendung
class Commands:
    Place= "place"
    Remove= "remove"
    Setting= "setting"
    StartLaser ="start"

# Himmelsrichtungen als Rotationswerte (0–3)
class Rotations:
    North= 0
    South= 1
    East= 2
    West= 3

# IDs der verschiedenen QuBrick-Typen
class QuBrickTypes:
    Mirror90= 1
    Mirror45= 2
    BeamSplitter= 0
    Periscope= 3
