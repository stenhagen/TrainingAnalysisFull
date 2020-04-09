using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingAnalysis
{
    class CustomMath
    {
        public static double meanList(List<double> values)
        {
            double acc = 0;
            foreach (double v in values)
            {
                acc = acc + v;
            }
            return acc / values.Count;
        }

        public static double stdList(List<double> values)
        {
            double mean = meanList(values);
            double acc = 0;
            foreach (double v in values)
            {
                acc = acc + Math.Pow(v - mean, 2);
            }
            return Math.Sqrt(acc);
        }
    }
}
