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
class ShutdownEntrie: public MenuEntry{
    public:
        ShutdownEntrie(Adafruit_GC9A01A * display) : MenuEntry(display){};
        void render(){
            display->fillRoundRect(xBgStart,yBgStart,bgWidth,bgHeight,bgRadius,bgBlue); // Draw Background
            display->fillRoundRect(xTopLeft,yTopLeft,xBottomRight-xTopLeft,yBottomRight-yTopLeft,bgRadius,red); // Draw Background
            display->fillTriangle(xTriangleStart11,yTriangleStart11,xTriangleStart12,yTriangleStart12,xTriangleStart13,yTriangleStart13,bgBlue); // Left blue triangle
            display->fillTriangle(xTriangleStart11,yTriangleStart11,xTopLeft,yTopLeft,xTriangleStart31,yTriangleStart31,bgBlue); // Top left
            display->fillTriangle(xTriangleStart21,yTriangleStart21,xTriangleStart22,yTriangleStart22,xTriangleStart23,yTriangleStart23,bgBlue); // Right blue triangle
            display->fillTriangle(xTriangleStart21,yTriangleStart21,xTopRight,yTopRight,xTriangleStart32,yTriangleStart32,bgBlue); // Top right
            display->fillTriangle(xTriangleStart31,yTriangleStart31,xTriangleStart32,yTriangleStart32,xTriangleStart33,yTriangleStart33,bgBlue); // Top blue triangle
            display->fillTriangle(xTriangleStart41,yTriangleStart41,xBottomLeft,yBottomLeft,xTriangleStart12,yTriangleStart12,bgBlue); // bottom left 
            display->fillTriangle(xTriangleStart41,yTriangleStart41,xTriangleStart42,yTriangleStart42,xTriangleStart43,yTriangleStart43,bgBlue); // bottom blue triangle
            display->fillTriangle(xTriangleStart42,yTriangleStart42,xBottomRight,yBottomRight,xTriangleStart22,yTriangleStart22,bgBlue); // bottom right
            DrawHeadline(headline);
            String settingString = "Nein";
            if(settingValue == 1)
                settingString = "Ja";
            DrawSettings(settingName+": "+settingString);
        }
        int getSetting(){return settingValue;}
        void setSetting(int value){
            settingValue = value;
            }
    protected:
        String headline ="Shutdown";
        String settingName = "Abschalten: ";
        int settingValue = 0;
        uint8_t laserSpacing = 15;
        uint8_t laserBarThickness = 15;
        uint8_t boxRadius = 5;
        uint8_t boxWidth = bgWidth*0.8;
        uint8_t boxHeight = bgHeight*0.8;
        uint8_t laserBarLength = xBgCenter-xBgStart;
        
        
        uint8_t xBoxStart = xBgCenter-boxWidth/2;


        uint8_t yBoxStart = xBgCenter-boxHeight/2 + triangleYSpacing;
        uint8_t triangleYSpacing = laserBarThickness;  
        //Left Triangle
        uint8_t xTriangleStart11 = xBgCenter-boxWidth/2;
        uint8_t yTriangleStart11 = yBgCenter-boxHeight/2+ triangleYSpacing;
        uint8_t xTriangleStart12 = xBgCenter-boxWidth/2;
        uint8_t yTriangleStart12 = yBgCenter+boxHeight/2-triangleYSpacing;
        uint8_t xTriangleStart13 = xBgCenter-triangleYSpacing;
        uint8_t yTriangleStart13 = yBgCenter;

        //right Triangle
        uint8_t xTriangleStart21 = xBgCenter+boxWidth/2;
        uint8_t yTriangleStart21 = yBgCenter-boxHeight/2+ triangleYSpacing;
        uint8_t xTriangleStart22 = xBgCenter+boxWidth/2;
        uint8_t yTriangleStart22 = yBgCenter+boxHeight/2-triangleYSpacing;
        uint8_t xTriangleStart23 = xBgCenter+triangleYSpacing;
        uint8_t yTriangleStart23 = yBgCenter;

        
        //top Triangle
        uint8_t xTriangleStart31 = xBgCenter-boxWidth/2+triangleYSpacing;
        uint8_t yTriangleStart31 = yBgCenter-boxHeight/2;
        uint8_t xTriangleStart32 = xBgCenter+boxWidth/2-triangleYSpacing;
        uint8_t yTriangleStart32 = yBgCenter-boxHeight/2;
        uint8_t xTriangleStart33 = xBgCenter;
        uint8_t yTriangleStart33 = yBgCenter-triangleYSpacing;

        
        //bottom Triangle
        uint8_t xTriangleStart41 = xBgCenter-boxWidth/2+triangleYSpacing;
        uint8_t yTriangleStart41 = yBgCenter+boxHeight/2;
        uint8_t xTriangleStart42 = xBgCenter+boxWidth/2-triangleYSpacing;
        uint8_t yTriangleStart42 = yBgCenter+boxHeight/2;
        uint8_t xTriangleStart43 = xBgCenter;
        uint8_t yTriangleStart43 = yBgCenter+triangleYSpacing;


        uint8_t xTopLeft = xTriangleStart11 ;
        uint8_t yTopLeft = yTriangleStart11 - triangleYSpacing;

        uint8_t xTopRight = xTriangleStart21 ;
        uint8_t yTopRight = yTriangleStart21 - triangleYSpacing;

        uint8_t xBottomLeft = xTriangleStart41 - triangleYSpacing;
        uint8_t yBottomLeft = yTriangleStart41;

        uint8_t xBottomRight = xTriangleStart42 + triangleYSpacing;
        uint8_t yBottomRight = yTriangleStart42;

};