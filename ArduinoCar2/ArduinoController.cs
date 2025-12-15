using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArduinoCar2
{
    public class ArduinoController
    {
        private Socket socket;  // Socket für die Verbindung
        public bool IsConnected;
        public string LastError { get; private set; } 
        
        /// <summary>
        /// Baut eine TCP-Verbindung zum Arduino auf
        /// </summary>
        /// <returns>Gibt true zurück, wenn die Verbindung erfolgreich war, sonst false</returns>
        public bool Connect()
        {
            try
            {

                IPAddress ip = IPAddress.Parse("192.168.4.1"); //IP des Arduinos
                IPEndPoint endpoint = new IPEndPoint(ip, 5555); // Port für die Socketverbindung

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endpoint);

                LastError = null;
                IsConnected = true;
                return true;
            }
            catch (SocketException ex)
            {
                this.LastError = ex.Message;
                IsConnected = false;
                return false;
            }
        }
        /// <summary>
        /// Wandelt einen Power-Wert in einen Byte um und bringt ihn in den genutzten Bereiches des Arduinos
        /// </summary>
        /// <param name="sliderValue">Wert zwischen -130 und 130</param>
        /// <returns>Byte-Wert für den Arduino</returns>
        private byte ConvertPower(int sliderValue)
        {
            return (byte)(130 + Math.Abs(sliderValue));
        }
        /// <summary>
        /// Sendet einfachen Befehl ohne Geschwindigkeit
        /// </summary>
        public void SendCmd(byte cmd)
        {
            if (socket != null && socket.Connected)
            {
                try
                {
                    byte[] package = new byte[] { cmd };
                    socket.Send(package);
                }
                catch
                {
                    MessageBox.Show("Arduino ist nicht verbunden!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        /// <summary>
        /// Sendet einen Befehl mit gleicher Kettengeschwindigkeit
        /// </summary>
        public void SendCmd(byte cmd, int power)
        {
            if (socket != null && socket.Connected)
            {
                try
                {
                    byte[] package = new byte[] { cmd, ConvertPower(power) };
                    socket.Send(package);
                }
                catch
                {
                    MessageBox.Show("Arduino ist nicht verbunden!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        /// <summary>
        /// Sendet einen Befehl mit seperaten Kettengeschwindigkeiten
        /// </summary>
        public void SendCmd(byte cmd, int powerLeft, int powerRight)
        {
            if (socket != null && socket.Connected)
            {
                try
                {
                    byte[] package = new byte[] { cmd, ConvertPower(powerLeft), ConvertPower(powerRight) };
                    socket.Send(package);
                }
                catch
                {
                    MessageBox.Show("Arduino ist nicht verbunden!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            }
        }
    }
}
