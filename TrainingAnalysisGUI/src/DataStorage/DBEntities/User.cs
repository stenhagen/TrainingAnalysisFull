using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;


namespace TrainingAnalysis.DataStorage
{
    public class User : IQueryable
    {
        private static string TableName { get; set; } = "[dbo].users"; 
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string GetTableName(){return TableName;} 
        public string FormatColumnsSQL()
        {
            return $"({nameof(ID).ToLower()}, {nameof(Username).ToLower()}, {nameof(Password).ToLower()})";
        }
        public string FormatArgsSQL(){return $"({ID}, {Username}, {Password})";}
        public string GetPrimaryKeyName() { return nameof(ID).ToLower(); }
        
        public int GetPrimaryKeyValue() { return ID; }
    }
}
