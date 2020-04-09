using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingAnalysis
{
    public static class Misc
    {
        public static int stringToInt(string number)
        {
            int n = -1;
            try
            {
                n = int.Parse(number);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }
            return n;
        }

        public static float stringToFloat(string number)
        {
            number = commaDotConversion(number);
            float n = -1;
            try
            {
                n = float.Parse(number);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }
            return n;
        }

        public static double stringToDouble(string number)
        {
            number = commaDotConversion(number);
            double n = -1;
            try
            {
                n = double.Parse(number);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }
            return n;
        }


        public static string commaDotConversion(string number)
        {
            if (number.Contains("."))
            {
                return number.Replace(".", ",");
            }
            return number;
        }

        public static bool AreStringEnumEqual(string s, Type enumType, object e) 
        {
            object sEnum;
            bool success = Enum.TryParse(enumType, s, false, out sEnum);
            return (success && Enum.Equals(sEnum, e));
        }

        public static object GetEnumFromString(string s, Type enumType)
        {
            object sEnum;
            bool success = Enum.TryParse(enumType, s, false, out sEnum);
            if (success)
            {
                return sEnum;
            }
            else
            {
                return null;
            }
        }

    }
}
