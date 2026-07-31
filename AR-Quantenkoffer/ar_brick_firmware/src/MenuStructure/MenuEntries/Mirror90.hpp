/**
 * @file Mirror90.hpp
 * @brief Menüeintrag für den 90-Grad-Spiegel-Baustein.
 * 
 * Zeichnet eine schematische Darstellung eines 90°-Spiegels mit
 * einstellbarem Abstandsparameter auf dem TFT-Display.
 */

#include "MenuStructure/MenuEntries/menuEntry.hpp"

/// Menüeintrag für den 90°-Spiegel mit Abstandseinstellung.
class Mirror90Entrie: public MenuEntry{
    public:
        Mirror90Entrie(Adafruit_GC9A01A * display) : MenuEntry(display){};
        /// Zeichnet den 90°-Spiegel mit Laserstrahl und Abstandsmarkierungen.
        void render(){
            display->fillRoundRect(xBgStart,yBgStart,bgWidth,bgHeight,bgRadius,bgBlue);
            display->fillRect(xBgCenter,yBgCenter-(laserBarThickness/2),laserBarLength,laserBarThickness,red);  // Laserbeam center to right
            display->fillRoundRect(whiteBar1X,whiteBarY,whiteBarWidth,whiteBarHeight,whiteBarRadius,white); // first white bar
            display->fillRoundRect(whiteBar2X-settingValue*2,whiteBarY,whiteBarWidth,whiteBarHeight,whiteBarRadius,white_opaque); // second gray bar
            DrawHeadline(headline);
            DrawSettings(settingName+": "+String(settingValue/10.0f));
        }
        /// Gibt den Abstandswert zurück.
        int getSetting(){return settingValue;};
        /// Setzt den Abstandswert.
        void setSetting(int value){
            //if(value < -10) value=-10;
            //else if(value > 10) value=10;
            settingValue = value;

            };
    protected:
        String headline ="90 Spiegel";
        String settingName = "Abstand";
        int settingValue = 0;

        uint8_t laserBarThickness = 15;
        uint8_t laserBarLength = xBgCenter-xBgStart;
        uint8_t whiteBarWidth = 15;
        uint8_t whiteBarSpacing = 10;
        uint8_t whiteBarRadius = 5;
        uint8_t whiteBarHeight = (bgHeight-yBgStart)*0.8; 
        uint8_t whiteBarY = yBgStart+((bgHeight-whiteBarHeight)/2);
        uint8_t whiteBar1X = xBgCenter;
        uint8_t whiteBar2X = xBgCenter-whiteBarWidth-whiteBarSpacing;
};