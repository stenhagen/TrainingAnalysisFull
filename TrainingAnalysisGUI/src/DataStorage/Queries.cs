using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Dapper;


namespace TrainingAnalysis.DataStorage
{
    public static class Queries
    {
        private readonly static string ConnectionString = "Server=.;Database=TrainingAnalysis;Trusted_Connection=True;";

        public static IEnumerable<T> Select<T>(string query) 
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                return connection.Query<T>(query);
            }
        }

        public static Enums.ErrorString Execute(string query)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                int res = connection.Execute(query);
                return res > 0 ? Enums.ErrorString.OK : Enums.ErrorString.NOK;
            }
        }

        public static Enums.ErrorString InsertSingleAllColumns(IQueryable obj)
        {
            string query = $"INSERT INTO {obj.GetTableName()} VALUES {obj.FormatArgsSQL()};";
            return Execute(query);
        }

        public static Enums.ErrorString InsertSingle(IQueryable obj)
        {
            string query = $"INSERT INTO {obj.GetTableName()} {obj.FormatColumnsSQL()} VALUES {obj.FormatArgsSQL()};";
            return Execute(query);
        }
        
        public static Enums.ErrorString DeleteWithPrimaryKey(IQueryable obj)
        {
            string query = $"DELETE FROM {obj.GetTableName()} WHERE {obj.GetPrimaryKeyName()} = {obj.GetPrimaryKeyValue()};";
            return Execute(query);
        }
    }
}
