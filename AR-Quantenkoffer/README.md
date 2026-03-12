# AR Quantenkoffer

The **AR Quantenkoffer (Quantum Suitcase)** is a modular experiment platform designed to teach quantum optics concepts using physical experiment components combined with augmented reality visualization.

It allows users to build optical experiments with physical blocks while the system detects their configuration and visualizes the experiment in AR.

---

## System Overview

The system consists of three main parts:

1. **Experiment Field**
   - Detects the position of experiment blocks
   - Provides power and communication

2. **Experiment Blocks ("Bricks")**
   - Represent optical components (mirrors, beam splitters, etc.)
   - Contain a microcontroller and display
   - Communicate with the main system via I2C

3. **Control Software**
   - Runs on a Raspberry Pi
   - Detects experiment configuration
   - Communicates with the AR application

---

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


---

## Hardware Components

The system includes:

- experiment bricks with microcontrollers
- rotary encoder interface
- display modules
- custom PCBs
- 3D printed mechanical parts

---

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


---

## Usage

1. Insert experiment bricks into the experiment field
2. Wait until the brick has booted and its type appears on the display
3. The system detects the brick position and type
4. The configuration is transmitted to the AR application

---

## Educational Purpose

The system is designed for experiments such as:

- interferometers
- beam splitters
- mirrors
- quantum optics demonstrations

---

## License

See individual subprojects for licensing details.