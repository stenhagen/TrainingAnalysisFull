using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingAnalysis
{
    interface IEvaluatable
    {
        public double EvaluateNumerically(string propertyName);  
    }
}
