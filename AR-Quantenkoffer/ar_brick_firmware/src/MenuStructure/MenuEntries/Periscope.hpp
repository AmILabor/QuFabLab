#include "MenuStructure/MenuEntries/menuEntry.hpp"
  /**
   *    1   3
   *       /|
   *    2 / |
   *     |  |
   *     |  |
   *     | / 2
   *     |/
  *      1   3
   */
class PeriscopeEntrie: public MenuEntry{
    public:
        PeriscopeEntrie(Adafruit_GC9A01A * display) : MenuEntry(display){};
        void render(){
            display->fillRoundRect(xBgStart,yBgStart,bgWidth,bgHeight,bgRadius,bgBlue); // Draw Background
            display->fillRect(xBgStart,yBoxStart+boxHeight-laserSpacing-laserBarThickness,laserBarLength,laserBarThickness,red);  // Laserbeam left to center
            display->fillRect(xBgCenter,yBoxStart+laserSpacing,laserBarLength,laserBarThickness,red);   // laserbeam center to right
            display->fillRoundRect(xBoxStart,yBoxStart,boxWidth,boxHeight,boxRadius,white); // White Round square
            display->fillTriangle(xBoxStart,yBoxStart,xBoxStart,yBoxStart+triangleYSpacing,xBoxStart+boxWidth-boxRadius,yBoxStart,bgBlue); // Top blue triangle
            display->fillTriangle(xBoxStart+boxRadius,yBoxStart+boxHeight,xBoxStart+boxWidth,yBoxStart+boxHeight-triangleYSpacing,xBoxStart+boxWidth,yBoxStart+boxHeight,bgBlue); // bottom blue triangle
            DrawHeadline(headline);
        }
        int getSetting(){return MenuEntry::NO_SETTINGS;};
        void setSetting(int value){};
    protected:
        String headline ="Periskop";
        String settingName = "";
        int settingValue = 0;
        uint8_t laserSpacing = 15;
        uint8_t laserBarThickness = 15;
        uint8_t boxRadius = 5;
        uint8_t boxWidth = bgWidth*0.2;
        uint8_t boxHeight = bgHeight*0.7;
        uint8_t laserBarLength = xBgCenter-xBgStart;
        uint8_t xBoxStart = xBgCenter-boxWidth/2;
        uint8_t yBoxStart = xBgCenter-boxHeight/2;
        uint8_t triangleYSpacing = laserBarThickness+laserSpacing+5;  
};