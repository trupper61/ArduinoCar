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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArduinoCar2
{
    /// <summary>
    /// Interaction logic for StateControl.xaml
    /// </summary>
    public partial class StateControl : UserControl
    {
        public DriveState StateData { get; set; }
        private Point dragStart;
        private bool dragging = false;
        public event Action<StateControl> ConnectRequested;
        public StateControl()
        {
            InitializeComponent();

            MouseLeftButtonDown += MouseLeftButtonDown_Click;
            MouseMove += MouseMove_Control;
            MouseLeftButtonUp += MouseLeftButtonUp_Click;
            MouseRightButtonDown += MouseRightButtonDown_Click;
        }
        private void MouseRightButtonDown_Click(object sender, MouseButtonEventArgs e)
        {
            ConnectRequested?.Invoke(this);
        }
        private void MouseLeftButtonDown_Click(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                new StateEditor(StateData).ShowDialog();
                name.Text = StateData.Name;
                return;
            }
            dragging = true;
            dragStart = e.GetPosition(Parent as Canvas);
            CaptureMouse();
        }
        private void MouseMove_Control(object sender, MouseEventArgs e)
        {
            if (!dragging) return;

            var canvas = Parent as Canvas;
            var pos = e.GetPosition(canvas);
            Canvas.SetLeft(this, pos.X - Width / 2);
            Canvas.SetTop(this, pos.Y - Height / 2);
        }
        private void MouseLeftButtonUp_Click(object sender, MouseButtonEventArgs e)
        {
            dragging = false;
            ReleaseMouseCapture();
        }
    }
}
