#include <SoftwareSerial.h>
#include <SerialCommand.h>
SerialCommand sCmd;

//needed for calculate BPM function
float inByte = 0;
float BPM = 0;
float beat_old = 0;
int beats[10];  // Used to calculate average BPM
int beatIndex;
float threshold = 200.0;  //Threshold at which BPM calculation occurs
boolean belowThreshold = true;

void setup() {
  setupHR();
  setupSerial();
}

void loop () {
  checkSerial();
  checkHR();
}

void setupHR(){
  pinMode(10, INPUT); // Setup for leads off detection LO +
  pinMode(11, INPUT); // Setup for leads off detection LO -
}

void checkHR(){

  if((digitalRead(10) == 1)||(digitalRead(11) == 1)){
    //invalid value - LEADS OFF DON't TAKE READING
    //Serial.println('!');
  }
  else{
    // send the value of analog input 0:
    inByte = analogRead(A5); 
    //Serial.println(inByte);
      
      // BPM calculation check
      if (inByte > threshold && belowThreshold == true)
      {
        calculateBPM();
        belowThreshold = false;
      }
      else if(inByte < threshold)
      {
        belowThreshold = true;
      }
    //Serial.println(analogRead(A0));
  }
  //Wait for a bit to keep serial data from saturating
  //delay(1);
}

void calculateBPM () {  
  int beat_new = millis();    // get the current millisecond
  int diff = beat_new - beat_old;    // find the time between the last two beats
  float currentBPM = 60000 / diff;    // convert to beats per minute
  if(currentBPM > 200){
    return;
  }
  beats[beatIndex] = currentBPM;  // store to array to convert the average
  float total = 0.0;
  int validCount = 0;
  for (int i = 0; i < (sizeof(beats)/sizeof(beats[0])); i++){
    
    if(beats[i] > 0){
      validCount++;
      total += beats[i];
    }
  }
  BPM = int( total / validCount );
  beat_old = beat_new;
  beatIndex = (beatIndex + 1) % (sizeof(beats)/sizeof(beats[0]));  // cycle through the array instead of using FIFO queue
  //Serial.print("valid count is: ");
  //Serial.println( validCount);
  //Serial.print("currentBPM is ");
  Serial.println(currentBPM);
  //Serial.print("averaged bpm is ");
  //Serial.println(BPM);
}


void setupSerial(){
  Serial.begin(9600);
  while (!Serial);

  sCmd.addCommand("PING", pingHandler);
  sCmd.addCommand("ECHO", echoHandler);
  sCmd.addCommand("BPM", bpmHandler);
}

void checkSerial(){
  if (Serial.available() > 0)
  {
    sCmd.readSerial();
  }
}

void pingHandler () {
  Serial.println("PONG");
}

void echoHandler () {
  char *arg;
  arg = sCmd.next();
  if (arg != NULL)
    Serial.println(arg);
  else
    Serial.println("nothing to echo");
}

void bpmHandler(){
  Serial.println(BPM);
}
