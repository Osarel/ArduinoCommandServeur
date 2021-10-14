/*
  Digital library
*/
#ifndef digital_h
#define digital_h

#include "Arduino.h"
#include "FastLed.h"
class Digital
{
  public:
    int pin;
    int type;
    int statut = 1000;
    Digital(int pin, int type);
    bool refreshDigitalState();
    float readAnalogValue();
};

#endif
