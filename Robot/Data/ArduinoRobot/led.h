/*
  LED.h library gestion
*/
#ifndef led_h
#define led_h

#include "Arduino.h"
#include "FastLed.h"
class Led
{
  public:
    Led();
    void attach(int pin, int numberpixel, int brightness);
    void color(int led, CRGB color);
    void fill(CRGB color);
    void update();
    void clear();
  private:
    CLEDController* createController();
    int number;
    int pin;
    CRGB *led;
    CLEDController *controller;
    uint8_t gBrightness = 128;
};

#endif
