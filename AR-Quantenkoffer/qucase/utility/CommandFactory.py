import dataclasses

from qucase.QuBrick import QuBrick
from .enums import Commands


@dataclasses.dataclass
class DummyBrick():
    x: int
    y: int
    rotation: int
    type: int
    setting: int


class CommandFactory:
    @staticmethod
    def generate_add_command(brick: QuBrick) ->dict:
        return {"command": Commands.Place, "posX": brick.x, "posY": brick.y, "rotation": brick.rotation, "type": brick.type}

    @staticmethod
    def generate_start_laser_command():
        return {"command": Commands.StartLaser, "posX": -1, "posY": -1}

    @staticmethod
    def generate_remove_command(brick: QuBrick)->dict:
        return {"command": Commands.Remove, "posX": brick.x, "posY": brick.y}

    @staticmethod
    def generate_setting_command(brick: QuBrick)->dict:
        return {"command": Commands.Setting, "posX": brick.x, "posY": brick.y, "value": brick.setting, "type":brick.type,
                "rotation": brick.rotation}
