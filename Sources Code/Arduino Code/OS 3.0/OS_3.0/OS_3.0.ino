//Project's Name: OS (Objects Scanner)
//Version: 3.0
//Author: Shouru Spepirus
//2016 © Titanlossus

// Khai bao gia tri mac dinh cho IR
int defaultIN1=-1;
int defaultIN2=-1;
int defaultOUT1=-1;
int defaultOUT2=-1;

// Khai bao Port
#define Thief_LED 8
#define ThiefMode_LED 9
#define CountMode_LED 10
#define Counted_LED 11
#define Relay 12

int  IN1_gate=14; //A0
int  IN2_gate=15; //A1
int  OUT1_gate=16; //A2
int  OUT2_gate=17; //A3

const int Delta=8;

//Khai bao bien:
int IN_Num=0; //Number of in Objects
int OUT_Num=0; //Number of out Objects
int NOW_Num=0; //Number of now objects
boolean Stop=false; //Stop thief dec
boolean Thief=false; //Chuyen che do count/thief
boolean IRT=false; //Chuyen doi sang IR test
boolean INSET=true; //Cho lenh update defautIN
boolean OUTSET=true; //Cho lenh update defautOUT
unsigned long T=40; // time of delay
int IN1; //luu gia tri A0
int IN2; //luu gia tri A1
int OUT1; //luu gia tri A2
int OUT2; //luu gia tri A3
boolean AIRT=false;
boolean Countingcheck=true;
int Status=2; //Trang thai so can gui: 1-IN ; 2-NOW; 3-OUT Mac dinh la 2-NOW


//Tuy chinh ban dau
void setup()
{

  Serial1.begin(9600);
  for (int i=8;i<=12;i++) 
  {
    pinMode(i,OUTPUT);
    digitalWrite(i,LOW);
  }
  digitalWrite(CountMode_LED,HIGH);
  sendMessage("THIEF:OFF");
}

void RecivedBT();

//Gui lenh tu bluetooth
void sendMessage(String message) 
{
  int messageLen = message.length();
  if(messageLen < 256)
  {  
     Serial1.write(messageLen);
     Serial1.print(message);
  }
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

//Xu li lenh nhan tu Bluetooth
void processCommand(String command) 
{
  //Chuyen sang mode thief
  if(command=="THIEF")
  { 
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

//Recieve IR IN1 default value from Phone
void IRIN1Default()
{
   if (Serial1.available())
  {
    int commandSize = (int)Serial1.read();
    String command;
    int commandPos = 0;
    while(commandPos < commandSize)
    {
      if(Serial1.available())
      {
        command= command + (char)Serial1.read();
        commandPos++;
      }
    }
    Serial.print(command);
    defaultIN1=(command).toInt();
    INSET=false;
  }
}


//Recieve IR IN1 default value from Phone
void IRIN2Default()
{
   if (Serial1.available())
  {
    int commandSize = (int)Serial1.read();
    String command;
    int commandPos = 0;
    while(commandPos < commandSize)
    {
      if(Serial1.available())
      {
        command= command + (char)Serial1.read();
        commandPos++;
      }
    }
    defaultIN2=(command).toInt();
    INSET=false;
  }
}

//Recieve IR OUT1 default value from Phone
void IROUT1Default()
{
  if (Serial1.available())
  {
    int commandSize = (int)Serial1.read();
    String command;
    int commandPos = 0;
    while(commandPos < commandSize)
    {
      if(Serial1.available()) 
      {
        command=command + (char)Serial1.read();
        commandPos++;
      }
    }
    defaultOUT1=String(command).toInt();
    
    OUTSET=false;
  }
}

//Recieve IR OUT2 default value from Phone
void IROUT2Default()
{
  if (Serial1.available())
  {
   int commandSize = (int)Serial1.read();
   String command;
   int commandPos = 0;
   while(commandPos < commandSize) 
   {
     if(Serial1.available())
     {
      command=command + (char)Serial1.read();
      commandPos++;
     }
   }
   defaultOUT2=String(command).toInt();
   OUTSET=false;
  }
}

int IN1_Temp;
int IN2_Temp;
int OUT1_Temp;
int OUT2_Temp;
int IN1_V,IN2_V,OUT1_V,OUT2_V;

//IR Auto Setting
void IRAutoIN()
{
  while (AIRT)
  {
    delay(T);
    RecivedBT();
    IN1_V = (int) analogRead(IN1_gate);
    IN2_V = (int) analogRead(IN2_gate);

    if ((IN1_Temp-IN1_V)>Delta && (IN2_Temp - IN2_V)>Delta )
    {
      sendMessage("AUTOVALUE2_IN:"+String(IN1_V)+":"+String(IN2_V));
      break;
    }
  }
}

void IRAutoOUT()
{
  while (AIRT)
  {
    delay(T);
    RecivedBT();
    OUT1_V = analogRead(OUT1_gate);
    OUT2_V = analogRead(OUT2_gate);

    if ((OUT1_Temp-OUT1_V)>Delta && (OUT2_Temp - OUT2_V)>Delta )
    {
      sendMessage("AUTOVALUE2_OUT:"+String(OUT1_V)+":"+String(OUT2_V));
      break;
    }
 delay(500);
 sendMessage("GETIRDEFAULT");
  }
}

//IR Test Process
void IRprocessCommand(String command) 
{
  if (command=="STOPAUTOIR") AIRT=false;
  else if(command=="AUTOIRTEST")
  {
   sendMessage("AUTOIRTEST_ON");
  }
  else if(command=="GETAUTOVALUE1_IN")
  {
    IN1_Temp=analogRead(IN1_gate);
    IN2_Temp=analogRead(IN2_gate);
    sendMessage("AUTOVALUE1_IN:"+String(IN1_Temp)+":"+String(IN2_Temp));
    IRAutoIN();
  }
  
  else if (command=="GETAUTOVALUE1_OUT")
  {
    OUT1_Temp=analogRead(IN1_gate);
    OUT2_Temp=analogRead(IN2_gate);
    sendMessage("AUTOVALUE1_OUT:"+String(OUT1_Temp)+":"+String(OUT2_Temp));
  }
  else if (command=="GETAUTOVALUE2_IN") IRAutoIN();
  else if (command=="GETAUTOVALUE2_OUT") IRAutoOUT();
  else if(command=="IRSYNC") sendMessage("IRSYNC:"+String(analogRead(IN1_gate))+ " & " + String(analogRead(IN2_gate))+":"+String(analogRead(OUT1_gate))+ " & " + String(analogRead(OUT2_gate)));
  else if(command=="STOPIRTEST")
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
  else if (command=="SETIRDEFAULT")
  {
    //Get default value for IN1
    INSET=true;
    sendMessage("GETIN1DEFAULT");
    while (INSET)
    {
       IRIN1Default();
    }
    
    //Get default value for IN2
    INSET=true;
    sendMessage("GETIN2DEFAULT");
    while (INSET)
    {
       IRIN2Default();
    }
    INSET=true;
    
    //Get default value for OUT1
    sendMessage("GETOUT1DEFAULT");
    while (OUTSET)
    {
       IROUT1Default();
    }
    
    //Get default value for OUT2
    OUTSET=true;
    sendMessage("GETOUT2DEFAULT");
    while (OUTSET)
    {
       IROUT2Default();
    }
    OUTSET=true;
    
    sendMessage("IRSYNC:"+String(defaultIN1)+" & "+String(defaultIN2)+":"+String(defaultOUT1)+" & "+String(defaultOUT2));
  }
}

//Nhan lenh tu bluetooth
void RecivedBT()
{
   if (Serial1.available())
  {
    int commandSize = (int)Serial1.read();
      String command;
      int commandPos = 0;
      while(commandPos < commandSize)
      {
        if(Serial1.available())
        {
          command= command + (char)Serial1.read();
          commandPos++;
        }
      }
     
      if(command=="BREAKALL")
      {
        Breakall();
      }
      else
      if    (IRT) IRprocessCommand(command);
      else   processCommand(command);
  }
}

//Mode 1: Count
void Counting()
{
 digitalWrite(CountMode_LED,HIGH);
 IN1=analogRead(IN1_gate);
 IN2=analogRead(IN2_gate);
 OUT1=analogRead(OUT1_gate);
 OUT2=analogRead(OUT2_gate);
 
  //2 gates detected (rarely meet)
  if (IN1<defaultIN1 && IN2<defaultIN2 && OUT1<defaultOUT1 && OUT2<defaultOUT2)
  while (true)
  {
   delay(T);
   RecivedBT();
   if (analogRead(IN1_gate)> defaultIN1 && analogRead(IN2_gate)>defaultIN2 && analogRead(OUT1_gate)>defaultOUT1 && analogRead(OUT2_gate)>defaultOUT2 ) break;
  }
  else
 //IN GATE DETECTED
 if (IN1<defaultIN1 && IN2<defaultIN2) 
 {
  digitalWrite(Counted_LED,HIGH);
  while (Counting)
  {
    delay(T);
    RecivedBT();

    IN1=analogRead(IN1_gate);
    IN2=analogRead(IN2_gate);
    OUT1=analogRead(OUT1_gate);
    OUT2=analogRead(OUT2_gate);

    //OUT gate detected while IN gate not through
    if (OUT1<defaultOUT1 && OUT2<defaultOUT2)
    {
      digitalWrite(Counted_LED,LOW);
      while (Counting)
      {
        delay(T);
        RecivedBT();
        if (analogRead(IN1_gate)> defaultIN1 && analogRead(IN2_gate)>defaultIN2 && analogRead(OUT1_gate)>defaultOUT1 && analogRead(OUT2_gate)>defaultOUT2 ) break;
      }
    }
    else if (IN1> defaultIN1 && IN2>defaultIN2)
    {
        digitalWrite(Counted_LED,LOW);
        IN_Num++;
        SendNum();
        delay(T);
        break;
    }
  }
}
 else
 //OUT GATE DETECTED
 if (OUT1<defaultOUT1 && OUT2<defaultOUT2) 
 {
  digitalWrite(Counted_LED,HIGH);
  while (Counting)
  {
    delay(T);
    RecivedBT();

    IN1=analogRead(IN1_gate);
    IN2=analogRead(IN2_gate);
    OUT1=analogRead(OUT1_gate);
    OUT2=analogRead(OUT2_gate);

    //IN gate detected while OUT gate not through
    if (IN1<defaultIN1 && IN2<defaultIN2)
    {
      digitalWrite(Counted_LED,LOW);
      while (Counting)
      {
        delay(T);
        RecivedBT();
        if (analogRead(IN1_gate)> defaultIN1 && analogRead(IN2_gate)>defaultIN2 && analogRead(OUT1_gate)>defaultOUT1 && analogRead(OUT2_gate)>defaultOUT2 ) break;
      }
    }
    else if (OUT1>defaultOUT1 && OUT2>defaultOUT2)
    {
        digitalWrite(Counted_LED,LOW);
        OUT_Num++;
        SendNum();
        delay(T);
        break;
    }
  }
 }
}

//Mode 2: Thief
void ThiefDec()
{

  IN1=analogRead(IN1_gate);
  IN2=analogRead(IN2_gate);
  OUT1=analogRead(OUT1_gate);
  OUT2=analogRead(OUT2_gate);
  if ((IN1<defaultIN1) || (IN2<defaultIN2) || (OUT1<defaultOUT1) || (OUT2<defaultOUT2))
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
  else if (Countingcheck) Counting();
}
