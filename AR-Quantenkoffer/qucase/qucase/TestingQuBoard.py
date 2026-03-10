import config
import logging

from .QuBoard import QuBoard
from .QuBrick import QuBrick

logger = logging.getLogger("QuBoard")


class TestingQuBoard(QuBoard):
    _inputs = [0]*11
    _outputs = [0]*8
    state_board: list[list[bool]]
    brick_board: list[list[QuBrick]]

    def __init__(self):
        self.state_changed = False
        self.error = None
        self.i2c = None
        self.state_board = []
        self.brick_board = []
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

    def register_brick_add_callback(self,cb):
        self.brick_add_callbacks.append(cb)

    def register_brick_remove_callback(self,cb):
        self.brick_remove_callbacks.append(cb)

    def register_brick_update_callback(self, cb: object) -> object:
        self.brick_update_callbacks.append(cb)

    def register_start_laser_callback(self,cb):
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
        pass

    def __setup_i2c(self):
        pass

    def add_qubrick(self, x, y,rotation,type) -> QuBrick:
        retries =5

        logger.info(f"Adding QuBrick at Position {x},{y} @{0x00}")
        self.brick_board[x][y] = QuBrick(self.i2c,0x00,x,y)
        self.brick_board[x][y].rotation = rotation
        self.brick_board[x][y].type = type

        self.state_board[x][y]= True

        for cb in self.brick_add_callbacks:
            cb(self.brick_board[x][y])
        return self.brick_board[x][y]

    def remove_qubrick(self, x, y):
        logger.info(f"Removing QuBrick at Position {x},{y} @ {0x00}")
        for cb in self.brick_remove_callbacks:
            cb(self.brick_board[x][y])
        self.brick_board[x][y]=None
        self.state_board[x][y]= False


    def scan_for_new_participants(self):
        address_changes = self.i2c.scan()
        return len(self.addresses) < len(address_changes)


    def __scan_start_laser(self):
        registered = True
        for cb in self.start_laser_callbacks:
            cb(registered)

    def scan(self):
        self.__scan_start_laser()

    def update_bricks(self):
        rows = len(self.brick_board)
        for x in range(rows):
            for y in range(len(self.brick_board[x])):
                current_brick = self.brick_board[x][y]
                if current_brick is None:
                    continue

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

    def get_brick(self,x,y) -> QuBrick:
        return self.brick_board[x][y]



