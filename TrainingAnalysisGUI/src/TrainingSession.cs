using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace TrainingAnalysis
{
    public abstract class TrainingSession
    {
        /* Base Class respresing a training session.   
        */

        public DateTime mStartTime { get; internal set; }
        public List<int> mTimeVector { get; }
        public List<double> mDistVector { get; }
        public List<Position> mPosVector { get; }
        public List<double> mAltVector { get; }
        public List<int> mHRVector { get; }
        public int mTicks { get; internal set;}

        public TrainingSession(string startString)
        {
            mStartTime = new DateTime(startString);
            mTimeVector = new List<int> { };
            mDistVector = new List<double> { };
            mPosVector = new List<Position> { };
            mAltVector = new List<double> { };
            mHRVector = new List<int> { };
            mTicks = 0;
        }

        public static Dictionary<string, string> getHeaderInfo(string header)
        {
            Dictionary<string, string> headerInfo = new Dictionary<string, string> { };

            string activityPrefix = "Activity Sport=";
            int activityStart = header.IndexOf(activityPrefix) + activityPrefix.Length;
            Regex activityRX = new Regex(@"\w+");
            Match activitymatch = activityRX.Match(header, activityStart);
            string activityString = activitymatch.ToString();
            headerInfo.Add("activity", activityString);

            string timePrefix = "Lap StartTime=";
            int timeStart = header.IndexOf(timePrefix) + timePrefix.Length;
            Regex timeRX = new Regex(@"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}");
            Match timematch = timeRX.Match(header, timeStart);
            string timeString = timematch.ToString();
            headerInfo.Add("startTime", timeString);

            return headerInfo;
        }

        public bool loadSessionBody(string content)
        {

            // Spilitting the string at each <Track> will yield N packages + the preamble.. 
            TagPair tagPair = XMLHelper.GetTagPair("Track");
            string tagPairStart = tagPair.start;
            string tagPairEnd = tagPair.end;
            string[] body = content.Split(tagPairStart);

            bool isFirst = true;
            // loop variable starts at 1 since body[0] is discardable preamble
            for (int p = 1; p < body.Length; p++)
            {
                // Removing stuff from </Track> onwards
                string package = body[p];
                int trackEnd = package.IndexOf(tagPairEnd);
                package = package.Substring(0, trackEnd);

                string tag = XMLHelper.FindNextTagName(package);
                bool isEmpty = false;
                while (!isEmpty && Misc.AreStringEnumEqual(tag, typeof(Enums.SessionTag), Enums.SessionTag.Trackpoint))
                {
                    string tp = XMLHelper.getNextTagContent(package);
                    if (loadTrackPoint(tp, isFirst))
                    {
                        mTicks++;
                    }
                    else
                    {
                        Console.WriteLine("Warning in loadTrackPoint: Tracking point not loaded correctly. Discarding session");
                        return false;
                    }
                    isFirst = false;
                    package = XMLHelper.getRemainder(package, "Trackpoint");
                    if (package.Length == 0)
                    {
                        isEmpty = true;
                        break;
                    }
                    tag = XMLHelper.FindNextTagName(package);
                }
            }
            return true;
        }

        protected abstract bool loadTrackPoint(string trackPoint, bool first);

    }
    public struct DateTime
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

    public struct Position
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
