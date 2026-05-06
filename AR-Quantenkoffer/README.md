# AR Quantenkoffer

The AR Quantenkoffer (Quantum Suitcase) is a modular experiment platform designed to teach physics concepts using physical experiment components combined with augmented reality visualization.

## Installation



## System Overview

The system consists of three main parts:

1. Experiment Field
   - Detects the position of experiment bricks
   - Provides power and communication

2. Experiment "QuBricks" 
   - Represent optical components (mirrors, beam splitters, etc.)
   - Contain a microcontroller and display
   - Communicate with the main system via I2C

3. Control Software
   - Runs on a Raspberry Pi
   - Detects experiment configuration
   - Communicates with the AR application


## Repository Structure
~~~
AR-Quantenkoffer
│
├── ar_brick_firmware
│ Firmware for the experiment bricks
│
├── qucase
│ Python control software running on the main system
│
├── Platinen
│ PCB designs for the hardware
│
└── stls
3D models for mechanical components
~~~


## Hardware Components

### Field
To print the 3D components, we used an Ultimaker S5.
Each field element consists of the follwing componenets:
  - a custom pcb to detect brick position
  - a pcb holder
  - a pcb cover plate, which convers the pcb and holds magnets to hold the QuBricks in place. 

The complete field contains 30 elements configured as 6 elements wide and 5 elements long. Each field element can hold one QuBrick. The STL-file for the pcb holder field is 3x5 elements and needs to be printed twice. The pcb cover plate needs to be printed 30 times (or 30 dived by the how many you can fit on your printer). 
The Raspberry Pi is connected to the field using the also 3D-printed "pi mount".

### QuBricks

The QuBricks consist of:
- The brick bottom, which contain the magnets that hold the QuBricks in place.
- The brick shell 
- The display fixture, which holds the display.

For the Michelson-Interferometer 6 QuBricks are needed.

The non-3D-printed compontents inside the Brick shell are:

- Rotary Encoder Board KY-40 to set the brick type and mirror distance
- the slef-designed board to which the following elements are added:
  - XIAO SAMD21 microcontroller 
  - PCF8574AN GPIO expander
  - DEBO LCD 1.28" Display 
  - pogo pin connectors
  - 3x 10k Resistor
 


## Software Componenets



### QuBrick Firmware



### qucase (Rapsberry Pi Software)



## Communication Architecture
~~~
Experiment Brick
↓
I2C Communication
↓
Raspberry Pi (qucase)
↓
WebSocket
↓
AR Application (HoloLens)
~~~



## Usage

1. Insert experiment bricks into the experiment field
2. Wait until the brick has booted and its type appears on the display
3. The system detects the brick position and type
4. The configuration is transmitted to the AR application

## License

See individual subprojects for licensing details.