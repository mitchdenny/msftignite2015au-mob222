#include <SoftwareSerial.h>

int bluetoothTxPin = 8;

int bluetoothRxPin = 9;
SoftwareSerial bluetooth(bluetoothTxPin, bluetoothRxPin);

int ultrasonicTriggerPin = 12;
int ultrasonicEchoPin = 3;


void setup() {
	configureUltrasonic();
	configureBluetooth();
}

void loop() {
	while (true) {

		String command = bluetooth.readStringUntil('\r');

		if (command == "read") {
			long distance = readDistanceWithUltrasonic();

			//if (Serial.availableForWrite()) {
			//	Serial.println(distance);
			//}

			bluetooth.println(distance);
		}
	}
}

void configureBluetooth() {
	bluetooth.begin(9600);
	bluetooth.print("$");
	bluetooth.print("$");
	bluetooth.print("$");
	delay(100);
	bluetooth.println("SU,96");
	delay(100);
	bluetooth.println("---");
	bluetooth.begin(9600);
}

long readDistanceWithUltrasonic() {

	digitalWrite(ultrasonicTriggerPin, LOW);
	delayMicroseconds(2);
	digitalWrite(ultrasonicTriggerPin, HIGH);
	delayMicroseconds(10);
	digitalWrite(ultrasonicTriggerPin, LOW);

	long duration = pulseIn(ultrasonicEchoPin, HIGH);

	long centimeters = microsecondsToCentimeters(duration);
	return centimeters;
}

long microsecondsToCentimeters(long microseconds)
{
	// The speed of sound is 340 m/s or 29 microseconds per centimeter.
	// The ping travels out and back, so to find the distance of the
	// object we take half of the distance travelled.
	return microseconds / 52.2;
}

void configureUltrasonic() {
	pinMode(ultrasonicTriggerPin, OUTPUT);
	digitalWrite(ultrasonicTriggerPin, LOW);
	pinMode(ultrasonicEchoPin, INPUT);
}
