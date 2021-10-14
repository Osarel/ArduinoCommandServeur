#include <Arduino.h>
#include <Servo.h>
#define FASTLED_INTERNAL
#include "FastLED.h"
#include "led.h"
#include "digital.h"
String inputString = "";
Servo myServos[15];
Led myLED[10];
int a, b, c, d, e, f;
Digital *declaredPIN = (Digital*)malloc(sizeof(Digital) * 30);
int numberOfPinDeclared = 0;
void setup() {
  // reserve 200 bytes for the message:
  inputString.reserve(200);

  Serial.begin(9600);
  Serial.flush();
}

void loop() {
  loop2();
}

void  SerialManager(String message) {
  if (message.startsWith("rob://")) {
    char command = message.charAt(6);
    retrieveArgument(message);
    switch (command) {
      case 'a':
        return serv();
      case 'b':
        return deta();
      case 'c':
        return rgbi();
      case 'd':
        return rgbr();
      case 'e':
        return rgbs();
      case 'f':
        return rgba();
      case 'g':
        return rgbc();
      case 'h':
        return rese();
      case 'i':
        return dpin();
      case 'j':
        return dcom(true);
      case 'k':
        return dcom(false);
      case 'l':
        return anaw(message);
      case 'm':
        return anar(message);
    }
  }
}

void retrieveArgument(String message){
  byte firstSlashPosition = message.indexOf('/', 8 );
  byte secondSlashPosition = message.indexOf('/', firstSlashPosition + 1 );
  byte treeSlashPosition = message.indexOf('/', secondSlashPosition + 1 );
  byte fourSlashPosition = message.indexOf('/', treeSlashPosition + 1 );
  byte fiveSlashPosition = message.indexOf('/', fourSlashPosition + 1 );
  byte sixSlashPosition = message.indexOf('/', fiveSlashPosition + 1 );
  a = message.substring(8, firstSlashPosition).toInt();
  b = message.substring(firstSlashPosition + 1, secondSlashPosition).toInt();
  c = message.substring(secondSlashPosition + 1, treeSlashPosition).toInt();
  d = message.substring(treeSlashPosition + 1, fourSlashPosition).toInt();
  e = message.substring(fourSlashPosition + 1, fiveSlashPosition).toInt();
  f = message.substring(fiveSlashPosition + 1, sixSlashPosition).toInt();
  /*    int newSlashPosition = message.indexOf('/', firstSlashPosition + 1 );
   *   int firstSlashPosition = 7;
int x = {};
   *   byte number = 0;
  while(newSlashPosition != -1){
    valeurs[number] = message.substring(firstSlashPosition + 1, newSlashPosition).toInt();
    firstSlashPosition = newSlashPosition;
    newSlashPosition = message.indexOf('/', firstSlashPosition + 1);
    number = number + 1;
  }*/
}


void serv() {
  int pin = a;
  byte degree = b;
  Servo servo = myServos[pin];
  if (!servo.attached()) {
    servo.attach(pin);
  }
  servo.write(degree);
}


void deta() {
  byte pin = a;
  myServos[pin].detach();
}

void rgbi() {
  byte pin = a;
  int nbr = b;
  byte bright = c;
  myLED[pin].attach(pin, nbr, bright);
  myLED[pin].clear();
  myLED[pin].update();
}


void rgbr() {
  int pin = a;
  //Inversion des couleurs à cause du bug couleurs
  int G = b;
  int R = c;
  int B = d;
  myLED[pin].fill(CRGB(R, G, B));
  myLED[pin].update();
}


void rgbs() {
  byte pin = a;
  byte LED = b;
  int G = c;
  //Inversion des couleurs à cause du bug couleurs
  int R = d;
  int B = e;
  int s = f;
  myLED[pin].color(LED, CRGB(R, G, B));
  if (s == 1) {
    myLED[pin].update();
  }
}
void rgba() {
  int pin = a;
  myLED[pin].update();
}
void rgbc() {
  int pin = a;
  myLED[pin].clear();
  myLED[pin].update();
}
void rese() {
  asm volatile ("  jmp 0");
}
void dpin() {
  int pin = a;
  declareDigitalPinIN(pin, INPUT);
}

void dcom(bool high) {
  int pin = a;
  declareDigitalPinOut(pin);
  digitalWrite(pin, high ? HIGH : LOW);
}

void anaw(String message) {
  int firstSlashPosition = message.indexOf('/', 8 );
  int secondSlashPosition = message.indexOf('/', firstSlashPosition + 1 );
  int pin = message.substring(11, firstSlashPosition).toInt();
  float value = message.substring(firstSlashPosition + 1, secondSlashPosition).toFloat();
  declareDigitalPinOut(pin);
  analogWrite(pin, value);
}
void anar(String message) {
  int firstSlashPosition = message.indexOf('/', 8 );
  int secondSlashPosition = message.indexOf('/', firstSlashPosition + 1 );
  int pin = message.substring(11, firstSlashPosition).toInt();
  float value = message.substring(firstSlashPosition + 1, secondSlashPosition).toFloat();
  declareDigitalPinIN(pin, 3);
  String messageb = "{\"type\": \"analog\",\"pin\":";
  messageb.concat(pin);
  messageb.concat(",\"state\" : ");
  messageb.concat(analogRead(pin));
  messageb.concat("}");
  Serial.println(messageb);
}
void declareDigitalPinOut(int pin) {
  if (!containDigitalPinDeclared(pin)) {
    declaredPIN[numberOfPinDeclared] = *new Digital(pin, OUTPUT);
    numberOfPinDeclared++;
    pinMode(pin, OUTPUT);
  }
}
void declareDigitalPinIN(int pin, int type)  {
  if (!containDigitalPinDeclared(pin)) {
    declaredPIN[numberOfPinDeclared] = *new Digital(pin, type);
    numberOfPinDeclared++;
    pinMode(pin, INPUT);
  }
}

bool containDigitalPinDeclared(int pin) {
  for (int i = 0; i < numberOfPinDeclared; i++) {
    if (declaredPIN[i].pin == pin) {
      return true;
    }
  }

  return false;
}
/*
  SerialEvent occurs whenever a new data comes in the hardware serial RX. This
  routine is run between each time loop() runs, so using delay inside loop can
  delay response. Multiple bytes of data may be available.
*/
void serialEvent() {
  while (Serial.available()) {
    // get the new byte:
    char inChar = (char)Serial.read();
    // add it to the message:
    inputString += inChar;
    // if the incoming character is a newline, set a flag so the main loop can
    // do something about it:
    if (inChar == '\n') {
      SerialManager(inputString);
      inputString = "";
    }
  }
}

void loop2() {
  //check For Digital Pin Update
  for (int i = 0; i < numberOfPinDeclared; i++) {
    if (declaredPIN[i].type == INPUT) {
      if (declaredPIN[i].refreshDigitalState()) {
        String message = "{\"type\": \"digital\",\"pin\":";
        message.concat(declaredPIN[i].pin);
        message.concat(",\"state\" : ");
        message.concat(declaredPIN[i].statut);
        message.concat("}");
        Serial.println(message);
      }
    }
  }

}