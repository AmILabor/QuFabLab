# QuFabLab

### Open Quantum Experiments for FabLabs and Education

QuFabLab is an open-source platform for building and exploring interactive quantum optics experiments using modular hardware, embedded systems, and augmented reality.

The project combines physical experiment components with real-time AR visualization, allowing students to build optical experiments while simultaneously observing the quantum behavior of the system.

QuFabLab aims to make quantum technologies accessible in FabLabs, universities, and STEM education environments.

---

# Project Overview

QuFabLab is centered around the AR Quantenkoffer (Quantum Suitcase) — a portable experimental platform that enables users to construct optical experiments using modular components.

The system automatically detects the configuration of the experiment and visualizes it using Microsoft HoloLens 2.

This enables a new form of learning where physical experimentation and digital visualization are combined.

---

# System Architecture

```id="g82x1a"
Experiment Bricks
      │
      │  I2C / GPIO
      ▼
Experiment Field
      │
      ▼
Raspberry Pi (QuCase Backend)
      │
      │ WebSocket
      ▼
AR Visualization (HoloLens 2)
```

1. Users place experiment bricks on the experiment field
2. The Raspberry Pi backend detects the configuration
3. The AR application visualizes the optical system

---

# Repository Structure

```id="9afwks"
QuFabLab
│
├── AR-Quantenkoffer
│   │
│   ├── ar_brick_firmware
│   │   Firmware for the experiment bricks
│   │
│   ├── qucase
│   │   Raspberry Pi control software
│   │
│   ├── Platinen
│   │   PCB designs for the hardware
│   │
│   └── stls
│       3D printable mechanical components
│
├── hololens2
│   Augmented reality visualization application
│
└── moodle
    Educational materials and course integration
```

---

# Core Components

## AR Quantenkoffer

A modular hardware system that allows users to build optical experiments using physical experiment blocks.

Features:

* experiment brick detection
* hardware interface for optical components
* power and communication infrastructure
* modular optical components

---

## Experiment Brick Firmware

Firmware running on the experiment bricks.

Responsibilities include:

* detecting brick orientation
* communicating with the backend via I2C
* displaying brick type and configuration
* providing user configuration via rotary encoder

---

## QuCase Backend

The backend software running on a Raspberry Pi.

Responsibilities:

* detecting experiment bricks
* managing experiment configuration
* communicating with the AR visualization system
* providing APIs for external applications

---

## HoloLens AR Visualization

The HoloLens application displays the experiment in augmented reality.

It visualizes components such as:

* mirrors
* beam splitters
* detectors
* photon paths
* interferometers

---

## Moodle Integration

The project includes educational content designed for structured learning environments.

The Moodle integration provides:

* experiment instructions
* theoretical background
* assignments and exercises
* guided learning modules

---

# Example Experiments

The system can be used to demonstrate concepts such as:

* beam splitting
* interferometry
* photon detection
* optical path interference
* quantum optics fundamentals

---

# Educational Goals

QuFabLab aims to:

- make quantum physics accessible through hands-on experiments
- combine physical experimentation with AR visualization
- provide open hardware for STEM education
- support universities, FabLabs, and makerspaces

---

# Target Audience

* universities
* FabLabs / makerspaces
* physics education programs
* STEM outreach initiatives

---

# Development

The project includes multiple development environments:

| Component      | Technology                  |
| -------------- | --------------------------- |
| Firmware       | C++ / PlatformIO            |
| Backend        | Python                      |
| AR Application | Unity / MRTK                |
| Hardware       | PCB + 3D printed components |

---

# Getting Started

Clone the repository:

```id="n8y5dl"
git clone https://github.com/AmILabor/QuFabLab.git
```

Explore the subprojects:

* `AR-Quantenkoffer`
* `hololens2`
* `moodle`

Each directory contains its own documentation.

---

# Contributing

Contributions are welcome.

Possible contributions include:

* hardware improvements
* firmware development
* AR visualization features
* educational content
* documentation

---

# Research & Educational Use

QuFabLab is designed for use in:

* physics laboratory courses
* STEM education programs
* FabLab workshops
* science outreach events

---

# License

See the individual project folders for license details.

---

# Acknowledgements

QuFabLab is an open initiative to bring quantum technologies into accessible learning environments through open hardware, software, and augmented reality.
