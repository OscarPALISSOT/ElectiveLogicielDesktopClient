using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace UserAdministration
{
    public class Controller
    {
        private SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
        private SqlConnection connection;

        public Controller(string user, string password)
        {
            builder.UserID = user;
            builder.Password = password;
            builder.DataSource = "NY2AX202-1.numerilab-cesi.fr";//TODO: mettre la chaine de connexion
            builder.InitialCatalog = "UserDb";
            builder.TrustServerCertificate = true;
            connection = new SqlConnection(builder.ConnectionString);
            connection.Open();
        }

        public SqlCommand Select(List<string> columns, List<string> values) {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("SELECT Users.* FROM Users LEFT JOIN UserRole ON Users.UserID = UserRole.UserID WHERE {0} LIKE '%{1}%'",columns[0], values[0]);
            for (int i = 1; i < columns.Count; i++)
                command.CommandText += string.Format(" OR {0} LIKE '%{1}%'", columns[i], values[i]);
            command.CommandText += ';';
            return command;
        }

        public SqlCommand Select(){
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT Users.* FROM Users LEFT JOIN UserRole ON Users.UserID = UserRole.UserID;";
            return command;
        }

        public void Delete(int ID){
            SqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("DELETE FROM Users WHERE UserID = {0};", ID);
            command.ExecuteNonQuery();
        }

        public void Update(int ID, List<string> values, List<string> columns){
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Users SET ";
            for (int i = 0; i < columns.Count; i++) {
                command.CommandText += string.Format("{0} = '{1}',",columns[i], values[i]);
            }
            command.CommandText += "UpdatedAt = GETDATE() WHERE UserID = " + ID + ";";
            command.ExecuteNonQuery();
        }

        public void Insert(List<string> values, List<string> columns)
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("INSERT INTO Users({0}) VALUES('{1}');", string.Join(",", columns), string.Join("','", values));
            command.ExecuteNonQuery();
        }
     }
}
