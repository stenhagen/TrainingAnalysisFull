using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingAnalysis
{
    public class Enums
    {
        public enum Activity
        {
            Running,
            Biking,
            OtherOutdoor,
            OtherIndoor
        }

        public enum SessionTag 
        {
            Track,
            Trackpoint,
            Time,
            LatitudeDegrees,
            LongitudeDegrees,
            AltitudeMeters,
            DistanceMeters,
            Value, // Heartbeat
            Cadence,
            SensorState,
            Position
        }

        public enum ErrorString
        {
            OK,
            NOK
        }
    }
}
