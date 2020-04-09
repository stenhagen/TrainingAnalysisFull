using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace TrainingAnalysis
{
    public class FileReader
    {
        public string readFile(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            else 
            {
                return "NOK";
            }
        }

        public static Dictionary<string, string> splitFileAndDir(string filePath)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            int splitPos = filePath.LastIndexOf(@"\");
            d.Add("dictionary", filePath.Substring(0, splitPos));
            d.Add("file", filePath.Substring(splitPos + 1, filePath.Length - (splitPos + 1)));
            return d;
        }
    }

}
