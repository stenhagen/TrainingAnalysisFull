using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingAnalysis
{
    namespace Calibration
    {
        public class TickDiff
        {
            public double mLatDiff { get; }
            public double mLongDiff { get; }
            public double mAltDiff { get; }
            public double mDist { get; }

            public TickDiff(double lat0, double lat1, double long0, double long1, double alt0, double alt1, double dist0, double dist1)
            {
                mLatDiff = lat1 - lat0;
                mLongDiff = long1 - long0;
                mAltDiff = alt0 - alt1;
                mDist = dist1 - dist0;
                if (mDist <= 0.0001) 
                {
                    NoDistanceDiffException ex = new NoDistanceDiffException();
                    throw ex;
                }
            }

            public TickDiff(TrainingSession ts, int index, int ticksDiff)
            {
                mLatDiff = ts.mPosVector[index + ticksDiff].mLatitude - ts.mPosVector[index].mLatitude;
                mLongDiff = ts.mPosVector[index + ticksDiff].mLongitude - ts.mPosVector[index].mLongitude;
                mAltDiff = ts.mAltVector[index + ticksDiff] - ts.mAltVector[index];
                mDist = ts.mDistVector[index + ticksDiff] - ts.mDistVector[index];
                if (mDist <= 0.0001)
                {
                    NoDistanceDiffException ex = new NoDistanceDiffException();
                    throw ex;
                }
            }
        }

        public class NoDistanceDiffException: Exception
        {
            public NoDistanceDiffException(): base("Distance between neighbouring points is too close to 0") { }
            public NoDistanceDiffException(string message): base(message) { }
        }
    }
}
