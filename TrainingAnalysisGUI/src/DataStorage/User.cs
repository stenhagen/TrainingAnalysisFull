using System.Collections.Generic;
using System.Data.SqlClient;
using System;
using System.Text;


namespace TrainingAnalysis.DataStorage
{
    public class User
    {

        public int Id { get; private set; }

        public string Username { get; }

        public string Password { get; }

        private static string TableName = "dbo.users";

        private User(int id, string username, string password)
        {
            Id = id;
            Username = username;
            Password = password;
        }

        public static int GetUsers()
        {
            return 0;
            /*
            List<User> users = new List<User>();
            string query = "SELECT * FROM dbo.users";
            using (SqlConnection connection = new SqlConnection(ConnectionSting))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    var reader = command.ExecuteReader();
                
                    int ordName = reader.GetOrdinal("username");
                    while (reader.Read())
                    {
                        string username = reader.GetString(ordName);
                    }
                }
                return 0;
            } 
            */
        }

        public static bool Insert(string username, string password)
        {
            string query = $"INSERT INTO {TableName} VALUES ('{username}', '{password}');";
            using (SqlConnection connection = new SqlConnection(Connection.ConnectionString))
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
        }
    }
}
