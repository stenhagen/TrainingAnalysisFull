using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingAnalysis.DataStorage
{
    public interface IQueryable
    {
        public string FormatColumnsSQL();
        public string FormatArgsSQL();
        public string GetTableName();
        public string GetPrimaryKeyName();
        public int GetPrimaryKeyValue();

    }
}