using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingAnalysis
{
    class CyclingSession: TrainingSession
    {
        public CyclingSession(string startString) : base(startString) { }
        
        protected override bool loadTrackPoint(string trackPoint, bool first)
        {

            Dictionary<object, bool> valuesUpdated = new Dictionary<object, bool> { };
            valuesUpdated.Add(Enums.SessionTag.Time, false);
            valuesUpdated.Add(Enums.SessionTag.DistanceMeters, false);
            valuesUpdated.Add(Enums.SessionTag.Position, false);
            valuesUpdated.Add(Enums.SessionTag.Value, false); // The innermost tag for heart rate. Value exist in no other context 
            valuesUpdated.Add(Enums.SessionTag.AltitudeMeters, false);


            string tag = XMLHelper.FindNextTagName(trackPoint);

            if (!Misc.AreStringEnumEqual(tag, typeof(Enums.SessionTag), Enums.SessionTag.Trackpoint))
            {
                Console.WriteLine("Warning in loadTrackPoint: Opening tag is incorect");
                return false;
            }
            if (!XMLHelper.IsTagGlobal(trackPoint, tag))
            {
                Console.WriteLine("Warning in loadTrackPoint: Trackpoint tag is not global");
                return false;
            }

            Stack<string> tpStack = new Stack<string> { };
            bool innerExists = true;

            double longVal = -181;
            double latVal = -91;

            while (innerExists || tpStack.Count > 0)
            {
                if (!innerExists)
                {
                    Console.WriteLine("Warning in loadTrackPoint: UnTagged value");
                    return false;
                }

                string parentTagName = XMLHelper.FindNextTagName(trackPoint);
                if (!XMLHelper.IsTagGlobal(trackPoint, parentTagName))
                {
                    tpStack.Push(XMLHelper.getRemainder(trackPoint, parentTagName));
                }
                string inner = XMLHelper.GetInnerString(trackPoint, parentTagName);
                innerExists = XMLHelper.ContainsTags(inner);

                // If content has a child that that also has tags we don't do any analysis just finds inner and pushes remainder
                if (innerExists)
                {
                    trackPoint = inner;
                }
                else
                {
                    if (!XMLHelper.ContainsTags(trackPoint))
                    {
                        Console.WriteLine("Warning in loadTrackPoint: UnTagged value");
                        return false;
                    }

                    // Converting to enum for switching
                    object tpEnum = Misc.GetEnumFromString(parentTagName, typeof(Enums.SessionTag));
                    switch (tpEnum)
                    {
                        case Enums.SessionTag.Time:
                            DateTime time = new DateTime(inner);
                            int diff = mStartTime.getDifference(time);
                            if (first)
                            {
                                mTimeVector.Add(diff);
                                valuesUpdated[Enums.SessionTag.Time] = true;
                            }
                            else
                            {
                                if (diff >= 0 && diff > mTimeVector[mTimeVector.Count - 1])
                                {
                                    mTimeVector.Add(diff);
                                    valuesUpdated[Enums.SessionTag.Time] = true;
                                }
                                else
                                {
                                    Console.WriteLine("Warning in loadTrackPoint: New time value: {0} is smaller than previous value", inner);
                                    return false;
                                }
                            }
                            break;

                        case Enums.SessionTag.LatitudeDegrees:
                            latVal = Misc.stringToDouble(inner);
                            break;

                        case Enums.SessionTag.LongitudeDegrees:
                            longVal = Misc.stringToDouble(inner);
                            break;

                        case Enums.SessionTag.AltitudeMeters:
                            mAltVector.Add(Misc.stringToDouble(inner));
                            valuesUpdated[Enums.SessionTag.AltitudeMeters] = true;
                            break;

                        case Enums.SessionTag.DistanceMeters:
                            mDistVector.Add(Misc.stringToDouble(inner));
                            valuesUpdated[Enums.SessionTag.DistanceMeters] = true;
                            break;

                        case Enums.SessionTag.Value:
                            mHRVector.Add(Misc.stringToInt(inner));
                            valuesUpdated[Enums.SessionTag.Value] = true;
                            break;

                        case Enums.SessionTag.SensorState:
                            break;

                        default:
                            Console.WriteLine(parentTagName);
                            Console.WriteLine("Warning in loadTrackPoint: Unrecognized tagname in track point: " + inner);
                            return false;
                    }

                    // Set trackPoint to the value popped from stack for next iteration
                    trackPoint = tpStack.Pop();
                    innerExists = true;
                }
            }

            if (latVal > -90 && longVal > -90)
            {
                mPosVector.Add(new Position(latVal, longVal));
                valuesUpdated[Enums.SessionTag.Position] = true;
            }

            bool AllUpdated = true;
            foreach (KeyValuePair<object, bool> entry in valuesUpdated)
            {
                if (!entry.Value)
                {
                    AllUpdated = false;
                    break;
                }
            }
            return AllUpdated;
        }
    }
}
