using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingAnalysis
{
    public class Calibration
    {

        // Calibration contain methods for finding and validating factors transforming GPS coordinates to meters.
        // ToDo: make static
        
        private List<CalPoint3D> TrimPointPairList(List<CalPoint3D[]> pointPairs) 
        {
            List<CalPoint3D> points = new List<CalPoint3D>();
            foreach (CalPoint3D[] pair in pointPairs)
            {
                points.Add(pair[0]);
            }
            return points;
        }

        public void CalibrateIteratively(TrainingSession ts, double relThresh, double reqAcc)
        {
            List<CalPoint3D[]> calibrationSet = FindCandidates(ts, 100, 1, 0.4);
            List<CalPoint3D[]> validationSet = FindCandidates(ts, 100, 1, 0.4);

            // Start by determining whether it is best to use altitude or not
            Dictionary<string, double> calWithAlt = Calibrate(calibrationSet, true);
            double xWithAlt = calWithAlt["meanX"];
            double yWithAlt = calWithAlt["meanY"];
            ValidationResults validationWithAlt = ValidateCalibration(xWithAlt, yWithAlt, validationSet, 0.02, true);

            Dictionary<string, double> calWithoutAlt = Calibrate(calibrationSet, false);
            double xWithoutAlt = calWithoutAlt["meanX"];
            double yWithoutAlt = calWithoutAlt["meanY"];
            ValidationResults validationWithoutAlt = ValidateCalibration(xWithoutAlt, yWithoutAlt, validationSet, 0.02, false);

            double errorWithAlt = validationWithAlt.meanRelativeError;
            double errorInsideThreshWithAlt = validationWithAlt.errorInsideThresh;
            double errorWithoutAlt = validationWithoutAlt.meanRelativeError;
            double errorInsideThreshWithoutAlt = validationWithoutAlt.errorInsideThresh;

            bool useAlt;
            bool calOK;
            Coordinate calibrationValues;
            if (errorWithAlt < errorWithoutAlt && errorInsideThreshWithAlt > errorInsideThreshWithoutAlt)
            {
                useAlt = true;
                calOK = errorWithAlt < relThresh && errorInsideThreshWithAlt > reqAcc;
                calibrationValues = new Coordinate(xWithAlt, yWithAlt);
            }
            else if (errorWithAlt > errorWithoutAlt && errorInsideThreshWithAlt < errorInsideThreshWithoutAlt)
            {
                useAlt = false;
                calOK = errorWithoutAlt < relThresh && errorInsideThreshWithoutAlt > reqAcc;
                calibrationValues = new Coordinate(xWithoutAlt, yWithoutAlt);
            }
            else
            {
                Console.WriteLine("Use altitude validation is ambivalent. Not using alt");
                useAlt = false;
                calOK = errorWithoutAlt < relThresh && errorInsideThreshWithoutAlt > reqAcc;
                calibrationValues = new Coordinate(xWithoutAlt, yWithoutAlt);
            }

            while (!calOK)
            {
                ValidationResults validationCalibrationSet = ValidateCalibration(calibrationValues.x, calibrationValues.y, calibrationSet, relThresh, useAlt);
                List<CalibrationError> calibrationSetSorted = validationCalibrationSet.calPairs;
                calibrationSetSorted.Sort();
                
            }
        }

        public ValidationResults ValidateCalibration(double x, double y, List<CalPoint3D[]> pointPairs, double relThresh, bool useAlt)
        {
            /* ValidateCalibration checks whether a calibration is accurate enough by using the calibration values 
             * to calculate a distance between close points and comparing that to the ground truth distance change.
             * 
             * relThresh is the threshold for the relative difference in distance between prediction and ground truth
             */

            List<CalibrationError> calError = new List<CalibrationError>();
            foreach (CalPoint3D[] pair in pointPairs)
            {
                double pred;
                if (useAlt)
                {
                    pred = Math.Sqrt(Math.Pow(x * pair[0].mLatDiff, 2) + Math.Pow(y * pair[0].mLongDiff, 2) + Math.Pow(pair[0].mAltDiff, 2));
                }
                else 
                {
                    pred = Math.Sqrt(Math.Pow(x * pair[0].mLatDiff, 2) + Math.Pow(y * pair[0].mLongDiff, 2));
                }
                double relError = Math.Abs((pred - pair[0].mDist) / pair[0].mDist);
                calError.Add(new CalibrationError(pair, relError));
            }
            return new ValidationResults(calError, relThresh);
        }

        public Dictionary<string, double> Calibrate(List<CalPoint3D[]> pointPairs, bool useAlt) 
        {
            // Calculates mean and std for calibration factors for x and y  
             
            Dictionary<string, double> stats = new Dictionary<string, double>();
            List<double> predsX = new List<double> { };
            List<double> predsY = new List<double> { };
            foreach (CalPoint3D[] pair in pointPairs)
            {
                double a0 = Math.Pow(pair[0].mLatDiff, 2);
                double b0 = Math.Pow(pair[0].mLongDiff, 2);
                double a1 = Math.Pow(pair[1].mLatDiff, 2);
                double b1 = Math.Pow(pair[1].mLongDiff, 2);
                double d0, d1;
                if (useAlt) 
                {
                    d0 = Math.Pow(pair[0].mDist, 2) - Math.Pow(pair[0].mAltDiff, 2);
                    d1 = Math.Pow(pair[1].mDist, 2) - Math.Pow(pair[1].mAltDiff, 2);
                }
                else
                {
                    d0 = Math.Pow(pair[0].mDist, 2);
                    d1 = Math.Pow(pair[1].mDist, 2);
                }

                double y = Math.Sqrt((a0 * d1 - a1 * d0) / (a0 * b1 - a1 * b0));
                double x = Math.Sqrt((d0 / a0) * ((b0 * (a0 * d1 - a1 * d0))/(a0 * ((a0 * b1 - a1 * b0)))));
                predsX.Add(x);
                predsY.Add(y);
            }
            stats.Add("meanX", CustomMath.meanList(predsX));
            stats.Add("meanY", CustomMath.meanList(predsY));
            stats.Add("stdX", CustomMath.stdList(predsX));
            stats.Add("stdY", CustomMath.stdList(predsY));
            return stats;
        }

        public List<CalPoint3D[]> FindCandidates(TrainingSession ts, int n, int ticksDiff, double altThresh)
        {
            // Finds point pairs suitable for calibration 

            int ticks = ts.mTicks;
            Random rand = new Random();
            const int p = 2;
            List<CalPoint3D[]> pointPairs = new List<CalPoint3D[]> { };
            while (pointPairs.Count < n)
            {
                CalPoint3D[] pointPair = new CalPoint3D[p];
                for (int k = 0; k < p; k++)
                {
                    bool altSuccess = false;
                    int pointNumber = 0;
                    while (!altSuccess)
                    {
                        // Comparing the point with the next ticked point
                        pointNumber = rand.Next(0, ticks - (ticksDiff + 1));
                        altSuccess = Math.Abs(ts.mAltVector[pointNumber] - ts.mAltVector[pointNumber + ticksDiff]) > altThresh;
                    }
                    pointPair[k] = new CalPoint3D(ts, pointNumber, ticksDiff);
                }
                if (CheckPair(pointPair))
                {
                    pointPairs.Add(pointPair);
                }
            }
            return pointPairs;
        }

        private double toDegrees(double rad)
        {
            return (rad * 180) / Math.PI;
        }

        private bool CheckPair(CalPoint3D[] pair)
        {
            // Checks whether candidate point pairs should be accepted. 
            // Not allowing pairs that are "too parallell" 

            CalPoint3D p0 = pair[0];
            CalPoint3D p1 = pair[1];
            const double thetaThresh = 5;
            const double lambda = 0.000001;

            // Using  dot product and a threshold on the angle theta
            // abs = ||a|| * ||b||
            // dot = a0*b0 + a1*b1
            //theta = arccos(dot/abs)
            double abs = Math.Sqrt(Math.Pow(p0.mLatDiff, 2) + Math.Pow(p0.mLongDiff, 2)) * Math.Sqrt(Math.Pow(p1.mLatDiff, 2) + Math.Pow(p1.mLongDiff, 2));
            double dot = p0.mLatDiff * p1.mLatDiff + p0.mLongDiff + p1.mLongDiff;
            if (dot < Math.Min(Math.Min(Math.Min(p0.mLatDiff, p1.mLatDiff), p0.mLongDiff), p1.mLongDiff)/5 || dot < lambda)
            {
                return true;
            }
            double theta = Math.Acos(dot / abs);
            double thetaDeg = toDegrees(theta);
            return thetaDeg > thetaThresh && thetaDeg < 180 - thetaThresh;
        }
    }
    public struct Coordinate
    {
        public double x { get; }
        public double y { get; }

        public Coordinate(double xIn, double yIn)
        {
            x = xIn;
            y = yIn;
        }
    }

    public struct ValidationResults
    {
        public List<CalibrationError> calPairs { get; }
        public double meanRelativeError { get; }
        public double errorInsideThresh { get; }

        public ValidationResults(List<CalibrationError> calErrors, double relThresh)
        {
            int accCorrect = 0;
            double accRelError = 0;
            foreach (CalibrationError calError in calErrors)
            {
                accRelError = accRelError + calError.error;
                if (calError.error < relThresh)
                {
                    accCorrect++;
                }
            }
            int count = calErrors.Count;

            meanRelativeError = accRelError / count;
            errorInsideThresh = (double)accCorrect / count;
            calPairs = calErrors;
        }
    }

}
