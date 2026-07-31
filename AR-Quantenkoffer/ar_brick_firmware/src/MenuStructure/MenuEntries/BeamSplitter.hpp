/**
 * @file BeamSplitter.hpp
 * @brief Menüeintrag für den Strahlteiler-Baustein (ohne Einstellungen).
 * 
 * Zeichnet einen Strahlteiler im TFT-Display. Dieser Baustein hat
 * keine konfigurierbaren Einstellungen.
 */

#include "MenuStructure/MenuEntries/menuEntry.hpp"

/// Menüeintrag für den Strahlteiler (keine Einstellungen).
class BeamSplitterEntrie: public MenuEntry{
    public:
        BeamSplitterEntrie(Adafruit_GC9A01A * display) : MenuEntry(display){};
        /// Zeichnet den Strahlteiler mit ein-/ausgehenden Laserstrahlen und Mittellinie.
        void render(){
            display->fillRoundRect(xBgStart,yBgStart,bgWidth,bgHeight,bgRadius,bgBlue);
            display->fillRect(xBgStart,yBgCenter-(laserBarThickness/2),laserBarLength,laserBarThickness,red);  // Laserbeam left to center
            display->fillRect(xBgCenter-(laserBarThickness/2),yBgCenter,laserBarThickness,laserBarLength,red); // Laserbeam center to bottom
            display->fillRect(xBgCenter,yBgCenter-(laserBarThickness/2),laserBarLength,laserBarThickness,red);   // laserbeam center to right
            display->fillRoundRect(boxStart,boxStart,boxSize,boxSize,boxRadius,white); // White Round square
            DrawAngledLine(boxStart,boxStart+boxSize,boxStart+boxSize,boxStart,centerLineWidth,bgBlue); // centerLine
            DrawHeadline(headline);
        }
        /// Hat keine Einstellungen.
        int getSetting(){return MenuEntry::NO_SETTINGS;};
        /// Setzen der Einstellung wird ignoriert.
        void setSetting(int value){};
    protected:
        String headline="Strahlteiler";
        String settingName = "";
        int settingValue;
        uint8_t boxSize = 70;
        uint8_t boxRadius = 5;
        uint8_t centerLineWidth = 10;
        uint8_t laserBarThickness = 15;
        uint8_t laserBarLength = xBgCenter-xBgStart;
        uint8_t boxStart = xBgStart + (bgHeight - boxSize)/2; 
};