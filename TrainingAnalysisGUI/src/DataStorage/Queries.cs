using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace TrainingAnalysis.DataStorage
{
    internal static class Queries
    {
        private readonly static String ConnectionString = "Server=.;Database=TrainingAnalysis;Trusted_Connection=True;";

        internal static List<T> select<T>(String query, ) where T: IQueryable  
        {

            List<User> users = new List<User>();
            string query = "SELECT * FROM dbo.users";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    var reader = command.ExecuteReader();
                    int ordName = reader.GetOrdinal("username");
                    var columnSchema = reader.GetColumnSchema();
                    while (reader.Read())
                    {
                        string username = reader.GetString(ordName);
                    }
                }
                return 0;
            }
        }

        internal static bool Insert(string username, string password)
        {
            string query = $"INSERT INTO {TableName} VALUES ('{username}', '{password}');";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected == 1;
                    }
                    catch (SqlException)
                    {
                        return false;
                    }
      

    }
}
