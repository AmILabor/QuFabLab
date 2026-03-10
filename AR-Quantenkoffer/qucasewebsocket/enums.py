from enum import Enum

class Commands():
    Place= "place"
    Remove= "remove"
    Setting= "setting"

class Rotations():
    North= 0
    East= 1
    South= 2
    West= 3

class QuBrickTypes():
    BeamSplitter= 0
    Mirror90= 1
    Mirror45= 2
    Periscope= 3
    GlassWedge= 4
    Polarizer= 5
    TunnelEffect= 6
    Camera= 7
    DoubleSlit= 8
    FibreCoupler= 9
    Waveplate= 10

