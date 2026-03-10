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

    def __init__(self):
        self.state_changed = False
        self.error = None
        self.i2c = None
        self.state_board = []
        self.brick_board = []
        self.laser_fired = False
        self.last_laser_fired = time.time()
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
        for _input in QuBoard._inputs:
            GPIO.setup(_input, GPIO.IN,pull_up_down=GPIO.PUD_DOWN)
        for output in QuBoard._outputs:
            GPIO.setup(output, GPIO.OUT)

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



    def add_qubrick(self, x, y) -> QuBrick:
        changed = self.__scan_i2c_bus()
        if len(changed) == 0:
            logger.error(f"Coud not add QuBrick at Position {x},{y} because no address has been found.")
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
        if self.brick_board[x][y] is None:
            logger.error(f"Could not remove brick because there is no Brick at {x},{y}")
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
            return
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

    def update_bricks(self):
        rows = len(self.brick_board)
        for x in range(rows):
            for y in range(len(self.brick_board[x])):
                current_brick = self.brick_board[x][y]
                if current_brick is None:
                    continue

                updated = current_brick.fetch()
                if current_brick.read_error:
                    self.remove_qubrick(x, y)
                    continue
                if updated:
                    for cb in self.brick_update_callbacks:
                        cb(current_brick)

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
