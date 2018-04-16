using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using OS_Model_Controller.Resources;
using Windows.Networking.Sockets;
using Windows.Networking.Proximity;
using System.Diagnostics;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using BluetoothConnectionManager;
using System.Windows.Media;
using Microsoft.Devices;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Phone.Tasks;

namespace OS_Model_Controller
{
    public partial class MainPage : PhoneApplicationPage
    {
        int IN_Value1 = 0;
        int OUT_Value1 = 0;
        double A_IN_Value = 0;
        double A_OUT_Value = 0;

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
                case "CONNECTED":
                    Maingrid.Visibility = Visibility.Visible;
                    First_GRID.Visibility = Visibility.Collapsed;
                    break;
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
              
                case "GETINDEFAULT":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        if (Autoset_Place.Visibility == Visibility.Collapsed)
                        {
                            if (IRIN_VALUE.Text == "") sendCmd("1050");
                            else sendCmd(IRIN_VALUE.Text);
                        }
                        else sendCmd(A_IN_Value.ToString());

                    });
                    break;

                case "GETOUTDEFAULT":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        if (Autoset_Place.Visibility == Visibility.Collapsed)
                        {
                            if (IROUT_VALUE.Text == "") sendCmd("1050");
                            else sendCmd(IROUT_VALUE.Text);
                        }
                        else sendCmd(A_OUT_Value.ToString());
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

                case "AUTOVALUE1":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        IN_Value1 = int.Parse(messageArray[1]);
                        OUT_Value1 = int.Parse(messageArray[2]);
                        Auto_Text1.Visibility = Visibility.Visible;
                        Auto_Text.Visibility = Visibility.Visible;
                    });
                    break;

                case "AUTOVALUE2_IN":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        float Temp = (IN_Value1 + int.Parse(messageArray[1])) / 2;
                        A_IN_Value = Math.Round(Temp, 0)+3;
                        Auto_Text1.Text = "IN Detected";
                        Auto_Text.Text = "IN:" + A_IN_Value.ToString();
                    });
                    break;
                case "AUTOVALUE2_OUT":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        float Temp = (OUT_Value1 + int.Parse(messageArray[1])) / 2;
                        A_OUT_Value = Math.Round(Temp, 0)+3;
                        Auto_Text1.Text = "OUT Detected";
                        Auto_Text.Text = "OUT:" + A_OUT_Value.ToString();

                    });
                    break;
                case "AUTOIRTEST_ON":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        Autoset_Place.Visibility = Visibility.Visible;
                        IR_Testplace.Visibility = Visibility.Collapsed;
                    });
                    break;
                case "GETIRDEFAULT":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        Auto_Text.Text = "";
                        Auto_Text1.Text = "COMPLETED!";
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
        bool Connectable = true;
        private void ConnectAppToDeviceButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (Connectable) AppToDevice();
        }

        private async void AppToDevice()
        {
            Connectable = false;
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
            Connectable = true;
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
           
            sendCmd("AUTOVALUE1");
            Auto_Text.Text = "Detecting Objects...";
            Auto_Text1.Text = "Please stand in the door";
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
            Auto_Text1.Visibility = Visibility.Collapsed;
            Auto_Text.Visibility = Visibility.Collapsed;
            Autoset_Place.Visibility = Visibility.Collapsed;
            IR_Testplace.Visibility = Visibility.Visible;
            Auto_Text1.Text = "";
            Auto_Text.Text = "";
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
    }
}