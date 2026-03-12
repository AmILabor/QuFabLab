#include "MenuStructure/MenuEntries/menuEntry.hpp"
class Mirror45Entrie: public MenuEntry{
    public:
        Mirror45Entrie(Adafruit_GC9A01A * display) : MenuEntry(display){};
        void render(){
            display->fillRoundRect(xBgStart,yBgStart,bgWidth,bgHeight,bgRadius,bgBlue); // Draw Background
            //display->fillRect(xBgStart,yBgCenter-(laserBarThickness/2),laserBarLength,laserBarThickness,red);  // Laserbeam center to right
            display->fillRect(xBgCenter,yBgCenter-(laserBarThickness/2),laserBarLength,laserBarThickness,red);  // Laserbeam center to right
            display->fillRect(xBgCenter-(laserBarThickness/2),yBgCenter,laserBarThickness,laserBarLength,red); // Laserbeam center to bottom
            DrawAngledLine(boxStart,boxStart+boxSize,boxStart+boxSize,boxStart,mirror_width,white); // diagonal mirror
            DrawHeadline(headline);
        }
        int getSetting(){return MenuEntry::NO_SETTINGS;};
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