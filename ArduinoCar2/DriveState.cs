using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArduinoCar2.DriveState;

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
            Stop
        }

        public StateAction Action { get; set; }
        public double Value { get; set; }
        public StateUnit Unit { get; set; }
        public int Index {  get; set; }
    }
    public static class EnumExtensions
    {
        public static Array GetStateActions => Enum.GetValues(typeof(StateAction));
        public static Array GetStateUnits => Enum.GetValues(typeof(StateUnit));
    }
}
