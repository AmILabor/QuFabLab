/**
 * @file menuEntry.hpp
 * @brief Abstrakte Basisklasse für alle Menüeinträge auf dem TFT-Display.
 * 
 * Definiert die Schnittstelle zum Zeichnen von Kopfzeile, Einstellungen,
 * Pfeilen und der Verbindungsanzeige. Jeder konkrete Brick-Typ implementiert
 * seine eigene visuelle Darstellung.
 */

#ifndef MenuEntrie_h
#define MenuEntrie_h
#include "Arduino.h" 
#include "Adafruit_GC9A01A.h"
#include <limits>


class MenuEntry {
public:
    /// Erzeugt einen Menüeintrag mit Display-Zeiger.
    MenuEntry(Adafruit_GC9A01A * display){
        this->display = display;
    };
     ~MenuEntry(){};
    /// Zeichnet die Brick-Darstellung auf dem Display (rein virtuell).
    virtual void render()=0;
    /// Gibt den aktuellen Einstellungswert zurück (rein virtuell).
    virtual int getSetting()=0;
    /// Setzt den Einstellungswert (rein virtuell).
    virtual void setSetting(int value)=0;
    static const int NO_SETTINGS = std::numeric_limits<int>::min();

    /// Zeichnet die Navigationspfeile links/rechts.
    void DrawArrows(){_DrawArrows(GC9A01A_BLACK);}
    /// Löscht die Navigationspfeile.
    void ClearArrows(){_DrawArrows(GC9A01A_WHITE);}
    /// Zeichnet die Einstellungspfeile auf/ab.
    void DrawSettingsArrows(){_DrawSettingsArrows(GC9A01A_BLACK);}
    /// Löscht die Einstellungspfeile.
    void ClearSettingsArrows(){_DrawSettingsArrows(GC9A01A_WHITE);}
    /// Löscht den Kopfzeilenbereich.
    void ClearHeadline(){display->fillRect(xBgStart,yBgStart-8*fontSizeHeadline,bgWidth,8*fontSizeHeadline,white);}
    /// Löscht den Einstellungsbereich.
    void ClearSettings(){display->fillRect(xBgStart,yBgStart+bgHeight,bgWidth,8*fontSizeHeadline,white);}
    /// Zeichnet die I2C-Verbindungsanzeige.
    void DrawConnectedIndicator(bool isConnected){_DrawConnectedIndicator(isConnected);}
protected:
    Adafruit_GC9A01A * display;
    String settingName;
    String headline;
    int settingValue;

    // Colors are optained by Adafruit_GC9A01A dispaly library by tft.
    static const uint16_t red = 63488;
    static const uint16_t bgBlue = 1405;
    static const uint16_t white = 65535;
    static const uint16_t white_opaque=38654;
  
    static const uint8_t xBgStart = 35;
    static const uint8_t yBgStart = 35;
    static const uint8_t bgWidth = 170;
    static const uint8_t bgHeight = 170;
    static const uint8_t bgRadius = 20;
    static const uint8_t xBgCenter = xBgStart+(bgWidth/2);
    static const uint8_t yBgCenter = yBgStart+(bgHeight/2);

    static const uint8_t xPosHeadline=xBgCenter;
    static const uint8_t yPosHeadline=yBgStart;
    static const uint8_t fontSizeHeadline=2;

    /// Zeichnet den Verbindungsindikator (grüner/roter Balken oben).
    void _DrawConnectedIndicator(bool isConnected){
        /**
         *     +--
         *  ---| [---
         *     +--
         */
        int lineStrength = 3;
        uint16_t currentColor= GC9A01A_RED;
        if(isConnected)
            currentColor= GC9A01A_DARKGREEN;
        display->fillRect(0,0,bgWidth,10,currentColor);
        

        
    }
    /// Zeichnet zwei Dreiecke als Navigationspfeile.
    void _DrawArrows(uint16_t color){
        display->drawTriangle(5,yBgCenter,xBgStart-5,yBgCenter-20,xBgStart-5,yBgCenter+20,color);
        display->drawTriangle(235,yBgCenter,xBgStart+bgWidth+5,yBgCenter-20,xBgStart+bgWidth+5,yBgCenter+20,color);
    }
      /// Zeichnet zwei Dreiecke als Einstellungspfeile.
      void _DrawSettingsArrows(uint16_t color){
        int p1x = xBgStart+30;
        int p1y = yBgStart+bgHeight+5;
        int p2x = xBgStart+30;
        int p2y = p1y+10;
        int p3x = xBgStart+20;
        int p3y = p1y+5;
        display->drawTriangle(p1x,p1y,p2x,p2y,p3x,p3y,color);
        p1x = xBgStart+bgWidth-30;
        p1y = yBgStart+bgHeight+5;
        p2x = xBgStart+bgWidth-30;
        p2y = p1y+10;
        p3x = xBgStart+bgWidth-20;
        p3y = p1y+5;
        display->drawTriangle(p1x,p1y,p2x,p2y,p3x,p3y,color);
    }
    /// Zeichnet die Kopfzeile zentriert über dem Hintergrund.
    void DrawHeadline(String hl){   
        display->setTextColor(GC9A01A_BLACK);
        display->setTextSize(fontSizeHeadline);
        int textWidth = (6*fontSizeHeadline*hl.length());
        display->setCursor(xPosHeadline-textWidth/2, yBgStart-8*fontSizeHeadline);
        display->println(hl);
    }
    /// Zeichnet den Einstellungstext unterhalb des Hintergrunds.
    void DrawSettings(String settingString){
        int stringLen =  settingString.length();
        display->setTextColor(GC9A01A_BLACK);
        display->setTextSize(1);
        display->setCursor(120-(6*stringLen)/2,212);
        display->println(settingString);    
    }

    /// Zeichnet eine dicke diagonale Linie (z. B. für Spiegel).
    void DrawAngledLine( int x, int y, int x1, int y1, int size, int color) {
        float dx = (size / 2.0) * (x - x1) / sqrt(sq(x - x1) + sq(y - y1));
        float dy = (size / 2.0) * (y - y1) / sqrt(sq(x - x1) + sq(y - y1));
        display->fillTriangle(x + dx, y - dy, x - dx,  y + dy,  x1 + dx, y1 - dy, color);
        display->fillTriangle(x - dx, y + dy, x1 - dx, y1 + dy, x1 + dx, y1 - dy, color);
    }

    /// Zeichnet ein Kreissymbol mit kleinem Pfeil (z. B. für Strahlteiler).
    void DrawCircleIcon( int cx, int cy, int radius,uint16_t color){
        display->drawCircle(cx,cy,radius,color);
        display->drawTriangle(cx+3,cy-radius,cx-2,cy-radius-5,cx-2, cy-radius+5,color);
        display->drawTriangle(cx-3,cy+radius,cx+2,cy+radius+5,cx+2, cy+radius-5,color);

    }


};
#endif