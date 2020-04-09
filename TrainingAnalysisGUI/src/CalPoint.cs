using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingAnalysis
{
    public class CalPoint2D
    {
        public double mLatDiff { get; }
        public double mLongDiff { get; }
        public double mDist { get; }

        public CalPoint2D(double lat0, double lat1, double long0, double long1, double dist0, double dist1)
        {
            mLatDiff = lat1 - lat0;
            mLongDiff = long1 - long0;
            mDist = dist1 - dist0;

        }

        public CalPoint2D(TrainingSession ts, int index, int ticksDiff)
        {
            mLatDiff = ts.mPosVector[index + ticksDiff].mLatitude - ts.mPosVector[index].mLatitude;
            mLongDiff = ts.mPosVector[index + ticksDiff].mLongitude - ts.mPosVector[index].mLongitude;
            mDist = ts.mDistVector[index + ticksDiff] - ts.mDistVector[index];
        }

        public CalPoint2D(double latDiff, double longDiff, double dist)
        {
            mLatDiff = latDiff;
            mLongDiff = longDiff;
            mDist = dist;
        }

    }

    public class CalPoint3D : CalPoint2D
    {
        public double mAltDiff { get; }
        public CalPoint3D(double lat0, double lat1, double long0, double long1, double alt0, double alt1, double dist0, double dist1)
            : base(lat0, lat1, long0, long1, dist0, dist1)
        {
            mAltDiff = alt0 - alt1;    
        }

        public CalPoint3D(TrainingSession ts, int index, int ticksDiff): base(ts, index, ticksDiff)
        {
            mAltDiff = ts.mAltVector[index + ticksDiff] - ts.mAltVector[index];
        }

        public CalPoint2D toCalPoint2D()
        {
            return new CalPoint2D(mLatDiff, mLongDiff, mDist);
        }

    }

}
