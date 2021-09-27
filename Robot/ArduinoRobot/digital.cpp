#include "Arduino.h"
#include "digital.h"
Digital::Digital(int ping, int types)
{
  pin = ping;
  type = types;
}

bool Digital::refreshDigitalState()
{
  if (type == INPUT)
  {
    int val = digitalRead(pin);
    if (val != statut)
    {
      statut = val;
      return true;
    }
    return false;
  }
  return false;
}
float Digital::readAnalogValue()
{
  if (type == 3)
  {
    int val = analogRead(pin);
    return val;
  }
  return 0;
}
