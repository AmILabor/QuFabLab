from qucase.QuBoard import QuBoard
from websocket_communication.ws_server import WSServer
from .CommandFactory import CommandFactory, DummyBrick
import logging

logger = logging.getLogger("MenuHandlerUtils")


def print_help():
    """
    Prints the Help-Menu Structure if testing is enabled.
    :return:
    """
    print("""
     Commands to manage qubricks on a virtual board
     types: 0-3
     rotations: 0-3
     x: 1-11
     y: 1-7
     settings: 0-10
     a: Add
     d: Delete
     s: Setting
     l: Start Laser
     m: Create the michelson interferrometer

     Examples:
     #a 1 1 0 1\t [adds type 0 rotation 1 at 1|1 ]
     #d 1 1\t [delets at position 1|1]
     #s 1 1 10\t [modifies the setting of brick at 1|1 to 10]
     """)


def handle_manual_input(input_text: str) -> list[dict] | None:
    """
    Parses the manual input strings from print_help and returns websocket command messages.
    :param input_text: space separated string
    :return: list of command message-dicts
    """
    args = input_text.split(" ")
    if len(args) > 5:
        print("Too many arguments...")
    if len(args) < 1:
        print("Too few arguments")
    if args[0] == "m":
        inputs = ["a 4 0 2 0", "a 1 4 1 3", "a 4 4 0 1", "a 4 7 1 2", "a 7 4 2 1", "a 7 0 2 3"]
        out = []
        for _ in inputs:
            out += handle_manual_input(_)
        return out
    if args[0] == "l":
        return [CommandFactory.generate_start_laser_command()]
    if len(args) < 3:
        print("Too few arguments")
    if len(args) == 3:
        x = int(args[1])
        y = int(args[2])
        return [CommandFactory.generate_remove_command(DummyBrick(x, y, 0, 0, 0))]
    if args[0] == "a":
        _, x, y, type, rotation = args
        return [CommandFactory.generate_add_command(
            DummyBrick(int(x), int(y), type=int(type), rotation=int(rotation), setting=0))]
    if args[0] == "s":
        _, x, y, setting = args

        try:
            setting = float(setting)
        except Exception:
            setting = int(setting)
        if setting is None:
            return []
        return [CommandFactory.generate_setting_command(DummyBrick(int(x), int(y), 0, 0, setting=setting))]


def handle_testing_loop(board: QuBoard, server: WSServer):
    cmd = input("#")
    try:
        _msges = handle_manual_input(cmd)
    except Exception as e:
        logger.error(f"Could not handle input msg: {cmd}")
        logger.error(e)
        pass
    for _msg in _msges:
        print(_msg)
        if _msg["command"] == "place":
            _brick = board.add_qubrick(_msg["posX"], _msg["posY"], _msg["rotation"], _msg["type"])
        if _msg["command"] == "remove":
            board.remove_qubrick(_msg["posX"], _msg["posY"])
        if _msg["command"] == "setting":
            _brick = board.get_brick(_msg["posX"], _msg["posY"])
            _brick.setting = _msg["value"]
            board.update_bricks()
        if _msg["command"] == "start":
            server.broadcast(_msg)
