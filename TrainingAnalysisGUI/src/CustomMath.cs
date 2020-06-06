using System;
using System.Collections.Generic;

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

        public static double meanList<T>(List<T> values, string propertyName) where T: IEvaluatable
        {
            double acc = 0;
            foreach (T v in values)
            {
                acc = acc + v.EvaluateNumerically(propertyName);
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

        public static double stdList<T>(List<T> values, string propertyName) where T : IEvaluatable
        {
            double mean = meanList(values, propertyName);
            double acc = 0;
            foreach (T v in values)
            {
                acc = acc + Math.Pow(v.EvaluateNumerically(propertyName) - mean, 2);
            }
            return Math.Sqrt(acc);
        }
    }
}
