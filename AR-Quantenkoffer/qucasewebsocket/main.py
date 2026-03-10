from QuCase import QuCaseTester
from time import sleep

if __name__=="__main__":
    case  = QuCaseTester(sleep_inbetween=5) # Control how many seconds between the testcase steps are
    # Just prevent that the program doesnt exit
    while True:
        while case.awaiting_connection:
            sleep(0.005)
        print("Starting Test Because a client has connected!")
        while not case.test_done():
            sleep(1)
        print("Test cases done!")
    case.stop()
