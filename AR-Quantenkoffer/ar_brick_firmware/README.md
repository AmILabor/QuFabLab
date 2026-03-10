# AR Brick Firmware

Firmware for the QuBricks used in the AR Quantenkoffer system.

Each brick contains a microcontroller and represents an optical component such as mirrors or beam splitters.

The firmware controls the brick display, detects rotation, and communicates with the main system via I2C.


## Architecture
~~~
Brick Hardware
│
├── Microcontroller (XIAO SAMD21)
├── Rotary Encoder
├── Display
└── GPIO Expander (PCF8574)
~~~



## Repository Structure

~~~
lib/
encoder library for the rotary encoder

lib/pcf8574AN
library for the I2C GPIO expander

src/
main firmware implementation

src/MenuStructure
menu system for brick configuration
~~~



## Functionality

When a brick is inserted into the experiment field:

1. The brick boots and initializes its hardware.
2. The Raspberry Pi scans the I2C bus.
3. When a new device appears, its position is stored.
4. The brick communicates its type and configuration.

Important:

**Wait until the brick has finished booting before inserting another one.**

Otherwise the system cannot correctly determine the brick position.


## Menu System

The brick includes a rotary encoder and menu system.

Functions:

- change brick type
- adjust component parameters
- access configuration settings

Example:

- change between different optical components
- fine adjustment for mirror angle



## Hardware

Main components:

- XIAO SAMD21 microcontroller
- PCF8574AN GPIO expander
- 1.28" LCD display
- Rotary Encoder (KY-040)
- pogo pin connectors



## Development


The firmware is built using PlatformIO.
~~~
platformio run
~~~

Upload firmware:

~~~
platformio run --target upload
~~~



## License

See project license.
