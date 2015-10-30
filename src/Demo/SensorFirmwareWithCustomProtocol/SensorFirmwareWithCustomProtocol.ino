int bluetoothTxPin = 8;
int bluetoothRxPin = 9;
int ultrasonicTriggerPin = 12;
int ultrasonicEchoPin = 3;

void setup() {
	configureUltrasonic(ultrasonicTriggerPin, ultrasonicEchoPin);
}

void loop() {
	Serial.println("Reading distance");
	long distance = readDistanceWithUltrasonic(ultrasonicTriggerPin, ultrasonicEchoPin);
	Serial.println(distance);
	delay(1000);

	//Serial.print(distance);
	//Serial.println();
	//delay(1000);
}


void configureDiagnostics(int baudRate) {
	Serial.begin(baudRate);
}

void configureBluetooth(int txPin, int rxPin) {
}

long readDistanceWithUltrasonic(int triggerPin, int echoPin) {
	digitalWrite(triggerPin, LOW);
	delayMicroseconds(2);
	digitalWrite(triggerPin, HIGH);
	delayMicroseconds(5);
	digitalWrite(triggerPin, LOW);

	long duration = pulseIn(echoPin, HIGH);

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

void configureUltrasonic(int triggerPin, int echoPin) {
	pinMode(triggerPin, OUTPUT);
	digitalWrite(triggerPin, LOW);
	pinMode(echoPin, INPUT);
}
