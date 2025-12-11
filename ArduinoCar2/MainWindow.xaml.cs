using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;
using static ArduinoCar2.DriveState;

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

        private bool forward, backward, left, right, circleLeft, circleRight = false;
        private DispatcherTimer timer;
        public List<(StateControl A, StateControl B)> Connections = new List<(StateControl A, StateControl B)>();
        public StateControl pending = null;
        public MainWindow()
        {
            InitializeComponent();
            UpdateUI();
            this.Focusable = true;
            this.Focus();
            //KeyDown += Window_KeyDown;
            //KeyUp += Window_KeyUp;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(70);
            timer.Tick += Timer_Tick;
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W) forward = false;
            if (e.Key == Key.S) backward = false;
            if (e.Key == Key.A) left = false;
            if (e.Key == Key.D) right = false;
            if (e.Key == Key.Q) circleLeft = false;
            if (e.Key == Key.E) circleRight = false;

            CheckTimer();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (controller.IsConnected)
            {
                if (forward)
                {
                    leftSpeed = 10;
                    rightSpeed = 10;  
                }
                if (backward)
                {
                    leftSpeed = -10;
                    rightSpeed = -10;
                }
                if (left && forward)
                {
                    leftSpeed = 10;
                    rightSpeed += 50;
                }
                if (right && forward)
                {
                    leftSpeed += 50;
                    rightSpeed = 10;
                }
                if (circleLeft)
                {
                    leftSpeed = -20;
                    rightSpeed = 20;
                }
                if (circleRight)
                {
                    leftSpeed = 20;
                    rightSpeed = -20;
                }
                if (!forward && !backward && !left && !right && !circleLeft && !circleRight)
                {
                    leftSpeed = 0;
                    rightSpeed = 0;
                }

                leftSpeed = returnMinMax(leftSpeed);
                rightSpeed = returnMinMax(rightSpeed);

                ArduinoCommands cmd;
                if (leftSpeed == 0 && rightSpeed == 0) cmd = ArduinoCommands.Stop;
                else if (leftSpeed <= 0 && rightSpeed <= 0) cmd = ArduinoCommands.Forward;
                else if (leftSpeed >= 0 && rightSpeed >= 0) cmd = ArduinoCommands.Backward;
                else if (leftSpeed < 0 && rightSpeed > 0) cmd = ArduinoCommands.CircleRight;
                else if (leftSpeed > 0 && rightSpeed < 0) cmd = ArduinoCommands.CircleLeft;
                else cmd = ArduinoCommands.Stop;

                controller.SendCmd((byte)cmd, leftSpeed, rightSpeed);
                UpdateUI();
                leftSpeed = 0;
                rightSpeed = 0;
            }
        }
        private int returnMinMax(int num)
        {
            if (num <= -60) return -60;
            else if (num >= 60) return 60;
            else return num;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W) forward = true;
            if (e.Key == Key.S) backward = true;
            if (e.Key == Key.A) left = true;
            if (e.Key == Key.D) right = true;
            if (e.Key == Key.Q) circleLeft = true;
            if (e.Key == Key.E) circleRight = true;

            CheckTimer();
        }

        private void CheckTimer()
        {
            bool keyPressed = forward || backward || right || left || circleLeft || circleRight;
            if (keyPressed)
            {
                if (!timer.IsEnabled)
                    timer.Start();
            }
            else
            {
                if (timer.IsEnabled)
                    timer.Stop();
                controller.SendCmd((byte)ArduinoCommands.Stop);
                leftSpeed = 0;
                rightSpeed = 0;
                UpdateUI();
            }
        }

        private void State_ConnectRequested(StateControl s)
        {
            if (pending == null)
            {
                pending = s;
                return;
            }
            Connections.Add((pending, s));
            DrawConnections();
            pending = null;
        }
        private void DrawConnections()
        {
            stateCanvas.Children.OfType<Line>().ToList().ForEach(l => stateCanvas.Children.Remove(l));
            foreach(var (a, b) in Connections)
            {
                var line = new Line
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    X1 = Canvas.GetLeft(a) + a.Width / 2,
                    Y1 = Canvas.GetTop(a) + a.Height / 2,
                    X2 = Canvas.GetLeft(b) + b.Width / 2,
                    Y2 = Canvas.GetTop(b) + b.Height / 2
                };
                stateCanvas.Children.Add(line);
            }
        }

        private async void StartPlan_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled)
                timer.Stop();
            CheckTimer();
            await ExecuteStates();
        }
        public async Task ExecuteStates()
        {
            foreach (var (control, next) in Connections)
            {
                var state = control.StateData;
                switch (state.Action)
                {
                    case StateAction.Forward:
                        controller.SendCmd((byte)ArduinoCommands.Forward, 10);
                        break;
                    case StateAction.Backward:
                        controller.SendCmd((byte)ArduinoCommands.Backward, 10);
                        break;
                    case StateAction.CircleLeft:
                        controller.SendCmd((byte)ArduinoCommands.CircleLeft, 10);
                        break;
                    case StateAction.CircleRight:
                        controller.SendCmd((byte)ArduinoCommands.CircleRight, 10);
                        break;
                }

                if (state.Unit == StateUnit.Seconds)
                    await Task.Delay((int)(state.Value * 1000));
                else
                    await Task.Delay((int)(state.Value * 100)); // Zeit hinterlegt 1 dm in ms
            }
            controller.SendCmd((byte)ArduinoCommands.Stop);
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

        private void ShowPanel_Click(object sender, RoutedEventArgs e)
        {
            drivePanel.Visibility = Visibility.Collapsed;
            planPanel.Visibility = Visibility.Visible;
        }
        
        private void BackToDrive_Click(object sender, RoutedEventArgs e)
        {
            drivePanel.Visibility = Visibility.Visible;
            planPanel.Visibility = Visibility.Collapsed;
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

        public void AddState_Click(object sender, RoutedEventArgs e)
        {
            var state = new DriveState("Geradeaus",StateAction.Forward, 1, StateUnit.Seconds);
            var controll = new StateControl();
            controll.ConnectRequested += State_ConnectRequested;
            controll.StateData = state;
            Canvas.SetLeft(controll, 100);
            Canvas.SetTop(controll, 100);
            stateCanvas.Children.Add(controll);
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
