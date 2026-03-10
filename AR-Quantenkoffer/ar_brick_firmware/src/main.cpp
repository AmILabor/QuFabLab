#include "Arduino.h"
#include <SoftWire.h>
#include <AsyncDelay.h>
#include "PCF8574AN.h"
#include <FlashStorage_SAMD.h>
#include "encoder.hpp"
#include "MenuStructure/menu.hpp"
#include "SPI.h"
#include "Adafruit_GFX.h"
#include "Adafruit_GC9A01A.h"
#include "settingPersistence.hpp"


#define PCF_ROT_JUMPER_P1 1
#define PCF_ROT_JUMPER_P2 2
#define PCF_OC_JUMPER  3
#define PCF_BACKLIGHT 0
#define SDA_SI2C 7
#define SCL_SI2C 6 
#define PDF8574_ADDR 0x38
#define TFT_CS 0
#define TFT_DC 3
#define I2C_ADDR  0x16
#define TYPE_REGISTER  0x10
#define SETTING_REGISTER0 0x11
#define SETTING_REGISTER1 0x12
#define ROTATION_REGISTER 0x13
#define STORE_SETTINGS 0x14

// We Need SoftWareI2C to speak to the I2CGPIOExpander
SoftWire i2c(SDA_SI2C,SCL_SI2C);

PCF8574AN pcf20(&i2c, PDF8574_ADDR);

char swTxBuffer[16];
char swRxBuffer[16];
int currentRotation=0;
uint8_t currentType;
int currentTypeSetting;
int registerPointer;
EncoderResult encoderResult;
Encoder * enc;
Menu * menu;
SettingsPersistence * persistence;
Adafruit_GC9A01A *tft;

void setupSoftwareI2C();
void setupGPIOExpander();
uint8_t readRotationJumpers();
void setBacklight(uint8_t state);
void setupTFT();
void restoreMenu();
void restoreSettingsFromPersistence();
void I2C_RxHandler(int numBytes);
void I2C_RqHandler();
uint8_t readOccupiedJumpers();


void setup() {
  Serial.begin(9600);
  setupSoftwareI2C();
  setupGPIOExpander();
  setupTFT();
  enc = new Encoder(1,2,9);
  menu = new Menu(tft,1);
  persistence = new SettingsPersistence();
  restoreSettingsFromPersistence();
  restoreMenu();
  menu->drawConnectedIndicator();
  menu->setDetected(false);

  Wire.begin(I2C_ADDR); 
  Wire.onReceive(I2C_RxHandler);
  Wire.onRequest(I2C_RqHandler);

}

unsigned long lastI2CRequest = millis();
unsigned long millisI2CRequestTimeout = 500;
unsigned long lastI2CConnectedState = true;
void loop(void) {
  enc->loop();
  encoderResult = enc->getResult();
  int rotation = readRotationJumpers();
  if(lastI2CConnectedState != menu->getDetected()){
    menu->drawConnectedIndicator();
  }
  if(millis()-lastI2CRequest> millisI2CRequestTimeout){
    menu->setDetected(false);
  }

  lastI2CConnectedState = menu->getDetected();
  int type=currentType;
  int typeSetting=currentTypeSetting;
  
  if(encoderResult.changed){
      if(encoderResult.pressed){
        menu->nextMode();
      }
      else{
        menu->handleInput(encoderResult.direction);
      }
      type = menu->getCurrentEntrie();
      typeSetting = menu->getCurrentTypeSetting();
  }
  if(type!= currentType || typeSetting!= currentTypeSetting ||rotation != currentRotation){
    currentType = type;
    currentTypeSetting = typeSetting;
    currentRotation=rotation;
    Serial.println("State changed!: "+String(currentType)+"="+String(currentTypeSetting)+"   ROT: "+String(currentRotation));
    persistence->persist(currentType,currentRotation);

  }
  
  delay(1);
  
}

/**
 * @brief Initializes the tft variable, resets the screen and enables the backlight of the screen.
 */
void setupTFT(){
  tft = new Adafruit_GC9A01A((uint8_t)TFT_CS, (uint8_t)TFT_DC);
  tft->begin();
  tft->setCursor(0,0);
  tft->fillScreen(GC9A01A_WHITE);
  setBacklight(1);
}

/**
 * @brief Initializes the software i2c-instance by setting delay, timeout and  receive and transmit-buffers
 */
void setupSoftwareI2C(){
  i2c.setTxBuffer(swTxBuffer, sizeof(swTxBuffer));
  i2c.setRxBuffer(swRxBuffer, sizeof(swRxBuffer));
  i2c.setDelay_us(5);
  i2c.setTimeout(1000);
  i2c.begin();
  
}

/**
 * @brief Initializes the GPIO-Expander.
 */
void setupGPIOExpander(){
  while (pcf20.begin() == false)
    {
      Serial.println("\nERROR: cannot communicate to PCF8574.");
      Serial.println("Please reboot / adjust address.\n");
      delay(500);
    }
  pcf20.selectNone();
}

/**
 * @brief Reads the rotation jumpers and shifts the two bits into an uint8_t
 * 
 * @return uint8_t value of rotation (0-3)
 */
uint8_t readRotationJumpers(){
  uint8_t p1val = pcf20.read(PCF_ROT_JUMPER_P1);
  uint8_t p2val = pcf20.read(PCF_ROT_JUMPER_P2);
  return (p1val << 1) + p2val;
}

/**
 * @brief Reads the occupied bridge, to see if it is getting pulsed.
 * 
 * @return uint8_t High or low if the GPIOs are pulsed.
 */
uint8_t readOccupiedJumpers(){
  uint8_t val = pcf20.read(PCF_OC_JUMPER);
  return val;
}

/**
 * @brief Enables or disables backlight of the TFT - necessary cause we use the GPIO expander to contorl the backlight.
 * 
 * @param state 
 */
void setBacklight(uint8_t state){
    pcf20.write(PCF_BACKLIGHT,state);
}

/**
 * @brief Restores the menu and renders the menu.
 * 
 */
void restoreMenu(){
  bool success = menu->setCurrentEntrie(currentType);
  if(success){
    menu->setCurrentTypeSetting(currentTypeSetting);
    menu->render();
  }
  else{
    menu->setCurrentEntrie(0);
    menu->render();
  }
}
/**
 * @brief Loads the settings and type values from the persistence 
 * 
 */
void restoreSettingsFromPersistence(){
  persistence->load();
  currentType = persistence->getType();
  currentTypeSetting = persistence->getSetting();
}
/**
 * @brief Handler to process incoming data from i2c this function handles the i2c-client part!
 * @param numBytes number of bytes that are sent.
 */
void I2C_RxHandler(int numBytes)
{
   if (Wire.available() > 0) 
  {
    registerPointer = Wire.read(); 
    
  }

}
/**
 * @brief Handler to write requested Data to the i2c this function handles the i2c-client part!
 */
void I2C_RqHandler(){
   lastI2CRequest = millis();
   menu->setDetected(true);
  if(registerPointer == TYPE_REGISTER){
    Wire.write(currentType);
  }
  else if(registerPointer == ROTATION_REGISTER){
    Wire.write(currentRotation);
  }
  else if(registerPointer==SETTING_REGISTER0){
    Wire.write(lowByte(currentTypeSetting));
    Wire.write(highByte(currentTypeSetting));
  }
  else if(registerPointer==STORE_SETTINGS){
    persistence->persist(currentType,currentRotation);
    Wire.write(currentType);
  }
}