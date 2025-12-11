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
using System.Windows.Shapes;
using static ArduinoCar2.DriveState;

namespace ArduinoCar2
{
    /// <summary>
    /// Interaction logic for StateEditor.xaml
    /// </summary>
    public partial class StateEditor : Window
    {
        private DriveState state;
        public StateEditor(DriveState state)
        {
            InitializeComponent();
            this.state = state;
            actionBox.SelectedIndex = (int)state.Action;
            unitBox.SelectedIndex = (int)state.Unit;
            valueBox.Text = state.Value.ToString();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            state.Action = (StateAction)(actionBox.SelectedIndex);
            state.Unit = (StateUnit)(unitBox.SelectedIndex);
            state.Value = double.Parse(valueBox.Text);

            Close();
        }
    }
}
