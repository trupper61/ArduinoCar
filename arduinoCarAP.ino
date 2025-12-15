#include <WiFiNINA.h>
#define EnA 10
#define EnB 5
#define In1 9
#define In2 8
#define In3 7
#define In4 6

int speedRight = 0;
int speedLeft = 0;
// int distance 0; //Nutzer bestimmt, wie weit es Fahren soll.

char ssid[] = "ArduinoCar";
char pass[] = "password123";

int status = WL_IDLE_STATUS;
WiFiServer server(5555); // Creates server listens incomming connections to port 5555

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

  delay(2000);
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
      if (client.available() >= 1) { // checks if their are bytes to read
        byte cmd = client.read();
        Serial.print("Received: ");
        Serial.println(cmd);
        
        speedRight = 150;
        speedLeft = 150;
        
        if (client.available() >= 1){
          speedRight = (int)client.read();
          speedLeft = speedRight;
        }
        if (client.available() >= 1){
          speedRight = (int)client.read();
        }

        switch(cmd) {
          case 0: stop(); speedLeft = 0; speedRight = 0; break;
          case 1: vorwaerts(); break;
          case 2: ruckwaerts(); break;
          case 3: dreheLinks(); break;
          case 4: dreheRechts(); break;
          default: stop(); break;
        }
          
        changeSpeed();
        Serial.print("Left: ");
        Serial.println(speedLeft);
        Serial.print("Right: ");
        Serial.println(speedRight);
      }
    }
    client.stop();
    Serial.println("Client disconnected.");

  }
}

void changeSpeed(){
  analogWrite(EnB, speedLeft);
  analogWrite(EnA, speedRight);
}

void dreheLinks() {
  digitalWrite(In3, LOW);
  digitalWrite(In4, HIGH);

  digitalWrite(In1, HIGH);
  digitalWrite(In2, LOW);
}

void dreheRechts() {
  digitalWrite(In3, HIGH);
  digitalWrite(In4, LOW);

  digitalWrite(In1, LOW);
  digitalWrite(In2, HIGH);
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

void ruckwaerts()
{
  // Motor B
  digitalWrite(In3, LOW);
  digitalWrite(In4, HIGH);
  // Motor A einschalten
  digitalWrite(In1, LOW);
  digitalWrite(In2, HIGH);
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
