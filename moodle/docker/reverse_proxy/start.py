from time import sleep
import subprocess

while 1:
    sleep(1)
    subprocess.call(["/usr/sbin/httpd"])
    print(".")