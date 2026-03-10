import asyncio

from websockets.sync.client import connect
import time

timeout_to_exit = 10
last_message = time.time()
with connect("ws://localhost:8765") as ws:
    while True:
        resp = None
        resp = ws.recv()
        if resp=="ACK?":
            ws.send("ACK")
            ack_count+=1
        else:
            print(resp)
            ack_count=0
        if ack_count > 10:
            break

