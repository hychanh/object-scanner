using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BluetoothClientWP8.Resources;
using Windows.Networking.Sockets;
using Windows.Networking.Proximity;
using System.Diagnostics;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using BluetoothConnectionManager;
using System.Windows.Media;
using Microsoft.Devices;
using Microsoft.Xna.Framework.Audio;
namespace BluetoothClientWP8
{
    public partial class MainPage : PhoneApplicationPage
    {
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
                        Status.Text = stateManager.Thief ?  "THIEF": "COUNT";
                        if (stateManager.Thief)
                        {
                            OSNum.Visibility = Visibility.Collapsed;
                            Count_Title.Visibility = Visibility.Collapsed;
                            Update_but.Visibility = Visibility.Collapsed;
                            Reset_but.Visibility = Visibility.Collapsed;
                            Thief_Title.Visibility = Visibility.Visible;
                          
                            IN_BUTTON.Visibility = Visibility.Collapsed;
                            OUT_BUT.Visibility = Visibility.Collapsed;
                            NOW_BUT.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            OSNum.Visibility = Visibility.Visible;
                            Count_Title.Visibility = Visibility.Visible;
                            Update_but.Visibility = Visibility.Visible;
                            Reset_but.Visibility = Visibility.Visible;
                            Thief_Title.Visibility = Visibility.Collapsed;
                           
                            IN_BUTTON.Visibility = Visibility.Visible;
                            OUT_BUT.Visibility = Visibility.Visible;
                            NOW_BUT.Visibility = Visibility.Visible;
                        }
                    });

                    break;
                case "NEW":
                   
                            stateManager.NEW = messageArray[3] == "ON" ? true : false;
                            Dispatcher.BeginInvoke(delegate()
                            {
                                OSNum.Text = stateManager.NEW ? messageArray[1] : "";

                                switch (messageArray[2])
                                {
                                    case "NOW":
                                        IN_BUTTON.IsEnabled = true;
                                        OUT_BUT.IsEnabled = true;
                                        NOW_BUT.IsEnabled = false;
                                        break;
                                    case "IN":
                                        IN_BUTTON.IsEnabled = false;
                                        OUT_BUT.IsEnabled = true;
                                        NOW_BUT.IsEnabled = true;
                                        break;
                                    case "OUT":
                                        IN_BUTTON.IsEnabled = true;
                                        OUT_BUT.IsEnabled = false;
                                        NOW_BUT.IsEnabled = true;
                                        break;
                                }
                            });
                break;
                case "CONNECTED":
                    stateManager.CONNECTION = messageArray[1] == "ON" ? true : false;
                    Dispatcher.BeginInvoke(delegate()
                    {
                      if(stateManager.CONNECTION==true) sendCmd("CONNECTED");
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
                case "COUNTING":
                    stateManager.COUNTING = messageArray[1] == "ON" ? true : false;
                    Dispatcher.BeginInvoke(delegate()
                    {
                        if (stateManager.COUNTING == true)
                        {
                            Turnoncount.IsChecked = true;
                            Turnoncount.Content = "Counting...";

                        }
                        else
                        {
                            Turnoncount.IsChecked = false;
                            Turnoncount.Content = "Uncounting...";          
                        }
                    });
                    break;
                case "IRSYNC":            
                    Dispatcher.BeginInvoke(delegate()
                    {
                        IRIN_Text.Text = messageArray[1];
                        IROUT_Text.Text= messageArray[2];                     

                    });
                    break;
                case "IRTESTSTOP":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        IR_Testplace.Visibility = Visibility.Collapsed;
                        Maingrid.Visibility = Visibility.Visible;

                    });
                    break;
                case "GETINDEFAULT":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        sendCmd(IRIN_VALUE.Text);

                    });
                    break;
                case "GETINDEFAULT1":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        sendCmd(IRIN_VALUE1.Text);

                    });
                    break;
                case "GETOUTDEFAULT":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        sendCmd(IROUT_VALUE.Text);

                    });
                    break;
                case "GETOUTDEFAULT1":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        sendCmd(IROUT_VALUE1.Text);

                    });
                    break;
                case "STARTIRTEST":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        Maingrid.Visibility = Visibility.Collapsed;
                        IR_Testplace.Visibility = Visibility.Visible;

                    });
                    break;
                case "ALLBREAKED":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        Maingrid.Visibility = Visibility.Visible;
                        IR_Testplace.Visibility = Visibility.Collapsed;
                        Thief_Detec.Visibility = Visibility.Collapsed;
                        Rung.Stop();
                        Warningup.Stop();

                    });
                    break;
                case "COUNTCHECKOFF":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        Turnoncount.IsChecked = false;
                        Turnoncount.Content = "Uncounting...";
                    });
                    break;
                case "STOPEDTHIEF":
                    Dispatcher.BeginInvoke(delegate()
                    {
                        sendCmd("COUNT");
                    });
                    break; 

             }
        }

         public void  Thiefdetected()
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
            ConnectAppToDeviceButton.Content = "Connecting...";
            PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
            var pairedDevices = await PeerFinder.FindAllPeersAsync();

            if (pairedDevices.Count != 0)
                { 
                foreach (var pairedDevice in pairedDevices)
                {
                    if (pairedDevice.DisplayName == DeviceName.Text)
                    {
                        connectionManager.Connect(pairedDevice.HostName);
                        ConnectAppToDeviceButton.Content = "Connected";
                        DeviceName.IsReadOnly = true;
                        ConnectAppToDeviceButton.IsEnabled = false;
                        sendCmd("CONNECT");
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
            if (Status.Text=="COUNT")
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

        private void Count_Click(object sender, RoutedEventArgs e)
        {
            if (Turnoncount.IsChecked == true)
            {
                sendCmd("TURNONCOUNT");
            }
            else sendCmd("TURNOFFCOUNT");
        }

        private void NOW_BUT_Clicked(object sender, RoutedEventArgs e)
        {
            sendCmd("NOW");
        }

        private void IN_BUT(object sender, RoutedEventArgs e)
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
        }

        private void IRTEST_Clicked(object sender, RoutedEventArgs e)
        {
            sendCmd("IRTEST");
        }

       
        private void breack_Click(object sender, RoutedEventArgs e)
        {
            sendCmd("BREAKALL");

        }

        private void Exit_clicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Terminate();
        }

   
      
    }
}