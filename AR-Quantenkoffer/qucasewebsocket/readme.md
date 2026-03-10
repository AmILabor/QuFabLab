# Websocket-Server-Dummy for QuantumCase dummy

- **main.py** is the server application 
- currently it waits until 
a client is connected and sends the configured testcase (QuCase.QuCaseTester)
- then the server waits for a new client connection so reconnecting to the server is necessary 
  to get a new set of testcases
- Enums to parse the messages (type and rotation are in **enums.py**)
- general messageformat is:
  - {"command": "place", "posX": 8, "posY": 0, "rotation": "W", "type": 1}
  - {"command": "setting", "posX": 0, "posY": 4, "value": 0.25}
  - {"command": "remove", "posX": 4, "posY": 0}
