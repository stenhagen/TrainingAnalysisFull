using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingAnalysis
{
    namespace Calibration 
    {
        public class Calibration
        {
            public double AltThresh = 0.1;

            public bool UseAlt { get; private set; } = false;

            public Calibration(bool useAlt)
            {
                UseAlt = useAlt;
            }


            // Calibration contain methods for finding and validating factors transforming GPS coordinates to meters.
            // ToDo: make static
            private List<TickDiff> TrimPointPairList(List<TickDiff[]> tickDiffPairs)
            {
                List<TickDiff> tickDiffs = new List<TickDiff>();
                foreach (TickDiff[] pair in tickDiffPairs)
                {
                    tickDiffs.Add(pair[0]);
                }
                return tickDiffs;
            }

            public static bool ShouldAltBeUsed(TrainingSession ts)
            {
                Calibration calibrationWithAlt = new Calibration(true);
                List<TickDiffPair> calibrationSet = calibrationWithAlt.FindCandidates(ts, 100);
                List<TickDiffPair> validationSet = calibrationWithAlt.FindCandidates(ts, 100);

                ConversionFactors estimationWithAlt = calibrationWithAlt.CalibrateIteratively(calibrationSet, validationSet);
                double errorWithAlt = calibrationWithAlt. CalculateCalibrationError(estimationWithAlt, validationSet);

                Calibration calibrationWithoutAlt = new Calibration(false);
                ConversionFactors estimationWithoutAlt = calibrationWithoutAlt.CalibrateIteratively(calibrationSet, validationSet);
                double errorWithoutAlt = calibrationWithAlt.CalculateCalibrationError(estimationWithoutAlt, validationSet);

                return errorWithAlt < errorWithoutAlt;
            }

            public ConversionFactors Calibrate(TrainingSession ts, bool useAlt)
            {
                List<TickDiffPair> calibrationSet = FindCandidates(ts, 100, 1);
                List<TickDiffPair> validationSet = FindCandidates(ts, 100, 1);
                return CalibrateIteratively(calibrationSet, validationSet);
            }

            private List<TickDiffPair> FindCandidates(TrainingSession ts, int n, int ticksDiff = 1)
            {
                // Finds tickDiff pairs suitable for calibration 

                int ticks = ts.mTicks;
                Random rand = new Random();
                List<TickDiffPair> tickDiffPairs = new List<TickDiffPair> { };
                HashSet<int> usedPoints = new HashSet<int> { };
                while (tickDiffPairs.Count < n)
                {
                    TickDiff[] tickDiffPair = new TickDiff[2];
                    int[] tickNumbers = new int[2];
                    for (int k = 0; k < 2; k++)
                    {
                        bool altSuccess = false;

                        while (!altSuccess)
                        {
                            // Comparing the tick with the next tick
                            tickNumbers[k] = rand.Next(0, ticks - (ticksDiff + 1));
                            if (usedPoints.Contains(tickNumbers[k]))
                            {
                                continue;
                            }
                            altSuccess = Math.Abs(ts.mAltVector[tickNumbers[k]] - ts.mAltVector[tickNumbers[k] + ticksDiff]) > AltThresh;
                        }
                        tickDiffPair[k] = new TickDiff(ts, tickNumbers[k], ticksDiff);
                    }
                    if (CheckPair(tickDiffPair))
                    {
                        tickDiffPairs.Add(new TickDiffPair(tickDiffPair[0], tickDiffPair[1]));
                        // Adding the first tick in each tickDiff used tickDiffs  
                        usedPoints.Add(tickNumbers[0]);
                        usedPoints.Add(tickNumbers[1]);
                    }
                }
                return tickDiffPairs;
            }


            private ConversionFactors CalibrateIteratively(List<TickDiffPair> calibrationSet, List<TickDiffPair> validationSet)
            {
                void DiscardOutliers(int minNumber, double succesiveErrorThreshold)
                {
                    for (int k = minNumber; k < calibrationSet.Count; k++)
                    {
                        if ((calibrationSet[k].Error - calibrationSet[k - 1].Error)/ calibrationSet[k - 1].Error > succesiveErrorThreshold)
                        {
                            calibrationSet.RemoveRange(k, calibrationSet.Count - k);
                        }
                    } 
                }

                // Predictions are added to the set element
                foreach (TickDiffPair pair in calibrationSet)
                {
                    RunSingleCalibration(pair);
                    CalculateCalibrationError(pair, validationSet);
                }
                calibrationSet.Sort();
                DiscardOutliers((int) calibrationSet.Count/5, 0.1);

                double latPred = CustomMath.meanList(calibrationSet, "Lat");
                double longPred = CustomMath.meanList(calibrationSet, "Long");
                return new ConversionFactors(latPred, longPred);
            }

            private void RunSingleCalibration(TickDiffPair pair)
            {
                // Calculates mean and std for calibration factors for x and y
                double a0 = Math.Pow(pair.TickDiff1.mLatDiff, 2);
                double b0 = Math.Pow(pair.TickDiff1.mLongDiff, 2);
                double a1 = Math.Pow(pair.TickDiff2.mLatDiff, 2);
                double b1 = Math.Pow(pair.TickDiff2.mLongDiff, 2);
                double d0, d1;
                if (UseAlt)
                {
                    d0 = Math.Pow(pair.TickDiff1.mDist, 2) - Math.Pow(pair.TickDiff1.mAltDiff, 2);
                    d1 = Math.Pow(pair.TickDiff2.mDist, 2) - Math.Pow(pair.TickDiff2.mAltDiff, 2);
                }
                else
                {
                    d0 = Math.Pow(pair.TickDiff1.mDist, 2);
                    d1 = Math.Pow(pair.TickDiff2.mDist, 2);
                }
                    
                double y = Math.Sqrt((a0 * d1 - a1 * d0) / (a0 * b1 - a1 * b0));
                double x = Math.Sqrt((d0 / a0) * ((b0 * (a0 * d1 - a1 * d0)) / (a0 * ((a0 * b1 - a1 * b0)))));
                pair.Prediction = new ConversionFactors(x, y);
            }

            private void CalculateCalibrationError(TickDiffPair predictionPair, List<TickDiffPair> validationSet)
            {
                predictionPair.Error = CalculateCalibrationError(predictionPair.Prediction, validationSet);
            }

            private double CalculateCalibrationError(ConversionFactors prediction, List<TickDiffPair> validationSet)
            {
                /* ValidateCalibration 
                 * to calculate a distance between close ticks and comparing that to the ground truth distance change.
                 * 
                 */
                List<double> errors = new List<double>();
                foreach (TickDiffPair validationPair in validationSet)
                {
                    double pred;
                    if (UseAlt)
                    {
                        pred = Math.Sqrt(Math.Pow(prediction.Lat * validationPair.TickDiff1.mLatDiff, 2) +
                            Math.Pow(prediction.Long * validationPair.TickDiff1.mLongDiff, 2) +
                            Math.Pow(validationPair.TickDiff1.mAltDiff, 2));
                    }
                    else
                    {
                        pred = Math.Sqrt(Math.Pow(prediction.Lat * validationPair.TickDiff1.mLatDiff, 2) +
                            Math.Pow(prediction.Long * validationPair.TickDiff1.mLongDiff, 2));
                    }
                    double relError = Math.Abs((pred - validationPair.TickDiff1.mDist) / validationPair.TickDiff1.mDist);
                    errors.Add(relError);
                }
                return CustomMath.meanList(errors);
            }

            private double toDegrees(double rad)
            {
                return (rad * 180) / Math.PI;
            }

            private bool CheckPair(TickDiff[] pair)
            {
                // Checks whether candidate tick diff pair should be accepted. 
                // Not allowing pairs that are "too parallell" 

                TickDiff p0 = pair[0];
                TickDiff p1 = pair[1];
                const double thetaThresh = 5;
                const double lambda = 0.000001;

                // Using  dot product and a threshold on the angle theta
                // abs = ||a|| * ||b||
                // dot = a0*b0 + a1*b1
                //theta = arccos(dot/abs)
                double abs = Math.Sqrt(Math.Pow(p0.mLatDiff, 2) + Math.Pow(p0.mLongDiff, 2)) * Math.Sqrt(Math.Pow(p1.mLatDiff, 2) + Math.Pow(p1.mLongDiff, 2));
                double dot = p0.mLatDiff * p1.mLatDiff + p0.mLongDiff + p1.mLongDiff;

                // What does this do?
                if (dot < Math.Min(Math.Min(Math.Min(p0.mLatDiff, p1.mLatDiff), p0.mLongDiff), p1.mLongDiff) / 5 || dot < lambda)
                {
                    return true;
                }
                double theta = Math.Acos(dot / abs);
                double thetaDeg = toDegrees(theta);
                return thetaDeg > thetaThresh && thetaDeg < 180 - thetaThresh;
            }
        }
    }
}
