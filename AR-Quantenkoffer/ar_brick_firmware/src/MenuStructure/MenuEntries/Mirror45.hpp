/**
 * @file Mirror45.hpp
 * @brief Menüeintrag für den 45-Grad-Spiegel-Baustein (ohne Einstellungen).
 * 
 * Zeichnet einen 45°-Spiegel im TFT-Display. Dieser Baustein hat
 * keine konfigurierbaren Einstellungen.
 */

#include "MenuStructure/MenuEntries/menuEntry.hpp"

/// Menüeintrag für den 45°-Spiegel (keine Einstellungen).
class Mirror45Entrie: public MenuEntry{
    public:
        Mirror45Entrie(Adafruit_GC9A01A * display) : MenuEntry(display){};
        /// Zeichnet den 45°-Spiegel mit diagonaler Linie und Laserstrahlen.
        void render(){
            display->fillRoundRect(xBgStart,yBgStart,bgWidth,bgHeight,bgRadius,bgBlue); // Draw Background
            //display->fillRect(xBgStart,yBgCenter-(laserBarThickness/2),laserBarLength,laserBarThickness,red);  // Laserbeam center to right
            display->fillRect(xBgCenter,yBgCenter-(laserBarThickness/2),laserBarLength,laserBarThickness,red);  // Laserbeam center to right
            display->fillRect(xBgCenter-(laserBarThickness/2),yBgCenter,laserBarThickness,laserBarLength,red); // Laserbeam center to bottom
            DrawAngledLine(boxStart,boxStart+boxSize,boxStart+boxSize,boxStart,mirror_width,white); // diagonal mirror
            DrawHeadline(headline);
        }
        /// Hat keine Einstellungen (gibt NO_SETTINGS zurück).
        int getSetting(){return MenuEntry::NO_SETTINGS;};
        /// Setzen der Einstellung wird ignoriert.
        void setSetting(int value){};
    protected:
        String headline="45 Spiegel";
        String settingName = "";
        int settingValue = 0;
        uint8_t boxSize = 70;
        uint8_t boxStart = xBgStart + (bgHeight - boxSize)/2; 
        uint8_t mirror_width = 15;
        uint8_t laserBarThickness = 15;
        uint8_t laserBarLength = xBgCenter-xBgStart;
};