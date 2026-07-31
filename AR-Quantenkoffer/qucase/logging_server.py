"""
WebSocket-Logging-Server für die Unity-Anwendung.

Die IP-Adresse des Servers muss im GameObject "WSLogger"
in der entsprechenden Szene eingestellt werden.
Eignet sich zum Debuggen und Überwachen der Nachrichten.
"""

from websocket_communication.ws_server import WSServer
from time import sleep

import logging
import json


# Gibt eine eingehende WebSocket-Nachricht formatiert auf der Konsole aus
def print_msg(ob):
    msg = json.loads(ob).get("message","")
    level = json.loads(ob).get("type","")
    print(f"[{level:10s}] {msg}")

if __name__ == "__main__":
    server= WSServer(port=8080)
    server.register_message_fn(lambda x,y,z: print_msg(z))
    while True:
        sleep(1)