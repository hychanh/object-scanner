//Project's Name: OS (Objects Scanner)
//Author: Shouru Spepirus
//2016 © Titanlossus

// Khai bao gia tri mac dinh cho IR
int defautIN=0;
int defautIN1=0;
int defautOUT=0;
int defautOUT1=0;

// Khai bao Port
int Thief_LED=8;
int ThiefMode_LED=9;
int CountMode_LED=10;
int Counted_LED=11;
int Relay=12;

//Chan Analog
int IN1_gate=14; //A0
int IN2_gate=15; //A1
int OUT1_gate=16; //A2
int OUT2_gate=17; //A3

//Tuy chinh ban dau
void setup() 
{
  Serial1.begin(9600);
  Serial.begin(9600);
  for (int i=8;i<=12;i++) 
  {
  pinMode(i,OUTPUT);
  digitalWrite(i,LOW);
  }
digitalWrite(CountMode_LED,HIGH);
sendMessage("THIEF:OFF");
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
unsigned long T=10; // time of delay
int IN1; //luu gia tri A0
int IN2; //luu gia tri A1
int OUT1; //luu gia tri A2
int OUT2; //luu gia tri A3

boolean Countingcheck=false;
int Status=2; //Trang thai so can gui: 1-IN ; 2-NOW; 3-OUT Mac dinh la 2-NOW

//IR Test Process
void IRprocessCommand(char* command) 
{
if(strcmp(command,"IRSYNC") == 0)
{
  sendMessage("IRSYNC:"+String(analogRead(IN1_gate))+ " & " + String(analogRead(IN2_gate))+":"+String(analogRead(OUT1_gate))+ " & " + String(analogRead(OUT2_gate)));
}
else
if(strcmp(command,"STOPIRTEST") == 0)
{
  sendMessage("IRTESTSTOP");
  IRT=false;
}
else
if (strcmp(command,"SETIRDEFAULT") == 0)
{
  
  sendMessage("GETINDEFAULT");
  while (INSET)
  {
     IRINDefault();
  }
  INSET=true;
  sendMessage("GETINDEFAULT1");
  while (INSET)
  {
     IRINDefault1();
  }
  INSET=true;
  sendMessage("GETOUTDEFAULT1");
  while (OUTSET)
  {
     IROUTDefault();
  }
  OUTSET=true;
  sendMessage("GETOUTDEFAULT2");
  while (OUTSET)
  {
     IROUTDefault1();
  }
  OUTSET=true;
  
  
 sendMessage("IRSYNC:"+String(defautIN)+" & "+String(defautIN1)+":"+String(defautOUT)+" & "+String(defautOUT1));
}
}

//IR Test Settings
void IRTest()
{
  sendMessage("STARTIRTEST");
  while(IRT)
  {
    RecivedBT();
  }
 }

//Process the IR Defaut value:
void IRINDefault()
{
   if (Serial1.available())
  {
    int commandSize = (int)Serial1.read();
      char command[commandSize];
      int commandPos = 0;
      while(commandPos < commandSize) {
        if(Serial1.available()) {
          command[commandPos] = (char)Serial1.read();
          commandPos++;
        }
      }
      command[commandPos] = 0;
     
      defautIN=String(command).toInt();
      INSET=false;
     
}
}

void IRINDefault1()
{
   if (Serial1.available())
  {
    int commandSize = (int)Serial1.read();
      char command[commandSize];
      int commandPos = 0;
      while(commandPos < commandSize) {
        if(Serial1.available()) {
          command[commandPos] = (char)Serial1.read();
          commandPos++;
        }
      }
      command[commandPos] = 0;
     
      defautIN1=String(command).toInt();
    INSET=false; 
}
}

void IROUTDefault()
{
  
   if (Serial1.available())
  {
    int commandSize = (int)Serial1.read();
      char command[commandSize];
      int commandPos = 0;
      while(commandPos < commandSize) {
        if(Serial1.available()) {
          command[commandPos] = (char)Serial1.read();
          commandPos++;
        }
      }
      command[commandPos] = 0;
 defautOUT=String(command).toInt();
      OUTSET=false;
}
}

void IROUTDefault1()
{
  
   if (Serial1.available())
  {
    int commandSize = (int)Serial1.read();
      char command[commandSize];
      int commandPos = 0;
      while(commandPos < commandSize) {
        if(Serial1.available()) {
          command[commandPos] = (char)Serial1.read();
          commandPos++;
        }
      }
      command[commandPos] = 0;
 defautOUT1=String(command).toInt();
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
  if (Thief==true)  sendMessage("THIEF:ON");
  else  sendMessage("THIEF:OFF");
  if (Countingcheck)  sendMessage("COUNTING:ON");
  else sendMessage("COUNTING:OFF");
}

// Break all cycles
void Breakall()
{
  digitalWrite(Thief_LED,LOW);
  digitalWrite(ThiefMode_LED,LOW);
  digitalWrite(CountMode_LED,HIGH);
  sendMessage("STOPEDTHIEF");
  Stop=true;
  Thief=false; 
  IRT=false;
  Countingcheck=false;
  digitalWrite(Relay,LOW);
  sendMessage("ALLBREAKED");
  sendMessage("THIEF:OFF");
  
}
int AUTO_IN1,AUTO_IN2,AUTO_OUT1,AUTO_OUT2;
//Xu li lenh nhan tu Bluetooth
void processCommand(char* command) 
{
  if(strcmp(command,"THIEF") == 0) { //Chuyen sang mode thief
    Thief=true;
    Countingcheck=false;
    sendMessage("THIEF:ON");
    digitalWrite(ThiefMode_LED,HIGH);
    digitalWrite(CountMode_LED,LOW);
  }
  else
  if(strcmp(command,"UPDATENUM") == 0) //Update Number cho Phone
  {
    SendNum();
  }
  else
  if(strcmp(command,"COUNT") == 0) //Chuyen mode Count;
  {
    Thief=false;
    sendMessage("THIEF:OFF");
    digitalWrite(ThiefMode_LED,LOW);
    digitalWrite(CountMode_LED,HIGH);
  }
  else
  if(strcmp(command,"RESETNUM") == 0) //Number=0;
  {
   IN_Num=0; OUT_Num=0;
   SendNum();
  } 
  else
  if(strcmp(command,"DECSTOP") == 0) //Stop the thief detecting...
  {
    Stop=true;
    Thief=false;
    sendMessage("THIEF:OFF");
    sendMessage("STOPEDTHIEF");
  } 
  else   if(strcmp(command,"TURNONCOUNT") == 0)
  {
    Countingcheck=true;
    sendMessage("COUNTING:ON");
  }
  else if(strcmp(command,"IN") == 0)
  {
    Status=1;
    SendNum();
  }
  else if(strcmp(command,"NOW") == 0)
  {
    Status=2;
    SendNum();
  }
  else if(strcmp(command,"OUT") == 0)
  {
    Status=3;
    SendNum();
  }
  else if(strcmp(command,"SYNC") == 0)
  {
    Sync();
  }
  else if(strcmp(command,"IRTEST") == 0)
  {
    IRT=true;
    IRTest();
  }
  else if (strcmp(command,"IRAUTOSET")==0)
  {
    AUTO_IN1=analogRead(IN1_gate);
    AUTO_IN2=analogRead(IN2_gate);
    AUTO_OUT1=analogRead(OUT1_gate);
    AUTO_OUT2=analogRead(OUT2_gate);
   sendMessage("IRAUTOSET_FIRST:"+String(AUTO_IN1)+ ":" + String(AUTO_IN2)+":"+String(AUT0_OUT1)+ ":" + String(AUTO_OUT2));
   digitalWrite(Counted_LED,HIGH);
  }
   else if (strcmp(command,"IRAUTOSET_FINAL")==0)
  {
    while (AUTO_IN1
     sendMessage("IRAUTOSET_FINAL:"+String(defautIN)+":"+String(defautIN1)+":"+String(defautOUT)+":"+String(defautOUT1));
  }
}

//Gui lenh tu bluetooth
void sendMessage(String message) 
{
  int messageLen = message.length();
  if(messageLen < 256) {  
    Serial1.write(messageLen);
    Serial1.print(message);
  }
}

//Nhan lenh tu bluetooth
void RecivedBT()
{
   if (Serial1.available())
  {
    int commandSize = (int)Serial1.read();
      char command[commandSize];
      int commandPos = 0;
      while(commandPos < commandSize) {
        if(Serial1.available()) {
          command[commandPos] = (char)Serial1.read();
          commandPos++;
        }
      }
      command[commandPos] = 0;
      if(strcmp(command,"BREAKALL") == 0)
      {
        Breakall();
      }
      else
      if (IRT==true) IRprocessCommand(command);
      else      processCommand(command);
  
  }
}

//Mode 1: Count
void Counting()
{
  //Doc gia tri IR cong IN va OUT
 IN1=analogRead(IN1_gate);
 IN2=analogRead(IN2_gate);
 
 OUT1=analogRead(OUT1_gate);
 OUT2=analogRead(OUT2_gate);
 
 //Two gates detected
 if ((IN1<defautIN) && (IN2<defautIN1) && (OUT1<defautOUT) && (OUT2<defautOUT1))
 {
   while (true)
   {
      RecivedBT();
    if (Countingcheck==false)
    {
      break;
    }
   if ((IN1>defautIN) && (IN2>defautIN1) && (OUT1>defautOUT) && (OUT2>defautOUT1))
      {
        break;
      }  
    delay(T);
   }
 }
 else
 
 //IN GATE DETECTED
 if ((IN1<defautIN) && (IN2<defautIN1))
 {
    while (true)
   {
     RecivedBT();
     if (Countingcheck==false)
    {
      digitalWrite(Counted_LED,LOW);
      break;
    }
    IN1=analogRead(IN1_gate);
    IN2=analogRead(IN2_gate);
    
    OUT1=analogRead(OUT1_gate);
    OUT2=analogRead(OUT2_gate);
      if ((OUT1<defautOUT) && (OUT2<defautOUT1))
      {
        while (true)
        {
         RecivedBT();
          if (Countingcheck==false)
    {   
      break;
    }
    IN1=analogRead(IN1_gate);
    IN2=analogRead(IN2_gate);
    OUT1=analogRead(OUT1_gate);
    OUT2=analogRead(OUT2_gate);
    if ((IN1>defautIN) && (IN2>defautIN1) && (OUT1>defautOUT) && (OUT2>defautOUT1))
      {
        break;
      }  
      delay(T);
      }
   }
    else
    {
      digitalWrite(Counted_LED,HIGH);
     if ((IN1>defautIN) && (IN2>defautIN1))
      {
       IN_Num++;
       SendNum();
       digitalWrite(Counted_LED,LOW);
       break;
     }
     delay(T);
   }
   }
 }
 else
 
 //OUT GATE DETECTED
  if ((OUT1<defautOUT) && (OUT2<defautOUT1)) 
 {
   while (true)
   {
    OUT1=analogRead(OUT1_gate);
    OUT2=analogRead(OUT2_gate);
    IN1=analogRead(IN1_gate);
    IN2=analogRead(IN2_gate);
      if ((IN1<defautIN) && (IN2<defautIN1))
      {
        while (true)
        {
         RecivedBT();
          if (Countingcheck==false)
          {   
              break;
           }
    IN1=analogRead(IN1_gate);
    IN2=analogRead(IN2_gate);
    OUT1=analogRead(OUT1_gate);
    OUT2=analogRead(OUT2_gate);
    if ((IN1>defautIN) && (IN2>defautIN1) && (OUT1>defautOUT) && (OUT2>defautOUT1))
      {
        break;
      }  
      delay(T);
      }
   }
    else
    { 
     digitalWrite(Counted_LED,HIGH);
     RecivedBT();
     if (Countingcheck==false)
    {
         digitalWrite(Counted_LED,LOW);
      break;
    }
   if ((OUT1>defautOUT) && (OUT2>defautOUT1))
      {
       OUT_Num++;
       SendNum();
       digitalWrite(Counted_LED,LOW);
       break;
      }
     }
     delay(T);
   }
 }
 
  delay(T);
}

//Mode 2: Thief
void ThiefDec()
{
  
 IN1=analogRead(IN1_gate);
  IN2=analogRead(IN2_gate);
     OUT1=analogRead(OUT1_gate);
    OUT2=analogRead(OUT2_gate);
  if ((IN1<defautIN) || (IN2<defautIN1) || (OUT1<defautOUT) || (OUT2<defautOUT1))
  {
   digitalWrite(Thief_LED,HIGH);
   sendMessage("THIEFDEC:ON");
   digitalWrite(Relay,HIGH);
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
    delay(T);
   }
 }
 delay(T);
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
