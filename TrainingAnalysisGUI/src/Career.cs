using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TrainingAnalysis
{
    public class Career
    {

        /* The class typically defining a person containing a base of all the persons training sessions
         */

        private SessionBase mRunningBase;
        private SessionBase mCyclingBase;

        public Career()
        {
            mRunningBase = new SessionBase(Enums.Activity.Running);
            mCyclingBase = new SessionBase(Enums.Activity.Biking);
        }

        public void addNewSessions()
        {
            string root = @"C:\own\programming\projects\TrainingAnalysis\csharp\TrainingAnalysis";
            string miscRel = @"TrainingAnalysis\MiscFiles";
            string miscAbs = Path.Combine(new string[] { root, miscRel });
            string inBaseAbs = Path.Combine(new string[] { miscAbs, "in_base.txt" });

            if (!File.Exists(inBaseAbs))
            {
                File.Create(inBaseAbs);
            }

            string[] sessionLines = File.ReadAllLines(inBaseAbs);
            HashSet<string> analyzedSessions = new HashSet<string> { };
            foreach (string session in sessionLines)
            {
                analyzedSessions.Add(session);
            }
            string sessionsRel = @"TrainingAnalysis\Sessions";
            string sessionAbs = Path.Combine(new string[] { root, sessionsRel });
            foreach (string session in Directory.EnumerateFiles(sessionAbs))
            {
                Dictionary<string, string> d = FileReader.splitFileAndDir(session);
                string fileName = d["file"];
                if (!analyzedSessions.Contains(fileName))
                {
                    FileReader f = new FileReader();
                    string sessionCont = f.readFile(session);
                    Dictionary<string, string> header = TrainingSession.getHeaderInfo(sessionCont);
                    string activity = header["activity"];
                    object activityEnum = Enum.Parse(typeof(Enums.Activity), activity);
                    bool success = true;
                   
                    if (Misc.AreStringEnumEqual(activity, typeof(Enums.Activity), Enums.Activity.Running))
                    {
                        success = mRunningBase.addSession(sessionCont);
                    }
                    else if (Misc.AreStringEnumEqual(activity, typeof(Enums.Activity), Enums.Activity.Biking))
                    {
                        success = mCyclingBase.addSession(sessionCont);
                    }
                    if (success)
                    {
                        File.AppendAllLines(inBaseAbs, new string[] { fileName });
                    }
                }
            }
        }
    }
}
