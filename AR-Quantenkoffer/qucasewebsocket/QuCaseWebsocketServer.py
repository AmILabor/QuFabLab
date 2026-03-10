import asyncio
import multiprocessing
from threading import Thread
import logging
import websockets
import json
from  multiprocessing import Queue

logger = logging.getLogger("QuCaseWebsocketServer")
logging.basicConfig(level=logging.INFO)

class QuCaseWebsocketServer():
    def __init__(self):
        self.queue = Queue()
        self.loop = asyncio.new_event_loop()
        self.running = False
        self.client_map = {}
        self.ws_thread = Thread(target=self.run,daemon=True)
        self.client_connected_callbacks = []
        self.client_closed_callbacks = []

    def register_client_closed_callback(self, cb):
        self.client_closed_callbacks.append(cb)

    def register_client_connected_callback(self, cb):
        self.client_connected_callbacks.append(cb)

    def start(self):
        self.running=True
        self.ws_thread.start()

    def put_broadcast_message(self, message: dict):
        try:
            message = json.dumps(message)
        except Exception as e:
            logger.error("Error while converting dict Message to JSON string")
            return
        self.queue.put(message)

    async def __send_ws_message(self,websocket,message: str):
        if type(message) is dict:
            message = json.dumps(message)

        logger.info(f"Sending {message}")
        try:
            await websocket.send(message)
        except websockets.ConnectionClosed:
            logger.info(f"Client closed {self.client_map[websocket]}")
            del self.client_map[websocket]

    async def __try_register_client(self, websocket):
        if websocket not in self.client_map:
            ids = [0] + list([x["id"] for x in self.client_map.values()])
            self.client_map[websocket] = {"id":max(ids) + 1}
            logger.info(f"New Client registered {self.client_map[websocket]}")
            for cb in self.client_connected_callbacks:
                cb(self.client_map[websocket])

    def __cleanup_clients(self):
        to_remove = []
        for client in self.client_map:
            if client.closed:
                logger.info(f"Client closed {self.client_map[client]}")
                to_remove.append(client)
                for cb in self.client_closed_callbacks:
                    cb(client)
        for client in to_remove:
            del self.client_map[client]

    def stop(self):
        self.running=False
        self.join()

    def run(self):
        asyncio.run(self.run_server())

    async def send_heartbeat(self,ws):
        try:
            await ws.send("ACK?")
            await ws.recv()
        except websockets.exceptions.ConnectionClosed:
            self.__cleanup_clients()
            return False
        return True

    async def handle_message(self, websocket):
        await self.__try_register_client(websocket)
        self.__cleanup_clients()
        while self.running:
            message = False
            try:
                message = self.queue.get(timeout=1)
            except Exception as e:
                pass
            if message is not False:
                await self.__send_ws_message(websocket, message)
            still_connected = await self.send_heartbeat(websocket)
            if not still_connected:
                break
        logger.info("Closed Connection!")


    async def run_server(self):
        async with websockets.serve(self.handle_message, "0.0.0.0", 8765):
            await asyncio.Future()



