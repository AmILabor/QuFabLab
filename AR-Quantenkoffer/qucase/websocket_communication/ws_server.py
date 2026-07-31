"""WebSocket-Server für QuCase.

Kapselt die Kommunikation zwischen dem Python-Backend und der Unity-Anwendung.
Bietet Methoden zum Senden von Broadcasts sowie zum Registrieren von Callbacks
für Verbindungs-, Trennungs- und Nachrichtenereignisse.
"""

import logging
from websocket_server import WebsocketServer
import json

time_format = "%d.%m %H:%M:%S"
logging.basicConfig(level=logging.INFO, datefmt=time_format,
                    format="%(asctime)s.%(msecs)03d %(name)-15s[%(levelname)-5s]: %(message)s")
logger = logging.getLogger("WSServer")


# WebSocket-Server, der Nachrichten an alle verbundenen Clients sendet
class WSServer():
    callbacks: dict[str, list[callable]]
    callbacks: dict[str, dict]

    # Initialisiert den Server, registriert die Framework-Callbacks und startet den Hintergrund-Thread
    def __init__(self, ip: str = '0.0.0.0', port: int = 8123, loglevel=logging.WARNING):
        self.callbacks = {}
        self.clients = {}
        self.__ws_server = WebsocketServer(host=ip, port=port)
        self.__ws_server.set_fn_new_client(self.__new_client_fn)
        self.__ws_server.set_fn_client_left(self.__left_client_fn)
        self.__ws_server.set_fn_message_received(self.__message_fn)
        logger.info(f"Started WSServer on {ip}:{port}")
        self.__ws_server.run_forever(threaded=True)

    # Interner Callback: wird vom WebSocket-Framework bei einem neuen Client aufgerufen
    def __new_client_fn(self, client, server) -> None:
        for _cb in self.callbacks.get('new_client', []):
            _cb(client,self)
        self.clients[client['id']] = client

    # Interner Callback: wird vom WebSocket-Framework bei einer eingehenden Nachricht aufgerufen
    def __message_fn(self, client, server, msg):
        for _cb in self.callbacks.get('message', []):
            _cb(client,server,msg)
        if client["id"] in self.clients:
            del self.clients[client['id']]

    # Interner Callback: wird vom WebSocket-Framework aufgerufen, wenn ein Client die Verbindung trennt
    def __left_client_fn(self, client, server):
        for _cb in self.callbacks.get('left_client', []):
            _cb(client)
        if client["id"] in self.clients:
            del self.clients[client['id']]

    # Gibt True zurück, wenn momentan mindestens ein Client verbunden ist
    def has_clients(self):
        return len(self.clients) > 0

    # Registriert einen Callback für eingehende Nachrichten
    def register_message_fn(self,cb: callable):
        _callbacks = self.callbacks.get('message', [])
        _callbacks.append(cb)
        self.callbacks['message'] = _callbacks

    # Registriert einen Callback, der bei einer neuen Client-Verbindung aufgerufen wird
    def register_new_client_fn(self, cb: callable):
        _callbacks = self.callbacks.get('new_client', [])
        _callbacks.append(cb)
        self.callbacks['new_client'] = _callbacks

    # Registriert einen Callback, der aufgerufen wird, wenn ein Client die Verbindung trennt
    def register_left_client_fn(self, cb: callable):
        _callbacks = self.callbacks.get('left_client', [])
        _callbacks.append(cb)
        self.callbacks['left_client'] = _callbacks

    # Sendet eine Nachricht (als JSON) an alle verbundenen Clients
    def broadcast(self, msg: dict):
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

    # Hilfsfunktion: dekodiert eine Nachricht und gibt sie formatiert aus
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
