/**
 * @file encoder.hpp
 * @brief Drehgeber-Bibliothek für den Quantenkoffer-Baustein.
 * 
 * Dekodiert die Signale eines Drehgebers (CLK/DT/SW) mit Interrupts
 * und stellt Richtung, Position und Tastendruck bereit.
 */

#ifndef Encoder_h
#define Encoder_h
#include "Arduino.h" 

/// Ergebnis einer Drehgeber-Abfrage mit Richtung, Position und Tastendruck.
struct EncoderResult{
    uint8_t direction;
    uint8_t position;
    bool changed;
    bool pressed;
};

class Encoder {
public:
    /// Erzeugt den Encoder und bindet Interrupts für CLK, DT und SW.
    Encoder(uint8_t clkPin, uint8_t dtPin, uint8_t swPin);
    /// Muss regelmäßig aufgerufen werden; verarbeitet die Interrupt-Flags.
    void loop();
    /// Wird vom Interrupt-Handler aufgerufen; setzt ein Flag für loop().
    void handleInterrupt();
    /// Gibt das aktuelle Ergebnis (Richtung, Position, Tastendruck) zurück.
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