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
        private DispatcherTimer timer; // kontinuirliches Senden für die Tasten
        public List<DriveState> states = new List<DriveState>(); // gespeicherte Zustände für Planer
        public MainWindow()
        {
            InitializeComponent();
            UpdateUI();
            this.Focusable = true;
            this.Focus();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(70);
            timer.Tick += Timer_Tick;
        }
        /// <summary>
        /// Beim loslassen der Tasten wird der entsprechende bool auf false gesetzt. Stoppt ggf. Timer, wenn keine Tasten mehr gedrückt sind.
        /// </summary>
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
        /// <summary>
        /// Prüft, ob mindestens eine Steuerungstaste gedrückt ist und 
        /// startet oder stoppt den Timer entsprechend.
        /// </summary>
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
        /// <summary>
        /// Wird alle 70ms ausgeführt, solange der Timer aktiv ist
        /// Berechnet linke und rechte Kettengeschwindigkeit basierend auf Tastendruck
        /// Die linke Shifttaste führt zu einen Boost
        /// Sendet die Werte anschließend an den Arduino
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (controller.IsConnected)
            {
                bool shift = Keyboard.IsKeyDown(Key.LeftShift);
                if (forward)
                {
                    leftSpeed = shift ? 100 : 10;
                    rightSpeed = shift ? 100 : 10;
                    if (left)
                    {
                        rightSpeed = shift ? 110 : 70;
                        leftSpeed = shift ? 40 : 10;
                    }
                    if (right)
                    {
                        leftSpeed = shift ? 110 : 70;
                        rightSpeed = shift ? 40 : 10;
                    }
                }
                if (backward)
                {
                    leftSpeed = shift ? -100 : -10;
                    rightSpeed = shift ? -100 : -10;
                    if (left)
                    {
                        rightSpeed = shift ? -110 : -70;
                        leftSpeed = shift ? -40 : -10;
                    }
                    if (right)
                    {
                        leftSpeed = shift? -110 : -70;
                        rightSpeed = shift ? -40 : -10;
                    }
                }
                if (circleLeft)
                {
                    leftSpeed = shift ? -50 : -20;
                    rightSpeed = shift ? 50 : 20;
                }
                if (circleRight)
                {
                    leftSpeed = shift ? 50 : 20;
                    rightSpeed = shift ? -50 : -20;
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
                else if (leftSpeed <= 0 && rightSpeed <= 0) cmd = ArduinoCommands.Backward;
                else if (leftSpeed >= 0 && rightSpeed >= 0) cmd = ArduinoCommands.Forward;
                else if (leftSpeed < 0 && rightSpeed > 0) cmd = ArduinoCommands.CircleRight;
                else if (leftSpeed > 0 && rightSpeed < 0) cmd = ArduinoCommands.CircleLeft;
                else cmd = ArduinoCommands.Stop;

                controller.SendCmd((byte)cmd, leftSpeed, rightSpeed);
                UpdateUI();
                leftSpeed = 0;
                rightSpeed = 0;
            }
        }
        /// <summary>
        /// Begrenzung der Kettengeschwindigkeit auf maximal +/-110
        /// </summary>
        private int returnMinMax(int num)
        {
            if (num <= -110) return -110;
            else if (num >= 110) return 110;
            else return num;
        }
        /// <summary>
        /// Fügt einen Zustand hinzu
        /// Index wird automatisch inkrementiert und mit Standardwerten vergeben
        /// </summary>
        private void AddState_Click(object sender, EventArgs e)
        {
            DriveState state = new DriveState
            {
                Index = states.Count,
                Action = StateAction.Forward,
                Value = 1.0,
                Unit = StateUnit.Seconds
            };
            states.Add(state);
            RefreshGrid();
        }
        /// <summary>
        /// Löscht ausgewählten Zustand
        /// Updated die Indexe von neuen
        /// </summary>
        private void DeleteState_Click(object sender, EventArgs e)
        {
            if (stateGrid.SelectedItem is DriveState selected)
            {
                states.Remove(selected);
                for (int i = 0; i < states.Count; i++)
                {
                    states[i].Index = i + 1;
                }
            }
            RefreshGrid();
        }
        private async void StartPlan_Click(object sender, EventArgs e)
        {
            await ExecuteStates();
        }
        /// <summary>
        /// Führt Zustände nacheinander aus
        /// Prüft die Verbindung, sendet entsprechende Befehle
        /// Wiederholt für alle erstellten Befehle und Berücksichtigt die entsprechenden Werte
        /// </summary>
        private async Task ExecuteStates()
        {
            if (!controller.IsConnected)
            {
                MessageBox.Show("Arduino ist nicht verbunden!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            foreach (DriveState state in states.OrderBy(s =>  s.Index))
            {
                switch (state.Action)
                {
                    case StateAction.Forward:
                        controller.SendCmd((byte)ArduinoCommands.Forward, 10);
                        break;
                    case StateAction.Backward:
                        controller.SendCmd((byte)ArduinoCommands.Backward, 10);
                        break;
                    case StateAction.CircleLeft:
                        controller.SendCmd((byte)ArduinoCommands.CircleLeft, 20);
                        break;
                    case StateAction.CircleRight:
                        controller.SendCmd((byte)ArduinoCommands.CircleRight, 20);
                        break;
                    case StateAction.Stop:
                        controller.SendCmd((byte)ArduinoCommands.Stop);
                        break;
                }
                int delay = 0;
                if (state.Unit == StateUnit.Seconds)
                {
                    delay = (int)(state.Value * 1000);
                }
                else
                {
                    delay = (int)(state.Value * 500); //für dm
                }
                await Task.Delay(delay);
            }
            controller.SendCmd((byte)ArduinoCommands.Stop);  // Stoppt Auto nach Plan
        }
        /// <summary>
        /// Aktualisiert DataGrid Anzeige des Planers
        /// </summary>
        private void RefreshGrid()
        {
            stateGrid.ItemsSource = null;
            stateGrid.ItemsSource = states;
        }
        /// <summary>
        /// Überprüft Eingaben in der Wert-Spalte
        /// Es sind nur Werte > 0 erlaubt, bei ungültigen Eingabe wird eine Warnung angezeigt
        /// </summary>
        private void StateGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header.ToString() == "Wert")
            {
                if (e.EditingElement is TextBox tb)
                {
                    if (double.TryParse(tb.Text, out double value))
                    {
                        if (value <= 0)
                        {
                            MessageBox.Show("Der Wert muss größer als 0 sein!", "Ungültige Eingabe", MessageBoxButton.OK, MessageBoxImage.Warning);
                            tb.Text = "1";
                        }
                    }
                    else
                    {
                        MessageBox.Show("Bitte eine gültige Zahl eingeben!", "Ungültige Eingabe", MessageBoxButton.OK, MessageBoxImage.Warning);
                        tb.Text = "1";
                    }
                }
            }
        }
        /// <summary>
        /// Baut Verbindung zum Arduino auf
        /// Gibt Fehler im Status an
        /// </summary>
        private void TryConnect()
        {

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
            controller.SendCmd((byte)ArduinoCommands.Forward, 70, 50);
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
            controller.SendCmd((byte)ArduinoCommands.Forward, 70, 10);
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
        /// <summary>
        /// Inhalte des Hilfs-Panels
        /// </summary>
        private void Help_Click(object sender, EventArgs e)
        {
            if (drivePanel.Visibility == Visibility.Visible)
            {
                helpTextBox.Text = @"Fahrsteuerung
Tastatur:
W   -   Vorwärts
S   -   Rückwärts
A   -   Links
D   -   Rechts
Q   -   Kreis links
E   -   Kreis rechts

Buttons:
Steuern den Panzer an alternativ zur Tastatur";
            }
            else if (planPanel.Visibility == Visibility.Visible) 
            {
                helpTextBox.Text = @"Planer
Hier kannst du Zustände festlegen und hinzufügen:
Aktionen beschreibt was der Panzer bewältigen soll
und der Wert bestimmt die Angabe in Sekunden oder in
Strecken in dm (Der Wert muss größer als 0 sein).

Der Planer wird von oben nach unten ausgeführt";
            }
            helpPanel.Visibility = Visibility.Visible;
        }

        private void CloseHelp_Click(object sender, EventArgs e)
        {
            helpPanel.Visibility = Visibility.Collapsed;    
        }
        /// <summary>
        /// Arduino Befehle
        /// </summary>
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
