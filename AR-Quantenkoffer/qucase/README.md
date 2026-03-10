# QuCase

QuCase is the control software for the AR Quantenkoffer system.

It runs on a Raspberry Pi and manages communication between the experiment hardware and external applications such as the AR visualization system.

---

## Features

- Detect experiment bricks
- Manage experiment configuration
- Handle hardware communication
- Provide WebSocket interface for external applications
- Logging and debugging tools

---

## Architecture
~~~
Experiment Field
↓
Brick Detection
↓
QuCase (Python Backend)
↓
WebSocket Server
↓
AR Application
~~~

## Project Structure
~~~
qucase/
core classes for experiment control

utility/
helper utilities and command handling

websocket_communication/
websocket server implementation
~~~


Main classes include:

- QuBoard – represents the experiment field
- QuBrick – represents an experiment brick
- CommandFactory – handles command generation


## Installation

Install dependencies:
~~~
pip install -r requirements.txt
~~~


Start the application:
~~~
python main.py
~~~


---

## WebSocket Interface

The WebSocket server allows external applications (such as the AR system) to receive updates about the experiment configuration.

---

## Development

The project is written in Python and structured to allow modular extensions.

---

## License

See project license.



