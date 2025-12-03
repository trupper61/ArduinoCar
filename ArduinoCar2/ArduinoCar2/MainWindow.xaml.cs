using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArduinoCar2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int leftSpeed = 0;
        int rightSpeed = 0;
        private ArduinoController controller = new ArduinoController();

        private bool isMovingForward = false;
        private bool isMovingBackward = false;
        private bool isTurningLeft = false;
        private bool isTurningRight = false;
        private bool isCircleLeft = false;
        private bool isCircleRight = false;
        private bool isStopped = true;
        public MainWindow()
        {
            InitializeComponent();
            UpdateUI();
            this.Focusable = true;
            this.Focus();
            //KeyDown += Window_KeyDown;
            //KeyUp += Window_KeyUp;
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (!isStopped)
            {
                ResetStates();
                isStopped = true;
                leftSpeed = 0;
                rightSpeed = 0;
                controller.SendCmd((byte)ArduinoCommands.Stop);

                UpdateUI();
            }
        }
        private void ResetStates()
        {
            isMovingForward = false;
            isMovingBackward = false;
            isTurningLeft = false;
            isTurningRight = false;
            isCircleLeft = false;
            isCircleRight = false;
            isStopped = false;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    if (!isMovingForward)
                    {
                        ResetStates();
                        isMovingForward = true;
                        leftSpeed = 100;
                        rightSpeed = 100;
                        controller.SendCmd((byte)ArduinoCommands.Forward, 10);
                    }
                    break;

                case Key.S:
                    if (!isMovingBackward)
                    {
                        ResetStates();
                        isMovingBackward = true;
                        leftSpeed = -100;
                        rightSpeed = -100;
                        controller.SendCmd((byte)ArduinoCommands.Backward, 10);
                    }
                    break;
                case Key.A:
                    if (!isTurningLeft)
                    {
                        ResetStates();
                        isTurningLeft = true;
                        leftSpeed = 40;
                        rightSpeed = 70;
                        controller.SendCmd((byte)ArduinoCommands.Forward, 5, 15);
                    }
                    break;
                case Key.D:
                    if (!isTurningRight)
                    {
                        ResetStates();
                        isTurningRight = true;
                        leftSpeed = 70;
                        rightSpeed = 40;
                        controller.SendCmd((byte)ArduinoCommands.Forward, 15, 5);
                    }
                    break;
                case Key.Q:
                    if (!isCircleLeft)
                    {
                        ResetStates();
                        isCircleLeft = true;
                        leftSpeed = -100;
                        rightSpeed = 100;
                        controller.SendCmd((byte)ArduinoCommands.CircleLeft, 10);
                    }
                    break;
                case Key.E:
                    if (!isCircleRight)
                    {
                        ResetStates();
                        isCircleRight = true;
                        leftSpeed = 100;
                        rightSpeed = -100;
                        controller.SendCmd((byte)ArduinoCommands.CircleRight, 10);
                    }
                    break;
            }
            UpdateUI();
        }


        private void TryConnect()
        {
            statusLabel.Text = "Verbindung wird aufgebaut...";
            statusLabel.Foreground = Brushes.Orange;

            if (controller.Connect())
            {
                statusLabel.Text = "Verbunden";
                statusLabel.Foreground = Brushes.Green;   
            }
            else
            {
                statusLabel.Text = $"Fehler: {controller.LastError}";
                statusLabel.Foreground = Brushes.Red;
                
            }
        }   
        private void Retry_Click(object sender, EventArgs e)
        {
            TryConnect();
        }
        private void UpdateUI()
        {
            leftBar.Value = (leftSpeed + 100) / 200.0 * 100;
            rightBar.Value = (rightSpeed + 100) / 200.0 * 100;
        }
        private void Forward_Click(object sender, EventArgs e)
        {
            leftSpeed = 100;
            rightSpeed = 100;
            controller.SendCmd((byte)ArduinoCommands.Forward, 10);
            UpdateUI();
        }
        private void Backward_Click(object sender, EventArgs e)
        {
            leftSpeed = -100;
            rightSpeed = -100;
            controller.SendCmd((byte)ArduinoCommands.Backward, 10);
            UpdateUI();
        }
        private void Left_Click(object sender, EventArgs e)
        {
            leftSpeed = 40;
            rightSpeed = 70;
            controller.SendCmd((byte)ArduinoCommands.Forward, 5, 15);
            UpdateUI();
        }
        private void Right_Click(object sender, EventArgs e)
        {
            leftSpeed = 70;
            rightSpeed = 40;
            controller.SendCmd((byte)ArduinoCommands.Forward, 15, 5);
            UpdateUI();
        }
        private void Stop_Click(object sender, EventArgs e)
        {
            leftSpeed = 0;
            rightSpeed = 0;
            controller.SendCmd((byte)ArduinoCommands.Stop);
            UpdateUI();
        }
        private void CircleLeft_Click(object sender, EventArgs e)
        {
            leftSpeed = -80;
            rightSpeed = 80;
            controller.SendCmd((byte)ArduinoCommands.CircleLeft, 10);
            UpdateUI();
        }
        private void CircleRight_Click(object sender, EventArgs e)
        {
            leftSpeed = 80;
            rightSpeed = -80;
            controller.SendCmd((byte)ArduinoCommands.CircleRight, 10);
        }



        public enum ArduinoCommands : byte
        {
            Stop = 0,
            Forward = 1,
            Backward = 2,
            CircleLeft = 3,
            CircleRight = 4,
        }
    }
}
