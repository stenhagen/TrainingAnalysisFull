using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingAnalysis
{
    public class CalibrationError : IComparable<CalibrationError>
    {
        /* CalibrationeError implements IComparable to allow for sorting of pointpairs based on error to
        * remove outliers in calibration.
        */

        public CalPoint3D[] pointPair { get; }
        public double error { get; }

        public CalibrationError(CalPoint3D[] pair, double e)
        {
            pointPair = pair;
            error = e;
        }

        public int CompareTo(CalibrationError p2)
        {
            if (error <= p2.error)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
}
