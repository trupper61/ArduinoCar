#define EnA 10
#define EnB 5
#define In1 9
#define In2 8
#define In3 7
#define In4 6

void setup() {
  // put your setup code here, to run once:

  pinMode(EnA, OUTPUT);
  pinMode(EnB, OUTPUT);
  pinMode(In1, OUTPUT);
  pinMode(In2, OUTPUT);
  pinMode(In3, OUTPUT);
  pinMode(In4, OUTPUT);

  delay(3000);
  vorwaerts();
}

void vorwaerts()
{
  // Motor B
  digitalWrite(In3, HIGH);
  digitalWrite(In4, LOW);

  analogWrite(EnB, 130);
  // Motor A einschalten
  digitalWrite(In1, HIGH);
  digitalWrite(In2, LOW);

  analogWrite(EnA, 130);


  delay(5000);

  digitalWrite(In1, LOW);
  digitalWrite(In2, LOW);
  digitalWrite(In3, LOW);
  digitalWrite(In4, LOW);
}

void loop() {
  // put your main code here, to run repeatedly:
}