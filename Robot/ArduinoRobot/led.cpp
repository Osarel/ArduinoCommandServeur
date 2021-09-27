#include "Arduino.h"
#include "led.h"
#include "FastLed.h"
enum Pins
{
  DataPin = 8,
};

Led::Led() {
  
}

void Led::attach(int ping, int numberpixel, int brightness)
{
  pin = ping;
  number = numberpixel;
  gBrightness = brightness;
  led = (CRGB *)malloc(sizeof(CRGB) * number);
  createController();
}

void Led::clear()
{
  controller->clearLedData();
  update();
}

CLEDController *Led::createController()
{
  switch (pin)
  {
  case 1:
    controller = &FastLED.addLeds<WS2812, 1, RGB>(led, number);
    break;
  case 2:
    controller = &FastLED.addLeds<WS2812, 2, RGB>(led, number);
    break;
  case 3:
    controller = &FastLED.addLeds<WS2812, 3, RGB>(led, number);
    break;
  case 4:
    controller = &FastLED.addLeds<WS2812, 4, RGB>(led, number);
    break;
  case 5:
    controller = &FastLED.addLeds<WS2812, 5, RGB>(led, number);
    break;
  case 6:
    controller = &FastLED.addLeds<WS2812, 6, RGB>(led, number);
    break;
  case 7:
    controller = &FastLED.addLeds<WS2812, 7, RGB>(led, number);
    break;
  case 8:
    controller = &FastLED.addLeds<WS2812, 8, RGB>(led, number);
    break;
  case 9:
    controller = &FastLED.addLeds<WS2812, 9, RGB>(led, number);
    break;
  case 10:
    controller = &FastLED.addLeds<WS2812, 10, RGB>(led, number);
    break;

  default:
    Serial.println("Unsupported Pin");
    break;
  }
  return controller;
}


void Led::update()
{
  controller->showLeds(gBrightness);
}

void Led::color(int pixel, CRGB color)
{
  led[pixel] = color;
}

void Led::fill(CRGB color)
{
  fill_solid(led, number, color);
  update();
}
