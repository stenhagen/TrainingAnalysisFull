using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingAnalysis
{
    namespace Calibration
    {
        public class ConversionFactors
        {
            /* ConversionFactors contains factors for a single prediction converting a difference in GPS coordinates to a difference in meters
             * 
             */
            public double Lat { get; }
            public double Long { get; }

            public ConversionFactors(double latitude, double longitude)
            {
                Lat = latitude;
                Long = longitude;
            }
        }
    }
}
