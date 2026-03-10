from QuCaseWebsocketServer import QuCaseWebsocketServer
from enums import Rotations, QuBrickTypes, Commands
from time import sleep
import threading

class QuCase:
    def __init__(self):
        self.ws_server = QuCaseWebsocketServer()
        self.ws_server.start()

    def add_brick(self,posX: int,posY: int ,rotation: Rotations,type: QuBrickTypes):
        self.ws_server.put_broadcast_message({"command":Commands.Place,"posX":posX,"posY":posY,"rotation":rotation,"type":type})

    def remove_brick(self,posX: int, posY: int):
        self.ws_server.put_broadcast_message(
            {"command": Commands.Remove, "posX": posX, "posY": posY})

    def set_brick_setting(self,posX: int ,posY: int ,type: QuBrickTypes, value: float):
        self.ws_server.put_broadcast_message(
            {"command": Commands.Setting, "posX": posX, "posY": posY, "value":value,"type":type})

    def stop(self):
        self.ws_server.stop()

class QuCaseTester():
    def __init__(self,sleep_inbetween=2):
        self.qucase = QuCase()
        self.__testing = False
        self.sleep_inbetween = sleep_inbetween
        self.__awaiting_connection = True
        self.qucase.ws_server.register_client_connected_callback(lambda x: self.run_test())
        self.qucase.ws_server.register_client_closed_callback(lambda x: self.disconnected())

    def disconnected(self):
        self.__testiing=False
        self.__awaiting_connection = True

    def run_test(self):
        self.__testing = True
        self.__awaiting_connection = False
        t = threading.Thread(target=self.__test_case)
        t.start()

    def test_done(self):
        return not self.testing

    def awaiting_connection(self):
        return self.awaiting_connection

    def __test_case(self):
        testcases = [(-1, 0, Rotations.North, QuBrickTypes.Periscope),
                     (11, 0, Rotations.North, QuBrickTypes.Periscope),
                     (4, 0, Rotations.North, QuBrickTypes.Mirror45),
                     (4, 4, Rotations.East, QuBrickTypes.BeamSplitter),
                     (0, 4, Rotations.West, QuBrickTypes.Mirror90),
                     (4, 7, Rotations.South, QuBrickTypes.Mirror90),
                     (8, 4, Rotations.East, QuBrickTypes.Mirror45),
                     (8, 0, Rotations.West, QuBrickTypes.Mirror45)]
        setting_testcases = [0.0625, 0.125, 0.1875, 0.25, 0.3125, 0.375, 0.4375, 0.5]
        for case in testcases:
            self.qucase.add_brick(*case)
            sleep(self.sleep_inbetween)
        for case in setting_testcases:
            self.qucase.set_brick_setting(0, 4, QuBrickTypes.Mirror90, case)
            sleep(self.sleep_inbetween)
        for case in testcases:
            self.qucase.remove_brick(case[0], case[1])
            sleep(self.sleep_inbetween)
        self.__testing = False