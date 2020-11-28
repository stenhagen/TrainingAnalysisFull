using System.Collections.Generic;

using System;
using System.Text;


namespace TrainingAnalysis.DataStorage
{
    public class User: DBEntity
    {

        public int Id { get; private set; }

        public string Username { get; }

        public string Password { get; }

        private readonly static string TableName = "dbo.users";

        private User(int id, string username, string password)
        {
            Id = id;
            Username = username;
            Password = password;
        }


    }
}
