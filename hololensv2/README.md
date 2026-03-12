# HoloLens 2 

This project contains the Unity application used for augmented reality visualization of experiments from the AR Quantenkoffer system in the QuFabLab project.
The application runs on Microsoft HoloLens 2 and visualizes optical experiments in real time while users build the experiment physically using the AR Quantenkoffer hardware.

---

# Overview

The HoloLens application connects to the QuCase backend running on a Raspberry Pi and receives experiment configuration updates.
Based on this configuration, the application renders a holographic representation of the optical experiment.

This allows users to:

* build experiments using physical components
* observe experiment behavior in augmented reality
* explore quantum optics concepts interactively

---

# Features

- Augmented reality visualization of optical experiments
- Real-time updates from the experiment hardware
- Spatial interaction using HoloLens
- Visualization of experiment components such as:
  - mirrors
  - beam splitters
  - detectors
  - optical paths
  - photon simulations

---

# Technology Stack

* Unity
* Mixed Reality Toolkit (MRTK)
* WebSocket communication
* Microsoft HoloLens 2

---

# Project Structure

```
Assets/
Unity assets including models, scripts, prefabs, and scenes

Prefabs/
AR representations of experiment components

Scripts/
Application logic

Resources/
Experiment data and additional assets
```

---

# Communication Architecture

```
Experiment Bricks
      ↓
Experiment Field
      ↓
QuCase Backend (Raspberry Pi)
      ↓ WebSocket
HoloLens 2 AR Application
```

The application listens for experiment configuration updates and renders the corresponding optical system.

---

# Development

## Requirements

* Unity (recommended LTS version)
* Mixed Reality Toolkit (MRTK)
* Windows with HoloLens development tools
* Microsoft HoloLens 2

## Build Target

```
Universal Windows Platform (UWP)
```

Target device:

```
Microsoft HoloLens 2
```

---

# Contributing

Contributions are welcome. Possible improvements include:

* new experiment visualizations
* improved AR interaction
* performance optimization
* additional optical components

---

# License

See the project license for details.
