#ifndef Menu_h
#define Menu_h
#include "Arduino.h" 
#include "Adafruit_GC9A01A.h"
#include "MenuEntries/menuEntry.hpp"
#include "MenuStructure/MenuEntries/BeamSplitter.hpp"
#include "MenuStructure/MenuEntries/Mirror90.hpp"
#include "MenuStructure/MenuEntries/Mirror45.hpp"
#include "MenuStructure/MenuEntries/Periscope.hpp"
#include "MenuStructure/MenuEntries/Shutdown.hpp"
#include <vector>

class Menu {
public:
    Menu(Adafruit_GC9A01A * _display,int rotation=3){
        display = _display;
        displayRotation = rotation;
        entries = {new BeamSplitterEntrie(_display),
                   new Mirror90Entrie(_display),
                   new Mirror45Entrie(_display)}; // new PeriscopeEntrie(_display)
    };
    bool getDetected(){
        return this->isDetected;
    }
    void render(){     
        display->setRotation(displayRotation);
        if(mode==0){
            //display->fillScreen(GC9A01A_WHITE);
            entries[currentEntrie]->ClearHeadline();
            entries[currentEntrie]->ClearSettings();
            entries[currentEntrie]->DrawArrows();
        }
        if(mode==1 && entries[currentEntrie]->getSetting()!=MenuEntry::NO_SETTINGS){
            entries[currentEntrie]->ClearArrows();
            entries[currentEntrie]->ClearSettings();
            entries[currentEntrie]->DrawSettingsArrows();
        }
        entries[currentEntrie]->render();

    }
    void drawConnectedIndicator(){
        entries[currentEntrie]->DrawConnectedIndicator(this->isDetected);    
        //entries[currentEntrie]->render();
    }
    void handleInput(uint8_t direction){
        if(direction==1){
            next();
        }
        else if(direction==0){
            prev();
        }
        render();
    }
    void next(){
        if(mode == 1){
            int v = entries[currentEntrie]->getSetting();
            entries[currentEntrie]->setSetting(v+1);
        }
        if(mode == 0){
            currentEntrie+=1;
            if(currentEntrie > entries.size()-1)
                currentEntrie=0;
        }
        
    }
    void prev(){
        if(mode == 1){
            int v = entries[currentEntrie]->getSetting();
            entries[currentEntrie]->setSetting(v-1);
        }
        else if(mode==0){
            currentEntrie-=1;
            if(currentEntrie<0)
                currentEntrie=entries.size()-1;
        }
        
    }
    void nextMode(){
        if(entries[currentEntrie]->getSetting()==MenuEntry::NO_SETTINGS)
            return;
        if(mode ==0) mode = 1;
        else if(mode == 1) mode = 0;
        render();
    }
    int getCurrentEntrie(){
        return currentEntrie;
    }
    int getCurrentTypeSetting(){
        return entries[currentEntrie]->getSetting();
    }
    bool setCurrentEntrie(int entrie){
        if(entrie <0 | entrie >= entries.size())
            return false;

        currentEntrie = entrie;
        return true;
    }
    void setCurrentTypeSetting(int setting){
        entries[currentEntrie]->setSetting(setting);
    }
    void setDetected(bool detected){
        this->isDetected = detected;
    }
private:
    bool pressed;
    int currentEntrie=0;
    int displayRotation;
    bool isDetected = false;
    int mode = 0; // 0 = Navigation Mode, 1 = Active, 2 = Setting
    Adafruit_GC9A01A * display;
    std::vector<MenuEntry*> entries; // = new std::list<MenuEntry>();
};
#endif