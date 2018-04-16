//Project's Name: OS Model(Objects Scanner Model)
//Author: Shouru Spepirus
//Version: 3.0
//2016 © Titanlossus

// Khai bao gia tri mac dinh cho IR
int defautIN=1050;
int defautOUT=1050;

// Khai bao Port
#define Thief_LED 8
#define ThiefMode_LED 10
#define CountMode_LED 11
#define Counted_LED 12
#define Relay 2
#define IN_gate A6
#define OUT_gate A5
const int Delta=8;

//Tuy chinh ban dau
void setup()
{
  Serial.begin(9600);
  for (int i=2;i<=12;i++) 
  {
  pinMode(i,OUTPUT);
  digitalWrite(i,LOW);
  }
  
  //Cấp điện cho IR Reciever
  digitalWrite(3,HIGH);
  digitalWrite(4,HIGH);
}

//Khai bao bien:
int IN_Num=0; //Number of in Objects
int OUT_Num=0; //Number of out Objects
int NOW_Num=0; //Number of now objects
boolean Stop=false; //Stop thief dec
boolean Thief=false; //Chuyen che do count/thief
boolean IRT=false; //Chuyen doi sang IR test
boolean INSET=true; //Cho lennh update defautIN
boolean OUTSET=true; //Cho lenh update defautOUT
unsigned long T=200; // time of delay
int IN; //Lưu giá trị A0
int OUT; //Lưu giá trị A1
int IN_Temp; //Lưu giá trị A0 để so sánh
int OUT_Temp; //Lưu giá trị A1 để so sánh
boolean Countingcheck=true;
int Status=2; //Trang thai so can gui: 1-IN ; 2-NOW; 3-OUT Mac dinh la 2-NOW
boolean AIRT=false;

//IR Auto Setting
void IRAuto()
{
  boolean INed=false;
  boolean OUTed=false;
while (AIRT)
{
  RecivedBT();
  delay(T);
  int IN_V=analogRead(IN_gate);
  int OUT_V=analogRead(OUT_gate);
  if ((IN_V-IN_Temp)>Delta)
  {
  sendMessage("AUTOVALUE2_IN:"+String(IN_V));
  INed=true;
  }
   if ((OUT_V-OUT_Temp)>Delta)
  {
  sendMessage("AUTOVALUE2_OUT:"+String(OUT_V));
  OUTed=true;
  }
  if ((OUTed) && (INed)) break;
}
delay(T);
sendMessage("GETIRDEFAULT");
}

//IR Test Process
void IRprocessCommand(String command) 
{
  if (command=="STOPAUTOIR")
  {
  AIRT=false;
  }
 if(command=="AUTOIRTEST")
{
  sendMessage("AUTOIRTEST_ON");
  AIRT=true;
}
else
 if(command=="AUTOVALUE1")
{
  IN_Temp=analogRead(IN_gate);
  OUT_Temp=analogRead(OUT_gate);
  sendMessage("AUTOVALUE1:"+String(IN_Temp)+":"+String(OUT_Temp));
  IRAuto();
}
else
if(command=="IRSYNC")
{
  sendMessage("IRSYNC:"+String(analogRead(IN_gate))+":"+String(analogRead(OUT_gate)));
}
else
if(command=="STOPIRTEST")
{
  digitalWrite(Counted_LED,LOW);
  digitalWrite(Thief_LED,LOW);
  digitalWrite(ThiefMode_LED,LOW);
  sendMessage("IRTESTSTOP");
  IRT=false;
} 
 else if(command=="SYNC")
  {
    Sync();
  }
else
if (command=="SETIRDEFAULT")
{
  sendMessage("GETINDEFAULT");
  while (INSET)
  {
     IRINDefault();
  }
  INSET=true;
 
  sendMessage("GETOUTDEFAULT");
  while (OUTSET)
  {
     IROUTDefault();  
  }
  OUTSET=true;
  sendMessage("IRSYNC:"+String(defautIN)+":"+String(defautOUT));
}
else if(command=="CONNECTED")
{
  sendMessage("CONNECTED");
}
}

//IR Test Settings
void IRTest()
{
  sendMessage("STARTIRTEST");
  digitalWrite(CountMode_LED,HIGH);
  digitalWrite(Counted_LED,HIGH);
  digitalWrite(Thief_LED,HIGH);
  digitalWrite(ThiefMode_LED,HIGH);
  delay(T);
  while(IRT)
  {
    RecivedBT();
  }
  
 }

//Process the IR Defaut value:
void IRINDefault()
{
   if (Serial.available())
  {
    int commandSize = (int)Serial.read();
      String command;
      int commandPos = 0;
      while(commandPos < commandSize) {
        if(Serial.available()) {
          command= command + (char)Serial.read();
          commandPos++;
        }
      }
      defautIN=(command).toInt();
      INSET=false;
     
}
}

void IROUTDefault()
{
  
   if (Serial.available())
  {
    int commandSize = (int)Serial.read();
      String command;
      int commandPos = 0;
      while(commandPos < commandSize) {
         if(Serial.available()) {
         command=command + (char)Serial.read();
          commandPos++;
        }
      }
   
 defautOUT=String(command).toInt();
      OUTSET=false;
}
}

//Gui so qua bluetooth
void SendNum()
{
  NOW_Num=IN_Num-OUT_Num;
  String Num="NEW:";
    switch (Status)
    {
      case 1: Num=Num+String(IN_Num)+":IN:"; break;
      case 2: Num=Num+String(NOW_Num)+":NOW:"; break;
      case 3: Num=Num+String(OUT_Num)+":OUT:"; break;
    }
  Num=Num+"ON";
  sendMessage(Num); 
}

//Dong bo hoa voi dien thoai:
void Sync()
{
  if (Thief)  sendMessage("THIEF:ON");
  else  sendMessage("THIEF:OFF");
  if (Countingcheck) sendMessage("COUNTING:ON");
  else sendMessage("COUNTING:OFF");
  if (IRT) sendMessage("IRT_ON");
}

// Break all cycles
void Breakall()
{
  digitalWrite(Thief_LED,LOW);
  digitalWrite(Counted_LED,LOW);
  digitalWrite(ThiefMode_LED,LOW);
  digitalWrite(CountMode_LED,HIGH);
  digitalWrite(Relay,LOW);
  sendMessage("STOPEDTHIEF");
  Stop=true;
  Thief=false; 
  IRT=false;
  AIRT=false;
  sendMessage("ALLBREAKED");
  sendMessage("THIEF:OFF");
  
}
//Xu li lenh nhan tu Bluetooth
void processCommand(String command) 
{
  if(command=="CONNECTED")
  {
      sendMessage("CONNECTED");
  }
  else if(command=="THIEF") { //Chuyen sang mode thief
    Thief=true;
    Countingcheck=false;
    sendMessage("THIEF:ON");
    digitalWrite(ThiefMode_LED,HIGH);
    digitalWrite(CountMode_LED,LOW);
  }
    else if(command=="IRTEST")
  {
    IRT=true;
    IRTest();
  }
  else
  if(command=="UPDATENUM")//Update Number cho Phone
  {
    SendNum();
  }
  else
  if(command=="COUNT")//Chuyen mode Count;
  {
    Thief=false;
    Countingcheck=true;
    sendMessage("THIEF:OFF");
    digitalWrite(ThiefMode_LED,LOW);
    digitalWrite(CountMode_LED,HIGH);
  }
  else
  if(command=="RESETNUM") //Number=0;
  {
   IN_Num=0; OUT_Num=0;
   SendNum();
  } 
  else
  if(command=="DECSTOP") //Stop the thief detecting...
  {
Stop=true;
Thief=false;
Countingcheck=true;
sendMessage("THIEF:OFF");
sendMessage("STOPEDTHIEF");
  } 
   else if (command=="IN")
  {
    Status=1;
    SendNum();
  }
  else if(command=="NOW")
  {
    Status=2;
    SendNum();
  }
  else if(command=="OUT")
  {
    Status=3;
    SendNum();
  }
  else if(command=="SYNC")
  {
    Sync();
  }
 
}

//Gui lenh tu bluetooth
void sendMessage(String message) 
{
  int messageLen = message.length();
  if(messageLen < 256) {  
    Serial.write(messageLen);
   Serial.print(message);
  }
}

//Nhan lenh tu bluetooth
void RecivedBT()
{
   if (Serial.available())
  {
    int commandSize = (int)Serial.read();
      String command;
      int commandPos = 0;
      while(commandPos < commandSize)
      {
        if(Serial.available()) {
          command= command + (char)Serial.read();
          commandPos++;
        }
      }
      if(command=="BREAKALL")
      {
        Breakall();
      }
      else
      if (IRT) IRprocessCommand(command);
      else     processCommand(command);
  
  }
}

//Mode 1: Count
void Counting()
{
 digitalWrite(CountMode_LED,HIGH);
 IN=analogRead(IN_gate); 
 OUT=analogRead(OUT_gate);
 
 //IN GATE DETECTED
 if (IN>defautIN && OUT<defautOUT) 
 {
 digitalWrite(Counted_LED,HIGH);
 while (Countingcheck)
  {
    RecivedBT();
    
    delay(T);
    IN=analogRead(IN_gate);
    OUT=analogRead(OUT_gate);
    if (OUT>defautOUT)
      {
        digitalWrite(Counted_LED,LOW);
        while (Countingcheck)
        {
         RecivedBT();
         delay(T);
         IN=analogRead(IN_gate);
         OUT=analogRead(OUT_gate);
         if ((OUT<defautOUT) && (IN<defautIN)) break;
        
        }
      }
    else
    {
       IN=analogRead(IN_gate);
         if (IN<defautIN)
     {
       digitalWrite(Counted_LED,LOW);
       IN_Num++;
       SendNum();
       delay(1000);
       break;
      }
    }
   }
  
 }
 else 
 
 //OUT GATE DETECTED
 if (OUT>defautOUT && IN<defautIN) 
 {
   digitalWrite(Counted_LED,HIGH);
   while (Countingcheck)
  {
  
    RecivedBT();
    delay(T);
    IN=analogRead(IN_gate);
    OUT=analogRead(OUT_gate);
    if (IN>defautIN)
      {
      digitalWrite(Counted_LED,LOW);
      while (Countingcheck)
        {
         RecivedBT();
         delay(T);
         IN=analogRead(IN_gate);
         OUT=analogRead(OUT_gate);
         if ((OUT<defautOUT) && (IN<defautIN)) break;
        }
      }
  else
  {
    OUT=analogRead(OUT_gate);
     if (OUT<defautOUT) 
     {
       OUT_Num++;
       SendNum();
       digitalWrite(Counted_LED,LOW);
       delay(T);
       break;
      }
       
   }

 }
 
}
}

//Mode 2: Thief
void ThiefDec()
{

  IN=analogRead(IN_gate);
  OUT=analogRead(OUT_gate);
  if ((IN>defautIN) || (OUT>defautOUT))
  {
   sendMessage("THIEFDEC:ON");
   digitalWrite(Relay,HIGH);
   digitalWrite(Thief_LED,HIGH);
   while (true)
   {  
     RecivedBT();
     if (Stop)
     {
      Stop=false;
      digitalWrite(Thief_LED,LOW);
      digitalWrite(Relay,LOW);
      break;
     }
   }
 }
}


//Main void
void loop() 
{
RecivedBT();

if (Thief)
{
ThiefDec();
}
else
if (Countingcheck) Counting();
}
