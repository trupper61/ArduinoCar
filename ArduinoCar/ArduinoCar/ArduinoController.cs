using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoCar
{
    public class ArduinoController
    {
        private Socket socket;
        public bool IsConnected => socket != null && socket.Connected;
        public string LastError { get; private set; }
        
        public bool Connect()
        {
            try
            {

                IPAddress ip = IPAddress.Parse("192.168.4.1");
                IPEndPoint endpoint = new IPEndPoint(ip, 5555);

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endpoint);

                LastError = null;
                return true;
            }
            catch (Exception ex)
            {
                this.LastError = ex.Message;
                return false;
            }
        }
        private byte ConvertPower(int sliderValue)
        {
            sliderValue = Math.Clamp(sliderValue, 0, 100);
            return (byte)(130 + (sliderValue * 100 / 100)); // 130 + 0–100
        }
        public void SendCmd(byte cmd)
        {
            if (socket != null && socket.Connected)
            {
                byte[] package = new byte[] { cmd };
                socket.Send(package);
            }
        }
        public void SendCmd(byte cmd, int power)
        {
            if (socket != null && socket.Connected)
            {
                byte[] package = new byte[] { cmd, ConvertPower(power) };
                socket.Send(package);
            }
        }

        public void SendCmd(byte cmd, int powerLeft, int powerRight)
        {
            if (socket != null && socket.Connected)
            {
                byte[] package = new byte[] { cmd, ConvertPower(powerLeft), ConvertPower(powerRight) };
                socket.Send(package);
            }
        }
        public void Disconnect()
        {
            try
            {
                socket?.Close();
                socket = null;
            }
            catch { }
        }
    }
}
