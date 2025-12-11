using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoCar2
{
    public class DriveState
    {
        public enum StateUnit
        {
            Seconds,
            Decimeters
        }
        public enum StateAction
        {
            Forward,
            Backward,
            CircleLeft,
            CircleRight,
        }

        public string Name { get; set; }
        public StateAction Action { get; set; }
        public double Value { get; set; }
        public StateUnit Unit { get; set; }
        public DriveState(string name, StateAction action, double value, StateUnit unit)
        {
            Name = name;
            Action = action;
            Value = value;
            Unit = unit;
        }
    }
}
