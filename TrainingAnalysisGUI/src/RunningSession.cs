using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingAnalysis
{
    public class RunningSession: TrainingSession
    {
        public List<int> mCadenceVector { get; }
        public static int CadenceError = -1;

        public RunningSession(string startString): base(startString)
        {
            mCadenceVector = new List<int> { };
        }

        protected override void FillInitCounter()
        {
            InitCounter.Add(Enums.SessionTag.Time, new InitValues(InitDefault));
            InitCounter.Add(Enums.SessionTag.DistanceMeters, new InitValues(InitDefault));
            InitCounter.Add(Enums.SessionTag.Position, new InitValues(InitDefault));
            InitCounter.Add(Enums.SessionTag.Value, new InitValues(InitDefault)); // The innermost tag for heart rate. Value exist in no other context 
            InitCounter.Add(Enums.SessionTag.AltitudeMeters, new InitValues(InitDefault));
            InitCounter.Add(Enums.SessionTag.Cadence, new InitValues(InitDefault));
        }

        protected override bool loadTrackPoint(string trackPoint, bool first)
        { 
            HashSet<object> dataElements = new HashSet<object>
            {
                Enums.SessionTag.Time,
                Enums.SessionTag.DistanceMeters,
                Enums.SessionTag.Position,
                Enums.SessionTag.Value,
                Enums.SessionTag.AltitudeMeters
            };

            foreach(object element in dataElements)
            {
                string elementString = element.ToString();
                 bool elementExists = trackPoint.Contains(elementString);

                string inner = "";
                if (elementExists)
                {
                    // If the element exists we try fetching inner.
                    // If this fails the file is corrupt and we return false.
                    inner = CheckTagAndGetInner(trackPoint, elementString);
                    //TODO throw exception
                    if (Misc.AreStringEnumEqual(inner, typeof(Enums.ErrorString), Enums.ErrorString.NOK))
                    {
                        return false;
                    }
                }

                bool success = false;
                switch (element)
                {
                    case Enums.SessionTag.Time:
                        if (!elementExists)
                        {
                            mTimeVector.Add(TimeError);
                            break;
                        }

                        DateTime time = new DateTime(inner);
                        int diff = mStartTime.getDifference(time);
                        if (first)
                        {
                            mTimeVector.Add(diff);
                            success = true;
                        }
                        else
                        {
                            if (diff >= 0 && diff > mTimeVector[mTimeVector.Count - 1])
                            {
                                mTimeVector.Add(diff);
                                success = true;
                            }
                            else
                            {
                                Console.WriteLine("Warning in loadTrackPoint: New time value: {0} is smaller than previous value", inner);
                                return false;
                            }
                        }
                        break;

                    case Enums.SessionTag.Position:
                        if (elementExists)
                        {
                            string latVal = CheckTagAndGetInner(trackPoint, Enums.SessionTag.LatitudeDegrees.ToString());
                            string longVal = CheckTagAndGetInner(trackPoint, Enums.SessionTag.LongitudeDegrees.ToString());
                            if (!(Misc.AreStringEnumEqual(latVal, typeof(Enums.ErrorString), Enums.ErrorString.NOK) ||
                                Misc.AreStringEnumEqual(longVal, typeof(Enums.ErrorString), Enums.ErrorString.NOK)))
                            {
                                double latDouble = Misc.stringToDouble(latVal, -181);
                                double longDouble = Misc.stringToDouble(longVal, -181);
                                if (!(latDouble< -180 || longDouble< -180))
                                {
                                    mPosVector.Add(new Position(latDouble, longDouble));
                                    success = true;
                                    break;
                                }
                            }
                        }
                        mPosVector.Add(PosError);
                        break;

                    case Enums.SessionTag.AltitudeMeters:
                        if (elementExists)
                        {
                            double val = Misc.stringToDouble(inner, 10001);
                            if (val< 10000)
                            {
                                mAltVector.Add(val);
                                success = true;
                                break;
                            }
                        }
                        mAltVector.Add(AltError);
                        break;

                    case Enums.SessionTag.DistanceMeters:
                        if (elementExists)
                        {
                            double val = Misc.stringToDouble(inner);
                            if (val >= 0)
                            {
                                mDistVector.Add(val);
                                success = true;
                                break;
                            }
                        }
                        mDistVector.Add(DistError);
                        break;

                    case Enums.SessionTag.Value:
                        if (elementExists)
                        {
                            int val = Misc.stringToInt(inner);
                            if (val >= 0)
                            {
                                mHRVector.Add(val);
                                success = true;
                                break;
                            }
                        }
                        mHRVector.Add(HRError);
                        break;

                    case Enums.SessionTag.Cadence:
                        if (elementExists)
                        {
                            int val = Misc.stringToInt(inner);
                            if (val >= 0)
                            {
                                mCadenceVector.Add(val);
                                success = true;
                                break;
                            }
                        }
                        mCadenceVector.Add(CadenceError);
                        break;

                    case Enums.SessionTag.SensorState:
                        break;

                    default:
                        break;
                }
                if (success && InitCounter[element].Final > InitThreshold) 
                {
                    InitCounter[element].Counter++;
                    if (InitCounter[element].Counter >= InitThreshold)
                    {
                        InitCounter[element].Final = mTicks - InitThreshold;
                    }
                }
            }
            return true;
        }
    }
}
