import os
import time
import logging
from typing import Callable, Optional
import RPi.GPIO as GPIO

REGISTRATION_TIMEOUT = 1.0  # seconds - adjust as needed
logger = logging.getLogger("QuBoard")


def led_on(led_pin):
    GPIO.output(led_pin, GPIO.HIGH)


def led_off(led_pin):
    GPIO.output(led_pin, GPIO.LOW)


def handle_brick_timer(board, led_pin, brick_placed_time, previous_brick_count):
    """
    Handles timing and registration of bricks.

    Returns:
        tuple: (brick_placed_time, previous_brick_count)
    """
    # Get current brick count
    current_bricks = board.get_bricks()
    current_brick_count = len(current_bricks)

    # Detect NEW brick placed
    if current_brick_count > previous_brick_count:
        brick_placed_time = time.time()
        led_on(led_pin)
        logger.info(f"Brick placed! Timer started ({REGISTRATION_TIMEOUT}s)")

    # Detect brick REMOVED
    elif current_brick_count < previous_brick_count:
        brick_placed_time = None
        led_off(led_pin)  # FIXED: now passes led_pin
        logger.info("Brick removed - timer reset")

    # Check if timer has elapsed
    if brick_placed_time is not None:
        elapsed = time.time() - brick_placed_time
        if elapsed >= REGISTRATION_TIMEOUT:
            # Register all unregistered bricks
            for brick in current_bricks:
                if not brick.registered:
                    brick.register()  # Change to your actual method
                    logger.info(f"Brick registered at ({brick.x}, {brick.y})")

            # Reset timer
            brick_placed_time = None
            led_off(led_pin)  # FIXED: now passes led_pin
            logger.info("Brick(s) registered - LED off")

    return brick_placed_time, current_brick_count



def elapsed(start: Optional[float], timeout: float, led_on: Callable[[], None], led_off: Callable[[], None],) -> bool:
    """Return True when ``timeout`` seconds have passed since ``start``."""
    if start is None:
        led_off()
        return False
    if time.time() - start >= timeout:
        led_off()
        return True
    led_on()
    return False


def generate_connect_qrcode(text: str):
    import io
    import qrcode
    text = text.strip()
    qr = qrcode.QRCode(version=4, box_size=2, border=2)
    qr.add_data(text)
    f = io.StringIO()
    qr.print_ascii(out=f,invert=True)
    f.seek(0)
    lines = f.readlines()
    _lines = ""
    for l in lines:
        _lines+=f"\t\t\t   {l}"
    return "\n"+_lines+ f"\n\t\t\t\t    {text}    "


def get_ip_port():
    ipv4 = os.popen(
        'ip addr show wlan0 | grep "\<inet\>" | awk \'{ print $2 }\' | awk -F "/" \'{ print $1 }\'').read().strip()
    return ipv4, 8123
