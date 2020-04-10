using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingAnalysis
{
    public static class XMLHelper
    {
        // Static class with methods to navigate in the xml-based (.tcx) training-session files.  

        public static string FindNextTagName(string s)
        {
            if (!s.StartsWith("<"))
            {
                Exception ex = new Exception("String " + s + " doesn't start with <");
                throw ex;
            }
            int end = s.IndexOf(">");
            if (end > 2)
            {
                return s.Substring(1, end - 1);
            }
            else
            {
                Exception ex = new Exception("Empty tag name");
                throw ex;
            }
        }

        public static TagPair GetTagPair(string tagName)
        {
            return new TagPair(tagName);
        }

        public static bool ContainsTags(string s)
        {
            return s.StartsWith("<");
        }

        public static bool IsTagGlobal(string s, string tag)
        {
            TagPair tagPair = GetTagPair(tag);
            int stringLength = s.Length;
            int tagEnd = s.IndexOf(tagPair.end) + tagPair.end.Length;
            return stringLength == tagEnd;
        }

        public static string GetInnerString(string s, string tag)
        {
            TagPair tagPair = GetTagPair(tag);
            if (!s.StartsWith(tagPair.start))
            {
                Exception ex = new Exception("String: " + s + " doesn't start with start tag: " + tagPair.start);
                throw ex;
            }
            int innerStart = tagPair.start.Length;
            int innerEnd = s.IndexOf(tagPair.end) - 1;
            if (innerEnd < innerStart)
            {
                Exception ex = new Exception("Empty tag");
                throw ex;
            }
            return s.Substring(innerStart, innerEnd - innerStart + 1);
        }

        public static string getRemainder(string s, string tag)
        {
            TagPair tagPair = GetTagPair(tag);
            if (IsTagGlobal(s, tag))
            {
                return "";
            }
            else
            {
                int stringLength = s.Length;
                string end = tagPair.end;
                int remainderStart = s.IndexOf(end) + end.Length;
                return s.Substring(remainderStart);
            }
        }

        public static string getNextTagContent(string s)
        {
            string tag = FindNextTagName(s);
            TagPair tagPair = GetTagPair(tag);
            int stringLength = s.Length;
            string end = tagPair.end;
            int contentLength = s.IndexOf(end) + end.Length;
            return s.Substring(0, contentLength);
        }

         
    }

    public struct TagPair
    {
        public string start { get; set; }
        public string end { get; set; }

        public TagPair(string tag)
        {
            start = "<" + tag + ">";
            end = "</" + tag + ">";

        }
    }
}

