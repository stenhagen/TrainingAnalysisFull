using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingAnalysis
{
    interface IEvaluatable
    {
        public double EvaluateNumerically(string propertyName);  
    }

    public class PropertyNotSupportedException : Exception
    {
        public PropertyNotSupportedException() : base() { }
        public PropertyNotSupportedException(string message) : base(message) { }
    }
}
