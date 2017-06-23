using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using NLog;

namespace PostGreSqlTest
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private DataTable dt = new DataTable();
        static void Main(string[] args)
        {
            try
            {
                DateTime start = DateTime.Now;
                List<string> lstInsert = new List<string>();
                List<int> lstUpdate = new List<int>();
                string msg = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                logger.Info(msg);
                logger.Info("-------------------------------------------------------");
                int StoreId = Convert.ToInt32(ConfigurationManager.AppSettings["StoreId"]);
                DataSet ds = new DataSet();
                string connectionString = ConfigurationManager.ConnectionStrings["DBCON"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                string commandText = "select LastModifiedDate from  CustomerTrack with (nolock) where storeId = " + StoreId;
                SqlCommand comand = new SqlCommand(commandText, con);
                con.Open();
                logger.Info("Getting last modified from hangouts");
                DateTime lstModified = Convert.ToDateTime(comand.ExecuteScalar());
                logger.Info("Obtained last modified from hangouts");
                con.Close();
                string postGreConnection = ConfigurationManager.ConnectionStrings["PostGreConnection"].ConnectionString;
                NpgsqlConnection conn = new NpgsqlConnection(postGreConnection);
                logger.Info("Trying to open postgresql connection");
                conn.Open();
                logger.Info("Getting latest data from postgresql");
                // Define a query

                //NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM Users.UsersTbl", conn);
                string sql = "select * from customer  where last_modified >'" + lstModified + "'";
                //NpgsqlDataAdapter dr = cmd.ExecuteReader();
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
                da.Fill(ds);
                logger.Info("Dataset Obtained");
                conn.Close();
                if (ds.Tables[0].Rows.Count != 0)
                {
                    DateTime lastRow = DateTime.Now;
                    //string statement = "insert into ICSCustomers select ";
                    logger.Info("Inserting Data to Hangouts customer table");
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        int CustId = Convert.ToInt32(dr["customer_number"]);
                        string firstname = dr["first_name"].ToString();
                        string lastName = dr["last_name"].ToString();
                        string Phone1 = "";
                        string Phone2 = "";
                        if (dr["phone1"] != DBNull.Value)
                            Phone1 = dr["phone1"].ToString();//Phone1 = Convert.ToInt64(dr["phone1"]);
                        if (dr["phone2"] != DBNull.Value)
                            Phone2 = dr["phone2"].ToString();//Phone2 = Convert.ToInt64(dr["phone2"]);
                        string email = dr["email_address"].ToString();
                        string address1 = dr["address1"].ToString();
                        string address2 = dr["address2"].ToString();
                        string city = dr["city"].ToString();
                        string state = dr["state"].ToString();
                        string CustomerType = dr["custtype"].ToString();
                        DateTime CustomerAdded = Convert.ToDateTime(dr["custadded"]);
                        string CardNumber = dr["clubcard1"].ToString();
                        lastRow = Convert.ToDateTime(dr["last_modified"]);
                        //statement += CustId + ",'" + firstname + "','" + lastName + "'," + Phone1 + "," + Phone2 + ",'" + email + "','" + address1 + "','" + address2 + "','" + city + "','" + state + "','" + CustomerType + "','" + CustomerAdded + "','" + CardNumber + "','',0,getdate()";
                        comand = new SqlCommand("InsertUpdateCustomers", con);
                        comand.CommandType = CommandType.StoredProcedure;
                        comand.Parameters.AddWithValue("@CustId", CustId);
                        comand.Parameters.AddWithValue("@firstName", firstname);
                        comand.Parameters.AddWithValue("@lastName", lastName);
                        comand.Parameters.AddWithValue("@Phone1", Phone1);
                        comand.Parameters.AddWithValue("@Phone2", Phone2);
                        comand.Parameters.AddWithValue("@email", email);
                        comand.Parameters.AddWithValue("@address1", address1);
                        comand.Parameters.AddWithValue("@address2", address2);
                        comand.Parameters.AddWithValue("@city", city);
                        comand.Parameters.AddWithValue("@state", state);
                        comand.Parameters.AddWithValue("@CustomerType", CustomerType);
                        comand.Parameters.AddWithValue("@CustomerAdded", CustomerAdded);
                        comand.Parameters.AddWithValue("@CardNumber", CardNumber);
                        con.Open();
                        int Result = Convert.ToInt32(comand.ExecuteScalar());
                        con.Close();
                        if (Result == 1)
                            lstInsert.Add(email);
                        else
                            lstUpdate.Add(CustId);
                    }

                    logger.Info("Data inserted successfully.");
                    string updateQuery = "Update CustomerTrack set LastModifiedDate='" + lastRow + "'";
                    logger.Info(updateQuery);
                    comand = new SqlCommand(updateQuery, con);
                    con.Open();
                    logger.Info("Updating lastmodified in hangouts db");
                    comand.ExecuteNonQuery();
                    logger.Info("Updated successfully");
                    con.Close();
                    for (int i = 0; i < lstInsert.Count; i++)
                    {
                        SendEmail se = new SendEmail();
                        var result = se.SendOneEmail(lstInsert[i]);
                    }

                }
                DateTime end = DateTime.Now;
                TimeSpan duration = start - end;
                logger.Info("Time taken to execute = "+duration);
                // Execute a query
                //NpgsqlDataReader dr = cmd.ExecuteReader();

                // Read all rows and output the first column in each row
                //while (dr.Read())
                //    Console.Write("{0}\n", dr[0]);

                // Close connection

                 //Console.ReadLine();
            }
            catch (Exception ex)
            {
                string path = ConfigurationManager.AppSettings["ErrorLog"];
                string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                message += Environment.NewLine;
                message += "-----------------------------------------------------------";
                message += Environment.NewLine;
                message += string.Format("Message: {0}", ex.Message);
                message += Environment.NewLine;
                message += string.Format("StackTrace: {0}", ex.StackTrace);
                message += Environment.NewLine;
                message += string.Format("Source: {0}", ex.Source);
                message += Environment.NewLine;
                message += string.Format("TargetSite: {0}", ex.TargetSite.ToString());
                message += Environment.NewLine;
                message += "-----------------------------------------------------------";
                message += Environment.NewLine;
                System.IO.Directory.CreateDirectory(path);
                using (StreamWriter writer = new StreamWriter(path + "Error.txt", true))
                {
                    writer.WriteLine(message);
                    writer.Close();
                }
            }
        }
    }
}
