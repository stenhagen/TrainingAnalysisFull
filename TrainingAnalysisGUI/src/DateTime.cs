using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TrainingAnalysis
{
    public class DateTime
    {
        public Dictionary<string, int> mDate { get; }
        public DateTime(string date)
        {
            string[] units = new string[] { "year", "month", "day", "hour", "minute", "second" };

            mDate = new Dictionary<string, int> { };

            Regex numberRx = new Regex(@"\d+");
            MatchCollection matches = numberRx.Matches(date);

            if (matches.Count < units.Length)
            {
                Exception rex = new Exception(String.Format("Regex should have matched at least {0} but matched {1} items", units.Length, matches.Count));
                throw rex;
            }

            for (int k = 0; k < units.Length; k++)
            {
                string matchString = matches[k].ToString();
                int unitNumber = -1;
                try
                {
                    unitNumber = int.Parse(matchString);
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                }
                mDate.Add(units[k], unitNumber);
            }
        }

        public int getDifference(DateTime comp)
        {
            // Calculates the difference in time between the reference and comp, supposing comp is after reference
            string[] units = new string[] { "year", "month", "day", "hour", "minute", "second" };

            int[] daysInMonth = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            Dictionary<string, int> factors = new Dictionary<string, int> { };
            factors.Add("year", 365);
            factors.Add("day", 24);
            factors.Add("hour", 60);
            factors.Add("minute", 60);
            factors.Add("second", 1);

            int diff = 0;
            int factor = 1;
            for (int k = units.Length - 1; k >= 0; k--)
            {
                if (units[k] == "month")
                {
                    int daysDiff = 0;
                    if (mDate["month"] <= comp.mDate["month"])
                    {
                        for (int m = mDate["month"]; m < comp.mDate["month"]; m++)
                        {
                            daysDiff = daysDiff + daysInMonth[m];
                        }
                    }
                    else
                    {
                        int wrapAround = 0;
                        for (int m = comp.mDate["month"]; m < mDate["month"]; m++)
                        {
                            wrapAround = wrapAround + daysInMonth[m];
                        }
                        daysDiff = -wrapAround;
                    }
                    diff = diff + factor * daysDiff;
                }
                else
                {
                    factor = factor * factors[units[k]];
                    diff = diff + factor * (comp.mDate[units[k]] - mDate[units[k]]);
                }
            }
            return diff;
        }

    }
}
