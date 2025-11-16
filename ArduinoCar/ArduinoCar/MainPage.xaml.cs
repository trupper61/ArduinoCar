namespace ArduinoCar
{
    public partial class MainPage : ContentPage
    {
        int leftSpeed = 0;
        int rightSpeed = 0;

        private ArduinoController controller = new ArduinoController();

        public MainPage()
        {
            InitializeComponent();
            UpdateUI();
            TryConnect();   
        }

        private void TryConnect()
        {
            statusLabel.Text = "Verbindung wird aufgebaut...";
            statusLabel.TextColor = Colors.Orange;

            if (controller.Connect())
            {
                statusLabel.Text = "Verbunden";
                statusLabel.TextColor = Colors.Green;
            }
            else
            {
                statusLabel.Text = $"Fehler: {controller.LastError}";
                statusLabel.TextColor = Colors.Red;
            }
        }

        private void Retry_Click(object sender, EventArgs e)
        {
            TryConnect();
        }
        private void UpdateUI()
        {
            leftBar.Progress = (leftSpeed + 100) / 200.0;
            rightBar.Progress = (rightSpeed + 100) / 200.0;
        }
        private void Forward_Click(object sender, EventArgs e)
        {
            leftSpeed = 100;
            rightSpeed = 100;
            controller.SendCmd(((byte)ArduinoCommands.Forward), 30);
            UpdateUI();
        }

        private void Backward_Click(object sender, EventArgs e)
        {
            leftSpeed = -100;
            rightSpeed = -100;
            controller.SendCmd((byte)ArduinoCommands.Backward, 30);
            UpdateUI();
        }

        private void Left_Click(object sender, EventArgs e)
        {
            leftSpeed = 30;
            rightSpeed = 50;
            controller.SendCmd((byte)ArduinoCommands.Forward, 30, 50);
            UpdateUI();
        }

        private void Right_Click(object sender, EventArgs e)
        {
            leftSpeed = 50;
            rightSpeed = 30;
            controller.SendCmd((byte)ArduinoCommands.Forward, 50, 30);
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
            controller.SendCmd((byte)ArduinoCommands.CircleLeft, 30);
            UpdateUI();
        }
        private void CircleRight_Click(object sender, EventArgs e)
        {
            leftSpeed = 80;
            rightSpeed = -80;
            controller.SendCmd((byte)ArduinoCommands.CircleRight, 30);
            UpdateUI();
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
