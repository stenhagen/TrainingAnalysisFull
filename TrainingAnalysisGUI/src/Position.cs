using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingAnalysis
{
    public class Position
    {
        public double mLatitude { get; }
        public double mLongitude { get; }
        public Position(double lat, double lon)
        {
            mLatitude = lat;
            mLongitude = lon;
        }
    }
}
