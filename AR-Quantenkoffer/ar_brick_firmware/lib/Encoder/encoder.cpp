#include "Encoder.hpp"

Encoder * clsPointer;

void outsideInterruptHandler(){
    clsPointer->handleInterrupt();
}

Encoder::Encoder(uint8_t clkPin, uint8_t dtPin, uint8_t swPin) {
    this->clkPin = clkPin;
    this->dtPin = dtPin;
    this->swPin = swPin;
    clsPointer=this;
    pinMode (this->clkPin,INPUT_PULLUP);
    pinMode (this->dtPin,INPUT_PULLUP);
    pinMode (this->swPin,INPUT_PULLUP);
    digitalWrite(this->clkPin, true);
    digitalWrite(this->dtPin, true);
    digitalWrite(this->swPin, true);
    attachInterrupt(digitalPinToInterrupt(this->dtPin),outsideInterruptHandler,CHANGE);
    attachInterrupt(digitalPinToInterrupt(this->clkPin),outsideInterruptHandler,CHANGE);
    attachInterrupt(digitalPinToInterrupt(this->swPin),outsideInterruptHandler,CHANGE);
}
void Encoder::loop(){
        if(this->handleLoop){
            // Reset the flag that brought us here (from ISR)
            this->handleLoop = false;

            static uint8_t lrmem = 3;
            static int lrsum = 0;
            static int8_t TRANS[] = {0, -1, 1, 14, 1, 0, 14, -1, -1, 14, 0, 1, 14, 1, -1, 0};

     
            // Read BOTH pin states to deterimine validity of rotation (ie not just switch bounce)
            int8_t l = digitalRead(this->clkPin);
            int8_t r = digitalRead(this->dtPin);

            // Read Switch-Pin and check if it is pulled low (then we can abort further processing)
            bool pressed = digitalRead(this->swPin)==0;
            if(pressed){
                this->pressed = 1;
                return;
            }
            // Move previous value 2 bits to the left and add in our new values
            lrmem = ((lrmem & 0x03) << 2) + 2 * l + r;

            // Convert the bit pattern to a movement indicator (14 = impossible, ie switch bounce)
            lrsum += TRANS[lrmem];

            /* encoder not in the neutral (detent) state */
            if (lrsum % 4 != 0)
            {
                this->dir=-1;
                return;
            }

            /* encoder in the neutral state - clockwise rotation*/
            if (lrsum == 4)
            {
                lrsum = 0;
                this->position+=1;
                this->dir=1;
                return;
            }

            /* encoder in the neutral state - anti-clockwise rotation*/
            if (lrsum == -4)
            {
                lrsum = 0;
                this->position-=1;
                this->dir=0;
                return;
            }

            // An impossible rotation has been detected - ignore the movement
            lrsum = 0;
            this->dir=-1;
        }
}
EncoderResult Encoder::getResult(){
    EncoderResult er;
    boolean changed = this->lastPosition != this->position || this->pressed;
    if(changed){
        this->lastPosition=this->position;
    }
    er.changed = changed;
    er.position = this->position;
    er.direction = this->dir;
    er.pressed=this->pressed;
    this->pressed=0;
    return er;
}

void Encoder::handleInterrupt(){
    this->handleLoop=true;
}
