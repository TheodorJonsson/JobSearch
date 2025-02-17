using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace JobSearch.Models
{
    public class JobMethods
    {
        public JobMethods() { }

        public int InsertJob(JobModel job, out string errormsg, int userId)
        {
            if(userId > 0)
            {
                SqlConnection sqlConnection = ConnectToSQL();
                string sqlString = GetInsertString(job);
                System.Diagnostics.Debug.WriteLine(sqlString);
                SqlCommand sqlInsertCommand = new SqlCommand(sqlString, sqlConnection);

                addSqlParamaters(job, sqlInsertCommand, userId);

                try
                {
                    sqlConnection.Open();
                    int i = 0;
                    i = sqlInsertCommand.ExecuteNonQuery();
                    if (i == 1) { errormsg = ""; }
                    else { errormsg = "Insert procedure failed"; }
                    return i;
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
            else
            {
                errormsg = "Please login to add a new job application";
                return -1;
            }
            
        }

        // Updates the job in the database using the inputted data from the forms.
        public int UpdateJob(JobModel job, out string errormsg, int userId)
        {
            if ((userId > 0))
            {
                SqlConnection sqlConnection = ConnectToSQL();
                string sqlString = GetEditString(job);

                System.Diagnostics.Debug.WriteLine(sqlString);
                SqlCommand sqlEditCommand = new SqlCommand(sqlString, sqlConnection);
                addSqlParamaters(job, sqlEditCommand, userId);
                sqlEditCommand.Parameters.Add("@JobId", SqlDbType.Int).Value = job.JobId;
               
                try
                {
                    sqlConnection.Open();
                    int i = 0;
                    i = sqlEditCommand.ExecuteNonQuery();
                    if (i == 1) { errormsg = ""; }
                    else { errormsg = "Insert procedure failed"; }
                    return i;
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
            else
            {
                errormsg = "Please login to add a new job application";
                return -1;
            }
        }
        

        public int DeleteJob(JobModel job, out string errormsg, int userId)
        {
            if(userId > 0)
            {
                SqlConnection sqlConnection = ConnectToSQL();
                string sqlString = "Delete From Jobs Where UserId = @UserId And JobId = @JobId";

                SqlCommand sqlDeleteCommand = new SqlCommand(sqlString, sqlConnection);
                sqlDeleteCommand.Parameters.Add("@JobId", SqlDbType.Int).Value = job.JobId;
                sqlDeleteCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                try
                {
                    sqlConnection.Open();
                    int i = 0;
                    i = sqlDeleteCommand.ExecuteNonQuery();
                    if (i == 1) { errormsg = ""; }
                    else { errormsg = "Insert procedure failed"; }
                    return i;
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
            else
            {
                errormsg = "Please login to add a new job application";
                return -1;
            }

        }


        // Queries the database and creates a list to return the list of job applications for that user
        // default sorting and no filtering
        public IList<JobModel> GetJobList(int userId, bool onGoing, out string errormsg)
        {
            SqlConnection sqlConnection = ConnectToSQL();

            string sqlString = "Select * From Jobs Where UserId = @UserId AND Ongoing = @Ongoing";
            SqlCommand getCommand = new SqlCommand(sqlString, sqlConnection);
            getCommand.Parameters.Add("UserId", System.Data.SqlDbType.Int).Value = userId;
            getCommand.Parameters.Add("Ongoing", System.Data.SqlDbType.Bit).Value = onGoing;

            IList<JobModel> jobList = new List<JobModel>();

            try
            {
                sqlConnection.Open();

                SqlDataReader reader = getCommand.ExecuteReader();
                while (reader.Read())
                {
                    
                    JobModel jM = new JobModel();
                    ReadJobApplicationRow(reader, jM);
                    jobList.Add(jM);
                }
                System.Diagnostics.Debug.WriteLine("Joblist count " + jobList.Count);
                if (jobList.Count < 0)
                {
                    errormsg = "No job applications found";
                    return null;
                }
                errormsg = "";
                return jobList;
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
                return null;
            }
            finally
            {
                sqlConnection.Close();
            }

        }


        // Overloaded method to get filter and sorting in aswell
        public IList<JobModel> GetJobList(int userId, bool onGoing, FilterJobs filter, out string errormsg)
        {
            SqlConnection sqlConnection = ConnectToSQL();

            string sqlString = "Select * From Jobs Where UserId = @UserId AND Ongoing = @Ongoing";
            sqlString = getFiltersSqlString(filter, sqlString);
            System.Diagnostics.Debug.WriteLine(sqlString);
            SqlCommand getCommand = new SqlCommand(sqlString, sqlConnection);
            getCommand.Parameters.Add("UserId", System.Data.SqlDbType.Int).Value = userId;
            getCommand.Parameters.Add("Ongoing", System.Data.SqlDbType.Bit).Value = onGoing;
            GetFilterParameters(filter, getCommand);
            IList<JobModel> jobList = new List<JobModel>();
            System.Diagnostics.Debug.WriteLine("sqlString");
            try
            {
                sqlConnection.Open();

                SqlDataReader reader = getCommand.ExecuteReader();
                while (reader.Read())
                {

                    JobModel jM = new JobModel();
                    ReadJobApplicationRow(reader, jM);
                    jobList.Add(jM);
                }
                System.Diagnostics.Debug.WriteLine("Joblist count " + jobList.Count);
                if (jobList.Count < 0)
                {
                    errormsg = "No job applications found";
                    return null;
                }
                errormsg = "";
                return jobList;
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
                return null;
            }
            finally
            {
                sqlConnection.Close();
            }

        }

        private static void GetFilterParameters(FilterJobs filter, SqlCommand getCommand)
        {
            if (filter != null)
            {
                if (filter.Company != null)
                {
                    getCommand.Parameters.Add("Company", System.Data.SqlDbType.NVarChar, 255).Value = filter.Company;
                }
                if (filter.Location != null)
                {
                    getCommand.Parameters.Add("Location", System.Data.SqlDbType.NVarChar, 255).Value = filter.Location;
                }
                if (filter.Position != null)
                {
                    getCommand.Parameters.Add("Position", System.Data.SqlDbType.NVarChar, 255).Value = filter.Position;
                }
            }
        }

        private static string getFiltersSqlString(FilterJobs filter, string sqlString)
        {
            if (filter != null)
            {
                if (!String.IsNullOrEmpty(filter.Company))
                {
                    sqlString += " AND Company = @Company";
                }
                if (!String.IsNullOrEmpty(filter.Location))
                {
                    sqlString += " AND Location = @Location";
                }
                if (!String.IsNullOrEmpty(filter.Position))
                {
                    sqlString += " AND Position = @Position";
                }
                if (!String.IsNullOrEmpty(filter.SortBy))
                {
                    sqlString += " ORDER BY " + filter.SortBy;
                }
                if (!String.IsNullOrEmpty(filter.OrderBy))
                {
                    sqlString += " " + filter.OrderBy;
                }
            }

            return sqlString;
        }

        // Helper method to not clutter the main function mostly with if statements
        private static void ReadJobApplicationRow(SqlDataReader reader, JobModel jM)
        {
            int ordinal = reader.GetOrdinal("JobId");
            if (!reader.IsDBNull(ordinal))
            {
                jM.JobId = reader.GetInt32(ordinal);
            }
            ordinal = reader.GetOrdinal("Company");
            if (!reader.IsDBNull(ordinal))
            {
                jM.Company = reader.GetString(ordinal);
            }
            ordinal = reader.GetOrdinal("Position");
            if (!reader.IsDBNull(ordinal))
            {
                jM.Position = reader.GetString(ordinal);
            }
            ordinal = reader.GetOrdinal("Location");
            if (!reader.IsDBNull(ordinal))
            {
                jM.Location = reader.GetString(ordinal);
            }
            ordinal = reader.GetOrdinal("Description");
            if (!reader.IsDBNull(ordinal))
            {
                jM.Description = reader.GetString(ordinal);
            }
            ordinal = reader.GetOrdinal("Date");
            if (!reader.IsDBNull(ordinal))
            {
                jM.Date = DateOnly.FromDateTime(reader.GetDateTime(ordinal));
            }
            ordinal = reader.GetOrdinal("Ongoing");
            if (!reader.IsDBNull(ordinal))
            {
                jM.Ongoing = reader.GetBoolean(ordinal);
            }
            ordinal = reader.GetOrdinal("ELevel");
            if (!reader.IsDBNull(ordinal))
            {
                jM.ELevel = reader.GetString(ordinal);
            }
        }

        // Another helper method for the adding of all parameters.
        private static void addSqlParamaters(JobModel job, SqlCommand sqlCommand, int userId)
        {
         
            if (userId > 0)
            {
                sqlCommand.Parameters.Add("userId", System.Data.SqlDbType.Int).Value = userId;
            }
 
            if (job.Company != null)
            {
                sqlCommand.Parameters.Add("Company", System.Data.SqlDbType.NVarChar, 255).Value = job.Company;
            }
            if (job.Date != null)
            {
                sqlCommand.Parameters.Add("Date", System.Data.SqlDbType.Date).Value = job.Date;
            }
            if (job.Position != null)
            {
                sqlCommand.Parameters.Add("Position", System.Data.SqlDbType.NVarChar, 255).Value = job.Position;
            }
            if (job.Location != null)
            {
                sqlCommand.Parameters.Add("Location", System.Data.SqlDbType.NVarChar, 255).Value = job.Location;
            }

            sqlCommand.Parameters.Add("Ongoing", System.Data.SqlDbType.Bit).Value = job.Ongoing;
          
            if (job.Description != null)
            {
                sqlCommand.Parameters.Add("Description", System.Data.SqlDbType.NVarChar, 255).Value = job.Description;
            }
            if(job.ELevel != null)
            {
                sqlCommand.Parameters.Add("ELevel", System.Data.SqlDbType.NVarChar, 255).Value = job.ELevel;
            }
        }



        // Helper method as to not rewrite this multiple times
        // Connects to the database
        private static SqlConnection ConnectToSQL()
        {
            SqlConnection sqlConnection = new SqlConnection();

            sqlConnection.ConnectionString = "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = jobSearchDB; Integrated Security = True; Encrypt = True";
            return sqlConnection;
        }

        // Gets the string for insertion depending on which column has a value or not.
        private static string GetInsertString(JobModel job)
        {
            string sqlStringStart = "Insert Into Jobs (UserId";
            string sqlStringMiddle = ") Values (@UserId";
            string sqlStringEnd = ")";
            if (job.Company != null)
            {
                sqlStringStart += ", Company";
                sqlStringMiddle += ", @Company";
            }
            if (job.Date != null)
            {
                sqlStringStart += ", Date";
                sqlStringMiddle += ", @Date";
            }
            if (job.Position != null)
            {
                sqlStringStart += ", Position";
                sqlStringMiddle += ", @Position";
            }
            if (job.Location != null)
            {
                sqlStringStart += ", Location";
                sqlStringMiddle += ", @Location";
            }
            if (job.Description != null)
            {
                sqlStringStart += ", Description";
                sqlStringMiddle += ", @Description";
            }

            sqlStringStart += ", Ongoing";
            sqlStringMiddle += ", @Ongoing";
            if (job.ELevel != null)
            {
                sqlStringStart += ", ELevel";
                sqlStringMiddle += ", @ELevel";
            }

            return sqlStringStart + sqlStringMiddle + sqlStringEnd;
        }

        private static string GetEditString(JobModel job)
        {
            string sqlStringStart = "Update Jobs Set ";
            string sqlStringEnd = " Where JobId = @JobId And UserId = @UserId";
            if (job.Company != null)
            {
                sqlStringStart += "Company = @Company, ";
            }
            if (job.Date != null)
            {
                sqlStringStart += "Date = @Date, ";
            }
            if (job.Position != null)
            {
                sqlStringStart += "Position = @Position, ";
            }
            if (job.Location != null)
            {
                sqlStringStart += "Location = @Location, ";
            }
            if (job.Description != null)
            {
                sqlStringStart += "Description = @Description, ";
            }

            sqlStringStart += "Ongoing = @Ongoing, ";
            if (job.ELevel != null)
            {
                sqlStringStart += "ELevel = @ELevel, ";
            }

            sqlStringStart = sqlStringStart.Remove(sqlStringStart.LastIndexOf(','));
   

            return sqlStringStart  + sqlStringEnd;
        }
    }
}
