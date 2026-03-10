import time

from utility.utils import generate_connect_qrcode, get_ip_port
from websocket_communication.ws_server import WSServer
from utility.CommandFactory import CommandFactory
from utility.MenuHandler import print_help, handle_testing_loop
from qucase.QuBoard import QuBoard
from qucase.TestingQuBoard import TestingQuBoard
import logging
from argparse import ArgumentParser
import os

time_format = "%d.%m %H:%M:%S"
logging.basicConfig(level=logging.INFO, datefmt=time_format,
                    format="%(asctime)s.%(msecs)03d %(name)-15s[%(levelname)-5s]: %(message)s")
logger = logging.getLogger("main")

if __name__ == "__main__":
    time.sleep(10)
    parser = ArgumentParser()
    parser.add_argument("--testing",action='store_true', default=False,required=False)
    args = parser.parse_args()
    testing = args.testing is not None
    ip, port = get_ip_port()
    ws_connection_uri = f"ws://{ip}:{port}"
    server = WSServer(ip=ip, port=port)
    testing = args.testing
    had_clients = False
    startup = True

    if testing:
        board = TestingQuBoard()
    else:
        board = QuBoard()
    restore = lambda client, server: [server.broadcast(CommandFactory.generate_add_command(brick)) for brick in
                                      board.get_bricks()]
    server.register_new_client_fn(restore)
    add = lambda brick: server.broadcast(CommandFactory.generate_add_command(brick))
    remove = lambda brick: server.broadcast(CommandFactory.generate_remove_command(brick))
    setting = lambda brick: server.broadcast(CommandFactory.generate_setting_command(brick))
    start = lambda brick: server.broadcast(CommandFactory.generate_start_laser_command())
    board.register_brick_add_callback(add)
    board.register_brick_remove_callback(remove)
    board.register_brick_update_callback(setting)
    board.register_start_laser_callback(start)

    if testing:
        print_help()

    while True:
        has_clients = server.has_clients()
        clients_changed = had_clients != has_clients
        if clients_changed or startup:
            had_clients = has_clients
        if not has_clients and (clients_changed or startup):
            if not testing:
                os.system("clear")
            logger.info("Waiting for connection...")s
        elif has_clients and clients_changed and not testing:
            if not testing:
                os.system("clear")

        startup = False
        if testing:
            handle_testing_loop(board=board, server=server)

        board.scan()
        board.update_bricks()
        time.sleep(0.05)
