

using Elfie.Serialization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace JobSearch.Models
{
    public class UserMethods
    {
        public UserMethods() { }


        // Inserts a user into the database using the stored procedure
        public int InsertUser(UserModel userModel, out string errormsg)
        {
            SqlConnection sqlConnection = ConnectToSQL();

            SqlCommand sqlInsertCommand = new SqlCommand("InsertUser", sqlConnection);

            sqlInsertCommand.CommandType = System.Data.CommandType.StoredProcedure;

            sqlInsertCommand.Parameters.Add(new SqlParameter("@Username", userModel.UserName));
            sqlInsertCommand.Parameters.Add(new SqlParameter("@Password", userModel.Password));

            try
            {
                sqlConnection.Open();
                int i = 0;
                i = sqlInsertCommand.ExecuteNonQuery();
                if (i == 1) { errormsg = ""; }
                else { errormsg = "Insert procedure failed"; }
                return 1;
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
                return 0;
            }
            finally
            {
                sqlConnection.Close();
            }

        }



        // Checks if the username exists in the database
        public bool HasUser(UserModel userModel, out string errormsg)
        {
            SqlConnection sqlConnection = ConnectToSQL();

            string sqlString = "Select * From Users Where Username = @Username";

            SqlCommand getCommand = new SqlCommand(sqlString, sqlConnection);
            getCommand.Parameters.Add("Username", System.Data.SqlDbType.NVarChar, 255).Value = userModel.UserName;

            try
            {
                sqlConnection.Open();
                SqlDataReader reader = getCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    errormsg = "User exists";
                    return true;

                }
                else
                {
                    errormsg = "Username does not exist";
                }
                return false;

            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
                return true;
            }
            finally { sqlConnection.Close(); }

        }


        // Gets the user and sends a usermodel with the id aswell which will be used for the list of jobs.
        // Checks if the user has the right password, it will be hashed before sent in
        public UserModel GetUser(UserModel userModel, out string errormsg)
        {
            SqlConnection sqlConnection = ConnectToSQL();
            string sqlString = "Select * From Users Where Username = @Username AND Password = @Password";

            SqlCommand getCommand = new SqlCommand(sqlString, sqlConnection);
            getCommand.Parameters.Add("Username", System.Data.SqlDbType.NVarChar, 255).Value = userModel.UserName;
            getCommand.Parameters.Add("Password", System.Data.SqlDbType.Binary, 32).Value = CalculateSHA256(userModel.Password);
            try
            {
                sqlConnection.Open();
                SqlDataReader reader = getCommand.ExecuteReader();
                UserModel loggedIn = new UserModel();
                loggedIn.Id = 0;
                while (reader.Read())
                {
                    // Reads the id portion of the data and assigns it to loggedIn usermodel
                    int ordinal = reader.GetOrdinal("UserId");                    
                    if (!reader.IsDBNull(ordinal))
                    {
                        loggedIn.Id = reader.GetInt32(ordinal);
                    }
                    // Reads the username and assigns it to loggedIn usermodel
                    ordinal = reader.GetOrdinal("Username");
                    if (!reader.IsDBNull(ordinal))
                    {
                        loggedIn.UserName = reader.GetString(ordinal);
                    }
                    // Leaves out the password as this is not needed to be stored anywhere
                }
                if(loggedIn.Id != 0 || loggedIn.UserName != null)
                {
                    errormsg = null;
                    return loggedIn;
                }
                errormsg = "Wrong username or password";
                return null;

            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
                return null;
            }
            finally { sqlConnection.Close(); }

        }


        // Helper method as to not rewrite this multiple times
        // Connects to the database
        private static SqlConnection ConnectToSQL()
        {
            SqlConnection sqlConnection = new SqlConnection();

            sqlConnection.ConnectionString = "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = jobSearchDB; Integrated Security = True; Encrypt = True";
            return sqlConnection;
        }


        // SHA256 encryption gotten from "How To Encrypt Passwords Using SHA-256 In C# And .NET" written by Walter G
        // https://www.thatsoftwaredude.com/content/6218/how-to-encrypt-passwords-using-sha-256-in-c-and-net
        // Encrypts the string to SHA256 which is used for the passwords
        private byte[] CalculateSHA256(string str)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] hashValue;
            UTF8Encoding objUtf8 = new UTF8Encoding();
            hashValue = sha256.ComputeHash(objUtf8.GetBytes(str));

            return hashValue;
        }
    }
}
