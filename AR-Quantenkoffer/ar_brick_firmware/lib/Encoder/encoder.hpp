#ifndef Encoder_h
#define Encoder_h
#include "Arduino.h" 

struct EncoderResult{
    uint8_t direction;
    uint8_t position;
    bool changed;
    bool pressed;
};

class Encoder {
public:
    Encoder(uint8_t clkPin, uint8_t dtPin, uint8_t swPin);
    void loop();
    void handleInterrupt();
    EncoderResult getResult();

private:
    uint8_t clkPin;
    uint8_t dtPin;
    uint8_t swPin;
    uint8_t dir;
    uint8_t position=0;
    uint8_t lastPosition=0;
    bool pressed;
    volatile bool handleLoop=false;

};
#endif