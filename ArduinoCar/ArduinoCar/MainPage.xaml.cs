using System.Net;
using System.Net.Sockets;

namespace ArduinoCar
{
    public partial class MainPage : ContentPage
    {
        int leftSpeed = 0;
        int rightSpeed = 0;

        public MainPage()
        {
            InitializeComponent();
            UpdateUI();
        }

        //private void On_Clicked(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        IPAddress ardionoIP = IPAddress.Parse("192.168.4.1");
        //        IPEndPoint endPoint = new IPEndPoint(ardionoIP, 5555);

        //        Socket mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //        mySocket.Connect(endPoint);
        //        byte[] outBuffer = new byte[] { 1 };

        //        mySocket.Send(outBuffer);
        //        mySocket.Shutdown(SocketShutdown.Both);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }
        //}

        //private void Off_Clicked(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        IPAddress ardionoIP = IPAddress.Parse("192.168.4.1");
        //        IPEndPoint endPoint = new IPEndPoint(ardionoIP, 5555);

        //        Socket mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //        mySocket.Connect(endPoint);
        //        byte[] outBuffer = new byte[] { 0 };

        //        mySocket.Send(outBuffer);
        //        mySocket.Shutdown(SocketShutdown.Both);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //}
        private void UpdateUI()
        {
            leftBar.Progress = (leftSpeed + 100) / 200.0;
            rightBar.Progress = (rightSpeed + 100) / 200.0;

            
        }
        private void Forward_Click(object sender, EventArgs e)
        {
            leftSpeed = 100;
            rightSpeed = 100;
            UpdateUI();
        }

        private void Backward_Click(object sender, EventArgs e)
        {
            leftSpeed = -100;
            rightSpeed = -100;
            UpdateUI();
        }

        private void Left_Click(object sender, EventArgs e)
        {
            leftSpeed = -50;
            rightSpeed = 50;
            UpdateUI();
        }

        private void Right_Click(object sender, EventArgs e)
        {
            leftSpeed = 50;
            rightSpeed = -50;
            UpdateUI();
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            leftSpeed = 0;
            rightSpeed = 0;
            UpdateUI();
        }
        private void SendCommand_Click(object sender, EventArgs e)
        {
            byte cmd = presetPicker.SelectedIndex switch
            {
                0 => 0, // Kreis links
                1 => 1, // Kreis rechts
                _ => 0
            };
            // send command
        }
    }
}
