using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace TrainingAnalysis.Calibration
{
    public class TickDiffPair : IComparable<TickDiffPair>, IEvaluatable
    {
        public TickDiff TickDiff1 { get; }
        public TickDiff TickDiff2 { get; }
        public ConversionFactors Prediction { get; set; }
        public double Error { get; set; }

        public TickDiffPair(TickDiff td1, TickDiff td2)
        {
            TickDiff1 = td1;
            TickDiff2 = td2;
        }

        public int CompareTo(TickDiffPair p2)
        {
            if (Error < 0 || p2.Error < 0)
            {
                Exception e = new Exception("Attempting to compare uninitialized errors of points");
                throw e;
            }
            return Error <= p2.Error ? -1 : 1;
        }
        public double EvaluateNumerically(string propertyName)
        {
            PropertyInfo info = typeof(TickDiffPair).GetProperty(propertyName);
            object value = info.GetValue(this);
            if ( value != null)
            {
                return (double)value;
            }
            else
            {
                info = Prediction.GetType().GetProperty(propertyName);
                value = info.GetValue(Prediction);
                if (value != null)
                {
                    return (double)value;
                }
                else
                {
                    Exception e = new Exception($"{propertyName} is not a valid property for type {nameof(TickDiffPair)}");
                    throw e;
                }
            }
        }
    }
}
