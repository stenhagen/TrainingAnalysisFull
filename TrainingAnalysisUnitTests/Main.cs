using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrainingAnalysis;
using TrainingAnalysis.Calibration;
using TrainingAnalysis.DataStorage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DateTime = TrainingAnalysis.DateTime;

namespace UnitTests
{
 
    [TestClass]
    public class TrainingSessionTests
    {

        public void DoubleWithinMargin(double expected, double actual, double margin)
        {
            Assert.IsTrue(expected >= actual - margin);
            Assert.IsTrue(expected <= actual + margin);
        }


        // Structs methods
        [TestMethod]
        public void TestDateTimeCreation()
        {
            DateTime dt = new DateTime("2019-09-13T16:24:02.000Z");
            Assert.AreEqual(dt.mDate["year"], 2019);
            Assert.AreEqual(dt.mDate["month"], 9);
            Assert.AreEqual(dt.mDate["day"], 13);
            Assert.AreEqual(dt.mDate["hour"], 16);
            Assert.AreEqual(dt.mDate["minute"], 24);
            Assert.AreEqual(dt.mDate["second"], 2);
        }

        [TestMethod]
        public void TestDateTimeDiff()
        {
            DateTime dt1 = new DateTime("2019-09-13T16:24:02.000Z");
            DateTime dt2 = new DateTime("2019-09-13T16:24:03.000Z");
            DateTime dt3 = new DateTime("2019-09-13T17:25:03.000Z");
            DateTime dt4 = new DateTime("2019-11-13T17:24:02.000Z");
            DateTime dt5 = new DateTime("2019-12-31T23:59:59.000Z");
            DateTime dt6 = new DateTime("2020-01-01T00:00:00.000Z");

            Assert.AreEqual(1, dt1.getDifference(dt2));
            Assert.AreEqual(3661, dt1.getDifference(dt3));
            Assert.AreEqual(5274000, dt1.getDifference(dt4));
            Assert.AreEqual(1, dt5.getDifference(dt6));

            Assert.AreEqual(-1, dt6.getDifference(dt5));
            Assert.AreEqual(-5274000, dt4.getDifference(dt1));
        }

        // File Reader
        [TestMethod]
        public void TestFileReader()
        {
            // Reading a training session file and checking the returned string

            FileReader f = new FileReader();
            string session = f.readFile(@"C:\own\programming\projects\TrainingAnalysis\csharp\TrainingAnalysis\TrainingAnalysis\running_full.tcx");
            Assert.AreEqual(1016321, session.Length);
            Assert.AreEqual("<?xml vers", session.Substring(0, 10)); 
        }

        // Training Session
        [TestMethod]
        public void TestGetHeaderInfo()
        {
            // Reading a training session file and checking the header content

            FileReader f = new FileReader();
            string session = f.readFile("C:\\own\\programming\\projects\\TrainingAnalysis\\csharp\\TrainingAnalysis\\TrainingAnalysis\\running_full.tcx");
            Dictionary<string, string> headerInfo = TrainingSession.getHeaderInfo(session); 
            Assert.AreEqual("Running", headerInfo["activity"]);
            Assert.AreEqual("2019-09-13T16:21:19", headerInfo["startTime"]);
        }

        [TestMethod]
        public void TestLoadSessionBodyRunning()
        {

            // Reading a training session file and checking that the first and last produced datapoints are registered
            // correctly in a TrainingSession.

            FileReader f = new FileReader();
            string session = f.readFile("C:\\own\\programming\\projects\\TrainingAnalysis\\csharp\\TrainingAnalysis\\TrainingAnalysis\\running_full.tcx");
            string startTime = TrainingSession.getHeaderInfo(session)["startTime"]; 
            RunningSession ts = new RunningSession(startTime);
            bool success = ts.loadSessionBody(session);
            Assert.IsTrue(success);
            Assert.AreEqual(2756, ts.mTimeVector.Count);
            
            // First
            Assert.AreEqual(0, ts.mTimeVector[0]);
            DoubleWithinMargin(58.40579, ts.mPosVector[0].mLatitude, 0.000001);
            DoubleWithinMargin(15.61054333, ts.mPosVector[0].mLongitude, 0.000001);
            DoubleWithinMargin(95.251, ts.mAltVector[0], 0.000001);
            DoubleWithinMargin(3.5, ts.mDistVector[0], 0.000001);
            Assert.AreEqual(124, ts.mHRVector[0]);

            // Last
            Assert.AreEqual(2755, ts.mTimeVector[2755]);
            DoubleWithinMargin(58.40277333, ts.mPosVector[2755].mLatitude, 0.000001);
            DoubleWithinMargin(15.61368, ts.mPosVector[2755].mLongitude, 0.000001);
            DoubleWithinMargin(68.886, ts.mAltVector[2755], 0.000001);
            DoubleWithinMargin(11815.2001953125, ts.mDistVector[2755], 0.000001);
            Assert.AreEqual(166, ts.mHRVector[2755]);
        }

        [TestMethod]
        public void TestLoadSessionCycling()
        {
            FileReader f = new FileReader();
            string path = @"C:\own\programming\projects\TrainingAnalysisFull\TrainingAnalysisGUI\sessions\Petter_Stenhagen_2019-07-30_17-02-02.tcx";
            string session = f.readFile(path);
            string startTime = TrainingSession.getHeaderInfo(session)["startTime"];
            CyclingSession cs = new CyclingSession(startTime);
            bool success = cs.loadSessionBody(session);
            Assert.IsTrue(success);
            Assert.AreEqual(2516, cs.mTimeVector.Count);

            // First
            Assert.AreEqual(0, cs.mTimeVector[0]);
            Assert.IsNull(cs.mPosVector[0]);
            DoubleWithinMargin(TrainingSession.AltError, cs.mAltVector[0], 0.000001);
            DoubleWithinMargin(TrainingSession.DistError, cs.mDistVector[0], 0.000001);
            Assert.AreEqual(TrainingSession.HRError, cs.mHRVector[0]);

            // next to last 
            int i = 2514;
            Assert.AreEqual(i, cs.mTimeVector[i]);
            DoubleWithinMargin(58.52981167, cs.mPosVector[i].mLatitude, 0.000001);
            DoubleWithinMargin(15.45538167, cs.mPosVector[i].mLongitude, 0.000001);
            DoubleWithinMargin(70.562, cs.mAltVector[i], 0.000001);
            DoubleWithinMargin(18551, cs.mDistVector[i], 0.000001);
            Assert.AreEqual(TrainingSession.HRError, cs.mHRVector[i]);

        }

        // Misc
        [TestMethod]
        public void TestStringToInt()
        {
            string sInt = "45";
            string sFloat = "45.6";

            Assert.AreEqual(45, Misc.stringToInt(sInt));
            Assert.AreEqual(-1, Misc.stringToInt(sFloat));
        }

        [TestMethod]
        public void TestStringToFloat()
        {
            string sInt = "45";
            string sFloat = "45.6";
            string sString = "45.6ab";
            DoubleWithinMargin(45.6, Misc.stringToFloat(sFloat), 0.0001);
            DoubleWithinMargin(45, Misc.stringToFloat(sInt), 0.0001);
            DoubleWithinMargin(-1, Misc.stringToFloat(sString), 0.0001);
        }

        [TestMethod]
        public void TestAreStringEnumEqual() 
        {
            string runningString = "Running";
            object runningEnum = Enums.Activity.Running;
            string runningWrong = "Ranning";
            string biking = "Biking";
            Assert.IsTrue(Misc.AreStringEnumEqual(runningString, typeof(Enums.Activity), runningEnum));
            Assert.IsFalse(Misc.AreStringEnumEqual(runningWrong, typeof(Enums.Activity), runningEnum));
            Assert.IsFalse(Misc.AreStringEnumEqual(biking, typeof(Enums.Activity), runningEnum));
        }

        [TestMethod]
        public void TestGetEnumFromString()
        {
            string runningString = "Running";
            string runningWrong = "Ranning";
            Type t = typeof(Enums.Activity);
            Assert.AreEqual(Enums.Activity.Running, Misc.GetEnumFromString(runningString, t));
            Assert.IsNull(Misc.GetEnumFromString(runningWrong, t));
        }

        // XMLHelper
        [TestMethod]
        public void TestFindNextTagName()
        {
            string s = "<AltitudeMeters>68.886</AltitudeMeters><DistanceMeters>11815.2001953125</DistanceMeters>";
            Assert.IsTrue(Misc.AreStringEnumEqual(XMLHelper.FindNextTagName(s), typeof(Enums.SessionTag), Enums.SessionTag.AltitudeMeters));
        }

        [TestMethod]
        public void TestGetTagPair()
        {
            string withTags = "<AltitudeMeters>";
            string withoutTags = "AltitudeMeters";
            Assert.IsTrue(XMLHelper.ContainsTags(withTags));
            Assert.IsFalse(XMLHelper.ContainsTags(withoutTags));
        }

        [TestMethod]
        public void TestIsTagGlobal()
        {
            string notGloabl = "<AltitudeMeters>68.886</AltitudeMeters><DistanceMeters>11815.2001953125</DistanceMeters>";
            string gloabl = "<AltitudeMeters>68.886</AltitudeMeters>";
            Assert.IsTrue(XMLHelper.IsTagGlobal(gloabl, "AltitudeMeters"));
            Assert.IsFalse(XMLHelper.IsTagGlobal(notGloabl, "AltitudeMeters"));
        }

        [TestMethod]
        public void TestGetInnerString()
        {
            string s = "<AltitudeMeters>68.886</AltitudeMeters><DistanceMeters>11815.2001953125</DistanceMeters>";
            Assert.AreEqual("68.886", XMLHelper.GetInnerString(s, "AltitudeMeters"));
        }

        [TestMethod]
        public void TestGetRemainder()
        {
            string sLong = "<AltitudeMeters>68.886</AltitudeMeters><DistanceMeters>11815.2001953125</DistanceMeters>";
            string remLong = "<DistanceMeters>11815.2001953125</DistanceMeters>";
            string sShort = "<AltitudeMeters>68.886</AltitudeMeters>";
            string remShort = "";

            Assert.AreEqual(remLong, XMLHelper.getRemainder(sLong, "AltitudeMeters"));
            Assert.AreEqual(remShort, XMLHelper.getRemainder(sShort, "AltitudeMeters"));
        }

        [TestMethod]
        public void TestGetNextTagContent()
        { 
            string s = "<AltitudeMeters>68.886</AltitudeMeters><DistanceMeters>11815.2001953125</DistanceMeters>";
            string content = "<AltitudeMeters>68.886</AltitudeMeters>";
            Assert.AreEqual(content, XMLHelper.getNextTagContent(s));
        }

        [TestMethod]
        public void TestShouldAltBeUsed()
        {
            FileReader f = new FileReader();
            string path = @"C:\own\programming\projects\TrainingAnalysisFull\TrainingAnalysisGUI\sessions\Petter_Stenhagen_2019-07-30_17-02-02.tcx";
            string session = f.readFile(path);
            string startTime = TrainingSession.getHeaderInfo(session)["startTime"];
            CyclingSession cs = new CyclingSession(startTime);
            bool success = cs.loadSessionBody(session);
            Calibration.ShouldAltBeUsed(cs);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void testORM()
        {
            List < User > users = Queries.Select<User>($"SELECT * FROM dbo.users").ToList();
            Assert.AreEqual(users[0].Username, "Username");
        }
    }
}
