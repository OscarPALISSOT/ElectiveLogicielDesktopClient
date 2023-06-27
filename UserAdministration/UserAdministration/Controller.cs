using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using BCrypt.Net;
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
            builder.DataSource = "NY2AX202-1.numerilab-cesi.fr";//TODO: mettre la chaine de connexion, elle est mise c'est bon
            builder.InitialCatalog = "UserDb";
            builder.TrustServerCertificate = true;
            connection = new SqlConnection(builder.ConnectionString);
            connection.Open();
        }

        ~Controller()
        {
            builder.Clear();
            connection.Close();
        }

        public SqlCommand Select(List<string> columns, List<string> values, string table = "Users") {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("SELECT * FROM {0} WHERE {1} LIKE '%{2}%'", table, columns[0], values[0]);
            for (int i = 1; i < columns.Count; i++)
                command.CommandText += string.Format(" OR {0} LIKE '%{1}%'", columns[i], values[i]);
            command.CommandText += ';';
            return command;
        }

        public SqlDataReader SelectRole(int values)
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("SELECT * FROM UserRole WHERE UserID = {0};", values);
            SqlDataReader reader = command.ExecuteReader();
            return reader;
        }

        public SqlCommand Select(string table = "Users"){
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM " + table + ";";
            return command;
        }

        public void Delete(int ID, bool Users = true){
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = Users ? string.Format("DELETE FROM UserRole WHERE UserID = {0};DELETE FROM Users WHERE UserID = {0};", ID) : string.Format("DELETE FROM UserRole WHERE IDRole = {0};DELETE FROM Roles WHERE IDRole = {0};", ID);
                command.ExecuteNonQuery();
            }
        }

        public void Update(int ID, List<string> values, List<string> columns, List<Box> roles){
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Users SET ";
                for (int i = 0; i < columns.Count; i++)
                {
                    command.CommandText += string.Format("{0} = '{1}',", columns[i], values[i]);
                }
                command.CommandText += "UpdatedAt = GETDATE() WHERE UserID = " + ID + ";";
                command.ExecuteNonQuery();
                command.CommandText = "DELETE FROM UserRole WHERE UserID = " + ID + ";";
                command.ExecuteNonQuery();
                foreach (Box role in roles)
                {
                    command.CommandText = string.Format("INSERT INTO UserRole (UserID, IDRole) VALUES('{0}','{1}');", ID, role.ID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateRole(int ID, string value)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = string.Format("UPDATE Roles SET Roles = '{0}' WHERE IDRole = '{1}';",value, ID);
                command.ExecuteNonQuery();
            }
        }

        public void Insert(List<string> values, List<string> columns, List<Box> roles)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = string.Format("INSERT INTO Users({0}) VALUES('{1}');", string.Join(",", columns), string.Join("','", values));
                command.CommandText += "DECLARE @id INT; SET @id = @@IDENTITY;";
                foreach (Box role in roles)
                    command.CommandText += string.Format("INSERT INTO UserRole (UserID, IDRole) VALUES (@id, {0});", role.ID);
                command.ExecuteNonQuery();
            }
        }

        public void InsertRole(string value)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = string.Format("INSERT INTO Roles(Roles) VALUES('{0}');", value);
                command.ExecuteNonQuery();
            }
        }

        public void DropToken(int ID)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = string.Format("UPDATE Users SET Token = NULL, TokenExpiration = NULL, RefreshToken = NULL, RefreshExpiration = NULL, UpdatedAt = GETDATE() WHERE UserID = '{0}'", ID);
                command.ExecuteNonQuery();
            }
        }
    }
}
