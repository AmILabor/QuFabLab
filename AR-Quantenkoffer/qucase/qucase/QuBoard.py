import time

import config
import RPi.GPIO as GPIO
import board
import busio
import logging

from .QuBrick import QuBrick

logger = logging.getLogger("QuBoard")


class QuBoard:
    _inputs = config.INPUTS
    _outputs = config.OUTPUTS
    _laser_start_button = config.LASER_START_INPUT
    _laser_fire_timeout = config.LASER_FIRE_TIMEOUT
    state_board: list[list[bool]]
    brick_board: list[list[QuBrick]]
    
    REQUIRED_BRICK_CONFIG = [
        {"x": 2, "y": 0, "rotation": 0, "type": 2},
        {"x": 2, "y": 2, "rotation": 1, "type": 0},
        {"x": 0, "y": 2, "rotation": 3, "type": 1},
        {"x": 2, "y": 4, "rotation": 2, "type": 1},
        {"x": 4, "y": 2, "rotation": 1, "type": 2},
        {"x": 4, "y": 0, "rotation": 3, "type": 2},
    ]

    def __init__(self):
        self.state_changed = False
        self.error = None
        self.i2c = None
        self.state_board = []
        self.brick_board = []
        self.laser_fired = False
        self.last_laser_fired = time.time()
        self.setup_complete = False
        self.error_state = False
        self.__setup_gpios()
        self.__setup_i2c()
        self.__setup_state_board()
        self.__setup_brick_board()
        self.brick_add_callbacks = []
        self.brick_remove_callbacks = []
        self.brick_update_callbacks = []
        self.start_laser_callbacks = []
        self.addresses = []
        logger.info("Board Setup Done!")

    def register_brick_add_callback(self, cb):
        self.brick_add_callbacks.append(cb)

    def register_brick_remove_callback(self, cb):
        self.brick_remove_callbacks.append(cb)

    def register_brick_update_callback(self, cb: object) -> object:
        self.brick_update_callbacks.append(cb)

    def register_start_laser_callback(self, cb):
        self.start_laser_callbacks.append(cb)

    def __setup_state_board(self):
        n_inputs = len(self._inputs)
        n_outputs = len(self._outputs)
        self.state_board = [[0 for __ in range(n_inputs)] for _ in range(n_outputs)]

    def __setup_brick_board(self):
        n_inputs = len(self._inputs)
        n_outputs = len(self._outputs)
        self.brick_board = [[None for __ in range(n_inputs)] for _ in range(n_outputs)]

    def __setup_gpios(self):
        GPIO.cleanup()
        GPIO.setmode(GPIO.BCM)
        GPIO.setup(QuBoard._laser_start_button, GPIO.IN, pull_up_down=GPIO.PUD_DOWN)
        GPIO.setup(16, GPIO.OUT)
        GPIO.output(16,GPIO.HIGH)
        logger.info("Setup GPIO 12 to auto high out")
        for _input in QuBoard._inputs:
            GPIO.setup(_input, GPIO.IN,pull_up_down=GPIO.PUD_DOWN)
        for output in QuBoard._outputs:
            GPIO.setup(output, GPIO.OUT)
        for led_pin in config.LED_GPIOS:
            GPIO.setup(led_pin, GPIO.OUT)

    def __setup_i2c(self):
        self.i2c = busio.I2C(board.SCL, board.SDA)
        devices = self.i2c.scan()
        if len(devices) > 0:
            logger.error("There are connected QuBricks on Startup")
            raise Exception("Please disconnect all QuBricks on startup!")

    def __scan_i2c_bus(self):
        retries = 5
        changed = []
        for _ in range(retries):
            time.sleep(0.1)
            address_changes = self.i2c.scan()
            changed = list(set(self.addresses).symmetric_difference(set(address_changes)))
            if len(changed) > 0:
                self.addresses = address_changes
                break
        return changed

    def __scan_start_laser(self):
        if time.time()-self.last_laser_fired < self._laser_fire_timeout:
            return
        registered = GPIO.input(QuBoard._laser_start_button)
        if registered != self.laser_fired and registered:
            self.last_laser_fired = time.time()
            logger.info("Calling Laser Firered Callbacks.")
            for cb in self.start_laser_callbacks:
                cb(registered)
        self.laser_fired = registered

    def _check_error_and_set_led(self) -> tuple[bool, bool]:
        """
        Check if there is any error on the board or bricks.
        Returns (error_detected, has_changed).
        Does not change GPIOs directly; caller should update LEDs when `has_changed` is True.
        """
        # Check for board-level error
        error_detected = self.error is not None

        # Check if any brick has a read error
        if not error_detected:
            for brick in self.get_bricks():
                if getattr(brick, "read_error", False):
                    error_detected = True
                    break

        has_changed = error_detected != getattr(self, "error_state", False)
        self.error_state = error_detected

        if error_detected:
            logger.warning("Error detected on board or bricks")
        # else:
        #     logger.info("No errors detected on board or bricks")

        return error_detected, has_changed
            
    def check_setup_complete(self) -> tuple[bool, list[str]]:
        """
        Check if all required QuBricks are in place with correct rotation and type.
        Returns (is_complete, has_changed).
        """
        errors: list[str] = []

        for req in self.REQUIRED_BRICK_CONFIG:
            try:
                brick = self.brick_board[req["x"]][req["y"]]
            except Exception:
                brick = None

            # Check if brick exists at position
            if brick is None:
                error_msg = f"No brick at position ({req['x']}, {req['y']})"
                # logger.warning(f"Setup incomplete: {error_msg}")
                errors.append(error_msg)
                continue

            # Check rotation
            if getattr(brick, "rotation", None) != req["rotation"]:
                error_msg = f"Brick at ({req['x']}, {req['y']}) has wrong rotation: expected {req['rotation']}, got {getattr(brick, 'rotation', None)}"
                # logger.warning(f"Setup incomplete: {error_msg}")
                errors.append(error_msg)

            # Check type
            if getattr(brick, "type", None) != req["type"]:
                error_msg = f"Brick at ({req['x']}, {req['y']}) has wrong type: expected {req['type']}, got {getattr(brick, 'type', None)}"
                # logger.warning(f"Setup incomplete: {error_msg}")
                errors.append(error_msg)

        is_complete = len(errors) == 0
        has_changed = is_complete != getattr(self, "setup_complete", False)
        self.setup_complete = is_complete

        # if is_complete:
        # #     logger.warning(f"Setup incomplete with {len(errors)} error(s): {errors}")
        # # else:
        #     logger.info("Setup check: all required bricks present and correct")

        # return is_complete, has_changed

    def add_qubrick(self, x, y) -> QuBrick:
        changed = self.__scan_i2c_bus()
        if len(changed) == 0:
            logger.error(f"Coud not add QuBrick at Position {x},{y} because no address has been found. Please remove the QuBrick you just placed")
            self.error = "Could not add QuBrick - no address found"
            return
        logger.info(f"Adding QuBrick at Position {x},{y} @{changed[0]}")
        self.brick_board[x][y] = QuBrick(self.i2c, changed[0], x, y)
        self.brick_board[x][y].fetch()
        #self.brick_board[x][y].store_settings_to_brick()
        self.state_board[x][y] = True

        for cb in self.brick_add_callbacks:
            cb(self.brick_board[x][y])
        return self.brick_board[x][y]

    def remove_qubrick(self, x, y):
        GPIO.output(config.LED_GPIOS[2], GPIO.LOW)
        if self.brick_board[x][y] is None:
            logger.error(f"Could not remove brick because there is no Brick at {x},{y}")
            self.error = "Could not remove brick - no brick at position"
            return
        brick_address = self.brick_board[x][y].address
        logger.info(f"Removing QuBrick at Position {x},{y} @ {brick_address}")
        if brick_address in self.addresses:
            self.addresses.remove(brick_address)
        for cb in self.brick_remove_callbacks:
            cb(self.brick_board[x][y])
        self.brick_board[x][y] = None
        self.state_board[x][y] = False

    def scan_for_new_participants(self):
        address_changes = self.i2c.scan()
        return len(self.addresses) < len(address_changes)

    def scan(self):
        self.__scan_start_laser()
        if not self.scan_for_new_participants():
            GPIO.output(config.LED_GPIOS[2], GPIO.HIGH)
            return
        GPIO.output(config.LED_GPIOS[2], GPIO.LOW) # Green LED to low when QuBrick is added
        logger.info("New Participant discovered via i2c. Scanning the matrix to determine its position.")
        for x, output_pin in enumerate(self._outputs):
            GPIO.output(output_pin, GPIO.HIGH)
            for y, input_pin in enumerate(self._inputs):
                time.sleep(0.005)
                last_state = self.state_board[x][y]
                current_state = GPIO.input(input_pin)
                if last_state != current_state:
                    logger.info(f"Adding Brick because {last_state} != {current_state} at {x},{y} ({output_pin},{input_pin})")
                    if current_state:
                        self.add_qubrick(x, y)
            GPIO.output(output_pin, GPIO.LOW)
        # Note: error and setup checks are handled in update_bricks only
    
    def update_bricks(self):
        rows = len(self.brick_board)
        for x in range(rows):
            for y in range(len(self.brick_board[x])):
                current_brick = self.brick_board[x][y]
                if current_brick is None:
                    continue

                updated = current_brick.fetch()
                if current_brick.read_error:
                    self.error = f"Brick read error at {x},{y}"
                    self.remove_qubrick(x, y)
                    continue
                if updated:
                    for cb in self.brick_update_callbacks:
                        cb(current_brick)
            
        # Clear error if no bricks have errors
        if self.error and self.error.startswith("Brick read error"):
            self.error = None
        # Check error state and update red LED only when status changed
        error_detected, error_changed = self._check_error_and_set_led()
        if error_changed:
            red_led = config.LED_GPIOS[0]
            GPIO.output(red_led, GPIO.HIGH if error_detected else GPIO.LOW)
            if error_detected:
                logger.warning("Error detected. Red LED enabled.")
            else:
                logger.info("Errors cleared. Red LED disabled.")

        # Update setup-complete status and set yellow LED only when status changed
        is_complete, has_changed = self.check_setup_complete()
        if has_changed:
            yellow_led = config.LED_GPIOS[1]
            GPIO.output(yellow_led, GPIO.HIGH if is_complete else GPIO.LOW)
            if is_complete:
                logger.info("Setup complete! Yellow LED enabled.")
            else:
                logger.warning("Setup no longer complete. Yellow LED disabled.")

    def get_bricks(self) -> list[QuBrick]:
        r = []
        for x in range(len(self.brick_board)):
            row = self.brick_board[x]
            for y in range(len(row)):
                if row[y] is not None:
                    r.append(row[y])
        return r

    def get_brick(self, x, y) -> QuBrick:
        return self.brick_board[x][y]
