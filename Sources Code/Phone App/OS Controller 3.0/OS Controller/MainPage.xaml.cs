using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using OS_Controller.Resources;
using Windows.Networking.Sockets;
using Windows.Networking.Proximity;
using System.Diagnostics;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using OS_Controller;
using System.Windows.Media;
using Microsoft.Devices;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Phone.Tasks;

namespace OS_Controller
{
    public partial class MainPage : PhoneApplicationPage
    {
        int IN_Value1 = 0;
        int IN_Value2 = 0;
        int OUT_Value1 = 0;
        int OUT_Value2 = 0;
        double A_IN_Value = 0;
        double A_OUT_Value = 0;
        double A_IN_Value2 = 0;
        double A_OUT_Value2= 0;

        private ConnectionManager connectionManager;

        private StateManager stateManager;
        public VibrateController Rung = VibrateController.Default;
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            connectionManager = new ConnectionManager();
            connectionManager.MessageReceived += connectionManager_MessageReceived;
            stateManager = new StateManager();
        }

        void connectionManager_MessageReceived(string message)
        {
            string[] messageArray = message.Split(':');
            switch (messageArray[0])
            {
                case "THIEF":

                    stateManager.Thief = messageArray[1] == "ON" ? true : false;
                    Dispatcher.BeginInvoke(delegate()
                    {
                        Status.Text = stateManager.Thief ? "THIEF" : "COUNT";
                        if (stateManager.Thief)
                        {
                            OSNum.Visibility = Visibility.Collapsed;
                            Count_Title.Visibility = Visibility.Collapsed;
                            Update_but.Visibility = Visibility.Collapsed;
                            Reset_but.Visibility = Visibility.Collapsed;
                            Thief_Title.Visibility = Visibility.Visible;
                            Sync.Visibility = Visibility.Collapsed;
                            IN_BUTTON.Visibility = Visibility.Collapsed;
                            OUT_BUT.Visibility = Visibility.Collapsed;
                            NOW_BUT.Visibility = Visibility.Collapsed;
                            Zoom_But.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            OSNum.Visibility = Visibility.Visible;
                            Count_Title.Visibility = Visibility.Visible;
                            Update_but.Visibility = Visibility.Visible;
                            Reset_but.Visibility = Visibility.Visible;
                            Thief_Title.Visibility = Visibility.Collapsed;
                            Sync.Visibility = Visibility.Visible;
                            IN_BUTTON.Visibility = Visibility.Visible;
                            OUT_BUT.Visibility = Visibility.Visible;
                            NOW_BUT.Visibility = Visibility.Visible;
                            Zoom_But.Visibility = Visibility.Visible;
                        }
                    });

                    break;
                case "NEW":

                    stateManager.NEW = messageArray[3] == "ON" ? true : false;
                    Dispatcher.BeginInvoke(delegate()
                    {
                        OSNum.Text = stateManager.NEW ? messageArray[1] : "";
                        OSNum_Zoom.Text = stateManager.NEW ? messageArray[1] : "";
                        switch (messageArray[2])
                        {
                            case "NOW":
                                IN_BUTTON.IsEnabled = true;
                                OUT_BUT.IsEnabled = true;
                                NOW_BUT.IsEnabled = false;
                                 IN_BUTTON_Zoom.IsEnabled = true;
                                 OUT_BUT_Zoom.IsEnabled = true;
                                 NOW_BUT_Zoom.IsEnabled = false;
                                break;
                            case "IN":
                                IN_BUTTON.IsEnabled = false;
                                OUT_BUT.IsEnabled = true;
                                NOW_BUT.IsEnabled = true;
                                 IN_BUTTON_Zoom.IsEnabled = false;
                                 OUT_BUT_Zoom.IsEnabled = true;
                                 NOW_BUT_Zoom.IsEnabled = true;
                                break;
                            case "OUT":
                                IN_BUTTON.IsEnabled = true;
                                OUT_BUT.IsEnabled = false;
                                NOW_BUT.IsEnabled = true;
                                IN_BUTTON_Zoom.IsEnabled = true;
                                OUT_BUT_Zoom.IsEnabled = false;
                                NOW_BUT_Zoom.IsEnabled = true;
                                break;
                        }
                    });
                    break;
                  case "THIEFDEC":
                    stateManager.THIEFDEC = messageArray[1] == "ON" ? true : false;
                    Dispatcher.BeginInvoke(delegate()
                    {
                        if (stateManager.THIEFDEC == true)
                        {
                            Thief_Detec.Visibility = Visibility.Visible;
                            Maingrid.Visibility = Visibility.Collapsed;
                            Thiefdetected();

                        }
                    });
                    break;

                case "ALLBREAKED":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        Maingrid.Visibility = Visibility.Visible;
                        Thief_Detec.Visibility = Visibility.Collapsed;
                        Rung.Stop();
                        Warningup.Stop();
                    });
                    break;
                case "IRSYNC":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        IRIN_Text.Text = messageArray[1];
                        IROUT_Text.Text = messageArray[2];
                    });
                    break;
              
                case "GETIN1DEFAULT":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        if (Autoset_Place.Visibility == Visibility.Collapsed)
                        {
                            if (IRIN_VALUE_1.Text == "") sendCmd("-1");
                            else sendCmd(IRIN_VALUE_1.Text);
                        }
                        else sendCmd(A_IN_Value.ToString());

                    });
                    break;
                case "GETIN2DEFAULT":
                    Dispatcher.BeginInvoke(delegate ()
                    {
                        if (Autoset_Place.Visibility == Visibility.Collapsed)
                        {
                            if (IRIN_VALUE_2.Text == "") sendCmd("-1");
                            else sendCmd(IRIN_VALUE_2.Text);
                        }
                        else sendCmd(A_IN_Value2.ToString());

                    });
                    break;
                case "GETOUT1DEFAULT":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        if (Autoset_Place.Visibility == Visibility.Collapsed)
                        {
                            if (IROUT_VALUE_1.Text == "") sendCmd("-1");
                            else sendCmd(IROUT_VALUE_1.Text);
                        } 
                        else sendCmd(A_OUT_Value.ToString());
                    });
                    break;
                case "GETOUT2DEFAULT":
                    Dispatcher.BeginInvoke(delegate ()
                    {
                        if (Autoset_Place.Visibility == Visibility.Collapsed)
                        {
                            if (IROUT_VALUE_2.Text == "") sendCmd("-1");
                            else sendCmd(IROUT_VALUE_2.Text);
                        }
                        else sendCmd(A_OUT_Value2.ToString());
                    });

                    break;
                case "STARTIRTEST":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        Maingrid.Visibility = Visibility.Collapsed;
                        IR_Testplace.Visibility = Visibility.Visible;

                    });
                    break;

                case "STOPEDTHIEF":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        sendCmd("COUNT");
                    });
                    break;

                case "AUTOVALUE1_IN":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        IN_Value1 = int.Parse(messageArray[1]);
                        IN_Value2 = int.Parse(messageArray[2]);
                        sendCmd("GETAUTOVALUE2_IN");
                    });
                    break;
                case "AUTOVALUE1_OUT":
                    Dispatcher.BeginInvoke(delegate ()
                    {
                        OUT_Value1 = int.Parse(messageArray[1]);
                        OUT_Value2 = int.Parse(messageArray[2]);
                        Auto_Text2.Visibility = Visibility.Visible;
                        Auto_Text3.Visibility = Visibility.Visible;

                    });
                    break;
                case "AUTOVALUE2_IN":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        double Temp = (IN_Value1 + int.Parse(messageArray[1]))*1.0 / 2;
                        A_IN_Value = Math.Round(Temp, 0)+3;
                        Temp = (IN_Value2 + int.Parse(messageArray[1])) *1.0 / 2;
                        A_IN_Value2 = Math.Round(Temp, 0)+3;
                        Auto_Text2.Text = "IN Detected";
                        Auto_Text3.Text = "IN:" + A_IN_Value.ToString();
                        But_Auto_Next.Visibility = Visibility.Visible;
                    });
                    break;
                case "AUTOVALUE2_OUT":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        double Temp = (OUT_Value1 + int.Parse(messageArray[1])) / 2;
                        A_OUT_Value = Math.Round(Temp, 0)+3;
                        Temp= (OUT_Value2 + int.Parse(messageArray[2])) / 2;
                        A_OUT_Value2 = Math.Round(Temp, 0)+3;
                        Auto_Text2.Text = "OUT Detected";
                        Auto_Text3.Text = "OUT:" + A_OUT_Value.ToString() + " & " + A_OUT_Value2.ToString();

                    });
                    break;
                case "AUTOIRTEST_ON":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        Autoset_Place.Visibility = Visibility.Visible;
                        But_Auto_Next.Visibility = Visibility.Collapsed;
                        IR_Testplace.Visibility = Visibility.Collapsed;
                    });
                    break;
                case "GETIRDEFAULT":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        Auto_Text3.Text = "";
                        Auto_Text2.Text = "COMPLETED!";
                        sendCmd("SETIRDEFAULT");
                    });
                    break;
                case  "IRT_ON":
                      Dispatcher.BeginInvoke(delegate()
                    {
                        Maingrid.Visibility = Visibility.Collapsed;
                        IR_Testplace.Visibility = Visibility.Visible;
                    });
                    break;
            }
        }

        public void Thiefdetected()
        {


            Rung.Start(TimeSpan.FromSeconds(5));
            Warningup.Play();
          
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            connectionManager.Initialize();
            stateManager.Initialize();
        
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            connectionManager.Terminate();
        }

        private void ConnectAppToDeviceButton_Click_1(object sender, RoutedEventArgs e)
        {
           
            AppToDevice();
        }

        private async void AppToDevice()
        {
             PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
            var pairedDevices = await PeerFinder.FindAllPeersAsync();

            if (pairedDevices.Count != 0)
            {
                foreach (var pairedDevice in pairedDevices)
                {
                    if (pairedDevice.DisplayName == DeviceName.Text)
                    {
                        connectionManager.Connect(pairedDevice.HostName);
                        Maingrid.Visibility = Visibility.Visible;
                        First_GRID.Visibility = Visibility.Collapsed;
                        continue;
                    }
                }
            }
        }

        async void sendCmd(string mess)
        {
            await connectionManager.SendCommand(mess);
        }

        private void Switch_click(object sender, RoutedEventArgs e)
        {
           
            if (Status.Text == "COUNT")
            {
                sendCmd("THIEF");

            }
            else
            {
                sendCmd("COUNT");

            }
        }

        private void Reset_clicked(object sender, RoutedEventArgs e)
        {
          sendCmd("RESETNUM");
        }

        private void Update_click(object sender, RoutedEventArgs e)
        {
            
            sendCmd("UPDATENUM");
        }

        private void Stop_Clicked(object sender, RoutedEventArgs e)
        {
            
            sendCmd("DECSTOP");
            Thief_Detec.Visibility = Visibility.Collapsed;
            Maingrid.Visibility = Visibility.Visible;
            Rung.Stop();
            Warningup.Stop();

        }

        private void NOW_BUT_Clicked(object sender, RoutedEventArgs e)
        {
          
            sendCmd("NOW");
        }

        private void IN_BUT_Clicked(object sender, RoutedEventArgs e)
        {
           
            sendCmd("IN");
        }

        private void OUT_BUT_CLICKED(object sender, RoutedEventArgs e)
        {
           
            sendCmd("OUT");
        }

        private void Sync_click(object sender, RoutedEventArgs e)
        {
          
            sendCmd("SYNC");
        }


        private void break_Click(object sender, RoutedEventArgs e)
        {
           
            sendCmd("BREAKALL");

        }

        private void Exit_clicked(object sender, RoutedEventArgs e)
        {
         
            Application.Current.Terminate();
        }

        private void But_Auto_start_Click(object sender, RoutedEventArgs e)
        {
           
            sendCmd("GETAUTOVALUE1_IN");
            But_Auto_start.Visibility = Visibility.Collapsed;
            Auto_Text2.Text = "Detecting Object...";
            Auto_Text1.Text = "Please go through the IN gate";
        }



        private void IRSync_Clicked(object sender, RoutedEventArgs e)
        {
            sendCmd("IRSYNC");
        }

        private void SET_clicked(object sender, RoutedEventArgs e)
        {
           
            sendCmd("SETIRDEFAULT");
        }

        private void Back_but_Click(object sender, RoutedEventArgs e)
        {
          
            sendCmd("STOPIRTEST");
            IR_Testplace.Visibility = Visibility.Collapsed;
            Maingrid.Visibility = Visibility.Visible;
        }

        private void IRTEST_Clicked(object sender, RoutedEventArgs e)
        {
          
            sendCmd("IRTEST");
        }

        private void Autoset_Clicked(object sender, RoutedEventArgs e)
        {
          
            sendCmd("AUTOIRTEST");
        }

        private void But_Auto_Back_Clicked(object sender, RoutedEventArgs e)
        {
         
            sendCmd("STOPAUTOIR");
            Auto_Text1.Text = "";
            Auto_Text2.Text = "";
            Auto_Text3.Text = "";
            Auto_Text1.Visibility = Visibility.Collapsed;
            Auto_Text2.Visibility = Visibility.Collapsed;
            Auto_Text3.Visibility = Visibility.Collapsed;
            Autoset_Place.Visibility = Visibility.Collapsed;
            IR_Testplace.Visibility = Visibility.Visible;
           
        }

        private void Zoom_back_Click(object sender, RoutedEventArgs e)
        {
          
            ZoomView.Visibility = Visibility.Collapsed;
            Maingrid.Visibility = Visibility.Visible;

        }

        private void Zoom_But_Click(object sender, RoutedEventArgs e)
        {
         
            Maingrid.Visibility = Visibility.Collapsed;
            ZoomView.Visibility = Visibility.Visible;
        }

        private void Warningup_MediaEnded(object sender, RoutedEventArgs e)
        {
            Warningup.Play();
        }

         private void GotoTL_Clicked(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            WebBrowserTask TSLink = new WebBrowserTask();
            TSLink.Uri = new Uri("https://www.facebook.com/groups/Titanlossus/");
            TSLink.Show();
        }

        private void But_Auto_Next_Click(object sender, RoutedEventArgs e)
        {
            Auto_Text1.Visibility = Visibility.Visible;
            Auto_Text2.Visibility = Visibility.Visible;
            sendCmd("GETAUTOVALUE1_OUT");
            Auto_Text2.Text = "Detecting Objects...";
            Auto_Text1.Text = "Please go through the OUT gate";
        }
    }
}