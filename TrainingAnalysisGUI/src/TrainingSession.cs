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

        protected Dictionary<object, InitValues> InitCounter { get; }

        public static DateTime StartTimeError = null;
        public static int TimeError = -1;
        public static double DistError = -1;
        public static Position PosError = null;
        public static double AltError = 10001;
        public static int HRError = -1;

        public static int InitThreshold = 10;
        public static int InitDefault = -1;

        public TrainingSession(string startString)
        {
            mStartTime = new DateTime(startString);
            mTimeVector = new List<int> { };
            mDistVector = new List<double> { };
            mPosVector = new List<Position> { };
            mAltVector = new List<double> { };
            mHRVector = new List<int> { };
            mTicks = 0;
            InitCounter = new Dictionary<object, InitValues>();
            FillInitCounter();
        }

        protected abstract void FillInitCounter();

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

        protected string CheckTagAndGetInner(string trackPoint, string tagName)
        {
            TagPair formattedTag = XMLHelper.GetTagPair(tagName);
            string pattern = formattedTag.start + "(.*)" + formattedTag.end;
            Regex tag = new Regex(pattern);
            Match match = tag.Match(trackPoint);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return "NOK";
            }
        }

        protected abstract bool loadTrackPoint(string trackPoint, bool first);
    }

    public class InitValues
    {
        public int Counter { get; set; }
        public int Final { get; set; }

        public InitValues(int init)
        {
            Counter = init;
            Final = init;
        }
    }
}
