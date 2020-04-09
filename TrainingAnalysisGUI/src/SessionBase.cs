using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingAnalysis
{
    class SessionBase
    {
        public Enums.Activity mActivity { get; set; }
        private string mBasePath;

        public SessionBase(Enums.Activity activity, string basePath)
        {
            mActivity = activity;
            mBasePath = basePath;
        }

        public SessionBase(Enums.Activity activity)
        {
            mActivity = activity;
            mBasePath = "";
        }

        public bool addSession(string sessionCont)
        {
            return true;
        }

    }
}
