import logging
from websocket_server import WebsocketServer
import json

time_format = "%d.%m %H:%M:%S"
logging.basicConfig(level=logging.INFO, datefmt=time_format,
                    format="%(asctime)s.%(msecs)03d %(name)-15s[%(levelname)-5s]: %(message)s")
logger = logging.getLogger("WSServer")


class WSServer():
    callbacks: dict[str, list[callable]]
    callbacks: dict[str, dict]

    def __init__(self, ip: str = '0.0.0.0', port: int = 8123, loglevel=logging.WARNING):
        self.callbacks = {}
        self.clients = {}
        self.__ws_server = WebsocketServer(host=ip, port=port)
        self.__ws_server.set_fn_new_client(self.__new_client_fn)
        self.__ws_server.set_fn_client_left(self.__left_client_fn)
        self.__ws_server.set_fn_message_received(self.__message_fn)
        logger.info(f"Started WSServer on {ip}:{port}")
        self.__ws_server.run_forever(threaded=True)

    def __new_client_fn(self, client, server) -> None:
        """
        Callback for websocket framework. Calls new_client-callbacks
        :param client: client that has connected
        :param server: websocket-framework server instance
        :return: None
        """
        for _cb in self.callbacks.get('new_client', []):
            _cb(client,self)
        self.clients[client['id']] = client

    def __message_fn(self, client, server, msg):
        """
          Callback for websocket framework. Calls message callbacks
          :param client: client that has connected
          :param server: websocket-framework server instance
          :param msg: message that has been sent.
          :return: None
          """
        for _cb in self.callbacks.get('message', []):
            _cb(client,server,msg)
        if client["id"] in self.clients:
            del self.clients[client['id']]

    def __left_client_fn(self, client, server):
        """
          Callback for websocket framework. Calls left_client-callbacks
          :param client: client that has connected
          :param server: websocket-framework server instance
          :return: None
          """
        for _cb in self.callbacks.get('left_client', []):
            _cb(client)
        if client["id"] in self.clients:
            del self.clients[client['id']]

    def has_clients(self):
        """
        Returns if the server currently has clients.
        :return: bool
        """
        return len(self.clients) > 0

    def register_message_fn(self,cb: callable):
        _callbacks = self.callbacks.get('message', [])
        _callbacks.append(cb)
        self.callbacks['message'] = _callbacks

    def register_new_client_fn(self, cb: callable):
        """
        Registers a new callback that is called when a new client connects
        :param cb: callback to be called.
        :return: None
        """
        _callbacks = self.callbacks.get('new_client', [])
        _callbacks.append(cb)
        self.callbacks['new_client'] = _callbacks

    def register_left_client_fn(self, cb: callable):
        """
        Registers a new callback that is called when a connected client leaves
        :param cb: callback to be called.
        :return: None
        """
        _callbacks = self.callbacks.get('left_client', [])
        _callbacks.append(cb)
        self.callbacks['left_client'] = _callbacks

    def broadcast(self, msg: dict):
        """
        Sends a message to all clients. encodes it by json
        :param msg: dictionary that is encoded and then sent
        :return: None
        """
        try:
            _msg = json.dumps(msg)
        except TypeError as e:
            logger.error(repr(e))
            logger.error(f"Type Error on sending: {msg}")
            return
        self.__ws_server.send_message_to_all(_msg)

if __name__=="__main__":
    from time import sleep
    import json

    def handle_msg(msg: str):
        try:
            _json_msg = json.loads(msg)
        except Exception as e:
            print("Error on decode")
            print(e)
            print(msg)
            return
        _msg = _json_msg.get("message","").split("</color>")[-1].strip()
        print(f"[>] {_msg}")


    server = WSServer(ip="0.0.0.0", port=8080)
    server.register_new_client_fn(lambda client,server: print("[V] Client connected"))
    server.register_left_client_fn(lambda server: print("[X] Client left"))
    server.register_message_fn(lambda client, server, msg: handle_msg(msg))
    logger.info("Server started...")
    while True:
        sleep(0.01)
