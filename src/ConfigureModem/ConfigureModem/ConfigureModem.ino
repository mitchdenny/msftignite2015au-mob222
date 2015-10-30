// The purpose of this sketch is to quickly configure the Sparkfun BlueSMiRF
// Silver bluetooth modem to a known baud rate so that it can used with the
// Arduino Uno and remote wiring API from a UWP application. Once the configuration
// is complete the program keeps running to shunt data backwards and forwards from
// the bluetooth modem and the onboard serial port (useful for testing things out).

#include <SoftwareSerial.h>

int modemTxPin = 8;
int modemRxPin = 9;
SoftwareSerial modem(modemTxPin, modemRxPin);

void setup() {
	Serial.print("Setup: Configuring Modem...");
	modem.begin(9600);
	modem.print("$");
	modem.print("$");
	modem.print("$");
	delay(100);
	modem.println("SU,96");
	delay(100);
	modem.println("---");
	Serial.println("done.");
	modem.begin(9600);
}

void loop() {
	while (true) {
		if (modem.available())
		{
			Serial.print((char)modem.read());
		}

		if (Serial.available())
		{
			modem.print((char)Serial.read());
		}
	}
}
