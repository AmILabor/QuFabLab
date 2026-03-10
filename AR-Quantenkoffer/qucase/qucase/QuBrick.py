
from config import *
import logging

logger = logging.getLogger("QuBrick")

class QuBrick:
    def __init__(self,i2c,address,xpos,ypos):
        self.i2c_bus = i2c
        self.address = address
        self.type = None
        self.setting = None
        self.rotation = None
        self.x = xpos
        self.y = ypos
        self.asd = None
        self.read_error=False

    def read_register(self,register,bytesize):
        try:
            self.i2c_bus.writeto(self.address,bytes([register]))
            result = bytearray(bytesize)
            self.i2c_bus.readfrom_into(self.address,result)
        except Exception as e:
            self.read_error = True
            return False
        return result

    def read_setting(self):
        _setting_bytes = self.read_register(SETTING_REGISTER0, 2)
        setting=False
        if not self.read_error:
            setting = int.from_bytes(_setting_bytes,'little',signed=True)/10.0
        return setting

    def read_type(self):
        _type = self.read_register(TYPE_REGISTER, 1)
        if not self.read_error:
            return _type[0]
        return 0

    def read_rotation(self):
        _rot = self.read_register(ROTATION_REGISTER, 1)
        if not self.read_error:
            return _rot[0]
        return 0

    def store_settings_to_brick(self):
        self.read_register(STORE_REGISTER,1)

    def fetch(self):
        _type=self.read_type()
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
