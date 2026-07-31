/**
 * @file settingPersistence.hpp
 * @brief Verwaltet das dauerhafte Speichern und Laden von
 * Baustein-Typ und -Einstellungen im Flash-Speicher (EEPROM-Emulation).
 */

#include "Arduino.h"
#include <FlashStorage_SAMD.h>

/// Verwaltet das Speichern und Laden von Brick-Typ und -Einstellungen im Flash.
class SettingsPersistence{
    public:
        SettingsPersistence(){}; ///< Standardkonstruktor.
        /// Speichert Typ und/oder Einstellung, falls geändert, und committed.
        bool persist(uint8_t type, int setting){
            bool persistet = false;
            Serial.print("Persisting.... ");
            if(type != _type){
                _type = type;
                persistet = persistType() || persistet;
                Serial.print("Type: "+String(_type)+" | ");
            }
            if(setting != _setting){
                _setting = setting;
                persistet = persistSetting() || persistet;
                Serial.print("Setting: "+String(_setting)+" | ");
            }
            if(persistet){
                Serial.println("Comitted!");
                EEPROM.commit();
            }
            return persistet;
        };
        /// Lädt Typ und Einstellung aus dem Flash-Speicher.
        void load(){
            _setting = loadSetting();
            _type = loadType();
        }
        /// Gibt den gespeicherten Brick-Typ zurück.
        uint8_t getType(){return _type;};
        /// Gibt die gespeicherte Einstellung zurück.
        int getSetting(){return _setting;};
    private:
        /// Schreibt den Brick-Typ in den EEPROM.
        bool persistType(){
            Serial.println("Persisting Type "+String(_type));
            EEPROM.put(TypeRegister,_type);
            return true;
        };
        /// Schreibt die Einstellung (als MSB/LSB) in den EEPROM.
        bool persistSetting(){
            Serial.println("Persisting Setting "+String(_setting));
            byte settingmsb = _setting >>8;
            byte settinglsb = _setting & 0x00FF;
            EEPROM.put(SettingsRegister0,settinglsb);
            EEPROM.put(SettingsRegister1,settingmsb);
            return true;
        };
        /// Lädt den Brick-Typ aus dem EEPROM.
        uint8_t loadType(){
            uint8_t result;
            result = EEPROM.read(TypeRegister);
            Serial.println("Loading Type "+String(result));

            return result;
        }
        /// Lädt die Einstellung (MSB/LSB) aus dem EEPROM.
        int loadSetting(){
            byte resultlsb, resultmsb;
            int result = 0;
            resultlsb = EEPROM.read(SettingsRegister0);
            resultmsb = EEPROM.read(SettingsRegister1);
            
            result = resultmsb <<8 | resultlsb;
            result = (result & 0x0FFF) - (result&0x1000);
            Serial.println("Loading Setting MSB "+String(resultmsb)+" LSB "+String(resultlsb));
            Serial.println("  "+String((result & 0x0FFF))+"-"+String((result&0x1000))+"="+String(result));
            return result;
        }

        uint8_t _type=-1;
        int _setting=-1;
        int TypeRegister = 0;
        int SettingsRegister0  = 1;
        int SettingsRegister1  = 2;
};
