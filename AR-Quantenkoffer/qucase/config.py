"""
Konfigurationsdatei für QuCase.

Definiert GPIO-Pin-Zuordnungen und I2C-Registeradressen
für die Spielfeld-Matrix des QuBoard.
"""

# QuFabLab qucase configuration
# Date: 2026-07-31
# GPIO pin assignments and I2C register addresses for the QuBoard field matrix.

# Output (Rows)
OUTPUTS = [14,15,18,17,27,23]#, GPIO_ROW2]
INPUTS = [24,8,21,20,16]
LED_GPIOS = [5,22, 26] # LEDs 
LASER_START_INPUT = 13
LASER_FIRE_TIMEOUT = 1.5

#Registers
TYPE_REGISTER  = 0x10
SETTING_REGISTER0 = 0x11
SETTING_REGISTER1 = 0x12
ROTATION_REGISTER = 0x13
STORE_REGISTER=0x14