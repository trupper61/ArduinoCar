using System.Net;
using System.Net.Sockets;

namespace ArduinoCar
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void On_Clicked(object sender, EventArgs e)
        {
            try
            {
                IPAddress ardionoIP = IPAddress.Parse("192.168.4.1");
                IPEndPoint endPoint = new IPEndPoint(ardionoIP, 5555);

                Socket mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mySocket.Connect(endPoint);
                byte[] outBuffer = new byte[] { 1 };

                mySocket.Send(outBuffer);
                mySocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void Off_Clicked(object sender, EventArgs e)
        {
            try
            {
                IPAddress ardionoIP = IPAddress.Parse("192.168.4.1");
                IPEndPoint endPoint = new IPEndPoint(ardionoIP, 5555);

                Socket mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mySocket.Connect(endPoint);
                byte[] outBuffer = new byte[] { 0 };

                mySocket.Send(outBuffer);
                mySocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
