#include <WiFiNINA.h>
#define EnA 10
#define EnB 5
#define In1 9
#define In2 8
#define In3 7
#define In4 6

int speed = 0;

char ssid[] = "ArduinoTest";
char pass[] = "password";

int status = WL_IDLE_STATUS;
WiFiServer server(5555); // Creates server listens incomming connections to port 80

void setup() {
  pinMode(EnA, OUTPUT);
  pinMode(EnB, OUTPUT);
  pinMode(In1, OUTPUT);
  pinMode(In2, OUTPUT);
  pinMode(In3, OUTPUT);
  pinMode(In4, OUTPUT);
  // put your setup code here, to run once:
  Serial.begin(9600);
  while(!Serial){
    ;
  }
  Serial.println("Starting Access Point...");
  status = WiFi.beginAP(ssid, pass);
  if (status != WL_AP_LISTENING) {
    Serial.println("Creating access point failed!");
    while (true);
  }

  delay(50000);
  printWiFiStatus();
  server.begin();
  Serial.println("Server started, waiting for connections...");
}

void loop() {
  // put your main code here, to run repeatedly:
  WiFiClient client = server.available(); // listen for incomming clients
  if (client) {
    Serial.println("New client connected");
    while (client.connected()) { // loop while client connected
      if (client.available()) { // checks if their are bytes to read
        char s = client.read();
        Serial.print("Received: ");
        if ((int)s == 1) {
          speed += 10;
          if (speed < 130) {
            speed = 130;
            vorwaerts();
            changeSpeed();
          }
          else if (speed < 180){
            changeSpeed();
          }
          
        }
        else if ((int)s == 0) {
          speed -= 10;
          if (speed < 130) {
            stop();
            speed = 0;
          }
          else {
            changeSpeed();
          }
        }
        Serial.println((int)s);
      }
    }
    client.stop();
    Serial.println("Client disconnected.");

  }
}

void changeSpeed(){
  analogWrite(EnB, speed);
  analogWrite(EnA, speed);
}

void vorwaerts()
{
  // Motor B
  digitalWrite(In3, HIGH);
  digitalWrite(In4, LOW);
  // Motor A einschalten
  digitalWrite(In1, HIGH);
  digitalWrite(In2, LOW);
}
void stop() {
  digitalWrite(In1, LOW);
  digitalWrite(In2, LOW);
  digitalWrite(In3, LOW);
  digitalWrite(In4, LOW);
}

void printWiFiStatus() {
  Serial.print("SSID: ");
  Serial.println(WiFi.SSID());

  IPAddress ip = WiFi.localIP();
  Serial.print("IP Address: ");
  Serial.println(ip);
}
