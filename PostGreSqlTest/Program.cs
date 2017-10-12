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
using System.Net.Http;
using Newtonsoft.Json;


namespace PostGreSqlTest
{
    class Program
    {
        public static readonly string USCode = "1";
        public static readonly string INDCode = "91";
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private DataTable dt = new DataTable();
        static void Main(string[] args)
        {
            
            try
            {
                Program _programobj = new Program();
                DateTime start = DateTime.Now;
                List<Customer> lstInsert = new List<Customer>();
                List<Customer> lstUpdate = new List<Customer>();
                Customer cust = null;
                Dictionary<string, string> EmailDict = new Dictionary<string, string>();
                Dictionary<string, string> SMSDict = new Dictionary<string, string>();
                string userid = ConfigurationManager.AppSettings["SMSLogin"];
                string password = ConfigurationManager.AppSettings["SMSPassword"];
                string msg = Environment.NewLine;
                msg += string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
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
                string sql = "select * from customer  where last_modified > '" + lstModified + "' order by last_modified ";

                //NpgsqlDataAdapter dr = cmd.ExecuteReader();
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
                da.Fill(ds);
                logger.Info("Dataset Obtained");
                conn.Close();
                if (ds.Tables[0].Rows.Count != 0)
                {
                    DateTime lastRow = DateTime.Now;
                    string LastModifiedString = string.Empty;
                    //string statement = "insert into ICSCustomers select ";
                    logger.Info("Inserting Data to Hangouts customer table");
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        cust = new Customer();
                        DataRow dr = ds.Tables[0].Rows[i];
                        cust.Customerid = Convert.ToInt32(dr["customer_number"]);
                        cust.FirstName = dr["first_name"].ToString();
                        cust.LastName = dr["last_name"].ToString();
                        string Phone1 = "";
                        string Phone2 = "";
                        if (dr["phone1"] != DBNull.Value)
                            Phone1 = dr["phone1"].ToString();//Phone1 = Convert.ToInt64(dr["phone1"]);
                        if (dr["phone2"] != DBNull.Value)
                            Phone2 = dr["phone2"].ToString();//Phone2 = Convert.ToInt64(dr["phone2"]);
                        cust.PhoneNumber = Phone1;
                        cust.EmailId = dr["email_address"].ToString();
                        cust.Address1 = dr["address1"].ToString();
                        cust.Address2 = dr["address2"].ToString();
                        cust.City = dr["city"].ToString();
                        cust.State = dr["state"].ToString();
                        cust.CustomerType = dr["custtype"].ToString();
                        DateTime CustomerAdded = DateTime.Now;
                        if (dr["custadded"] != DBNull.Value)
                            CustomerAdded = Convert.ToDateTime(dr["custadded"], System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        cust.CardNumber = dr["clubcard1"].ToString();
                        lastRow = Convert.ToDateTime(dr["last_modified"]);
                        LastModifiedString = dr["last_modified"].ToString();
                        string zip = dr["zip_code"].ToString();
                        if (zip.Length >= 5)
                            zip = zip.Substring(0, 5);
                        string ExpireDate = dr["notes1"].ToString();
                        if (ExpireDate.Contains("Enomatic expiration:"))
                        {
                            ExpireDate = ExpireDate.Replace("Enomatic expiration", ""); 
                        }
                        else
                        {

                            ExpireDate = DateTime.Now.AddYears(1).ToString();
                        }
                        //statement += CustId + ",'" + firstname + "','" + lastName + "'," + Phone1 + "," + Phone2 + ",'" + email + "','" + address1 + "','" + address2 + "','" + city + "','" + state + "','" + CustomerType + "','" + CustomerAdded + "','" + CardNumber + "','',0,getdate()";
                        comand = new SqlCommand("InsertUpdateCustomers", con);
                        comand.CommandType = CommandType.StoredProcedure;
                        comand.Parameters.AddWithValue("@CustId", cust.Customerid);
                        comand.Parameters.AddWithValue("@firstName", cust.FirstName);
                        comand.Parameters.AddWithValue("@lastName", cust.LastName);
                        comand.Parameters.AddWithValue("@Phone1", Phone1);
                        comand.Parameters.AddWithValue("@Phone2", Phone2);
                        comand.Parameters.AddWithValue("@email", cust.EmailId);
                        comand.Parameters.AddWithValue("@address1", cust.Address1);
                        comand.Parameters.AddWithValue("@address2", cust.Address2);
                        comand.Parameters.AddWithValue("@city", cust.City);
                        comand.Parameters.AddWithValue("@state", cust.State);
                        comand.Parameters.AddWithValue("@CustomerType", cust.CustomerType);
                        comand.Parameters.AddWithValue("@CustomerAdded", CustomerAdded);
                        comand.Parameters.AddWithValue("@CardNumber", cust.CardNumber);
                        comand.Parameters.AddWithValue("@Zip", zip);
                        comand.Parameters.AddWithValue("@ExpireDate",ExpireDate );
                        con.Open();
                        int Result = Convert.ToInt32(comand.ExecuteScalar());
                        con.Close();
                        if (Result == 1)
                        {
                            logger.Info("CustomerId inserted = " + cust.Customerid);
                            
                            //string trimmedFirstName = "";
                            if (cust.FirstName.Length > 13)
                            {
                                cust.FirstName = cust.FirstName.Substring(0, 13);
                            }
                            lstInsert.Add(cust);
                        }
                        else
                        {
                            logger.Info("CustomerId updated = " + cust.Customerid);
                            lstUpdate.Add(cust);
                        }
                    }

                    logger.Info("Data inserted successfully.");
                    string updateQuery = "Update CustomerTrack set LastModifiedDate='" + lastRow + "',LastModifiedString ='"+LastModifiedString +"'";
                    logger.Info(updateQuery);
                    updateQuery += "where storeid = " + StoreId;
                    comand = new SqlCommand(updateQuery, con);
                    con.Open();
                    logger.Info("Updating lastmodified in hangouts db");
                    comand.ExecuteNonQuery();
                    logger.Info("Updated successfully");
                    con.Close();
                    if (lstInsert.Count > 0)
                    {
                        foreach (Customer cu in lstInsert)
                        {
                            logger.Info("Sending sms");
                            string message = string.Empty;
                            SendSMS sms = new SendSMS();
                            if (cu.CardNumber != "" || cu.CardNumber != string.Empty)
                            {
                                message = "THANK YOU for becoming a V.I.P.CLUB CARD member at Wineoutlet.Plz try our IOS App https://goo.gl/RdXfDo or Android https://goo.gl/ewTw4r";
                            }
                            else
                            {
                                message = "Hi " + cu.FirstName + ", We're glad to have you onboard with WineOutlet!Plz try our iOS App https://goo.gl/RdXfDo Android App https://goo.gl/ewTw4r";
                            }
                            if (cu.PhoneNumber == "" || cu.PhoneNumber == string.Empty)
                            {
                                logger.Info("Mobile Number not present for "+cu.Customerid);
                            }
                            else
                            {
                                 sms.SendAlertSMS(userid, password, USCode + cu.PhoneNumber, message);
                            }

                            if (cu.EmailId != string.Empty|| cu.EmailId != "")
                            {
                                SendEmail se = new SendEmail();
                                if (cu.CardNumber != "" || cu.CardNumber != string.Empty)
                                {
                                    int x = se.UpdateVIPMail(cu.EmailId, cu.FirstName).Result;
                                }
                                else
                                {
                                    int x = se.UpdateMail(cu.EmailId, cu.FirstName).Result;
                                }
                            }
                        }
                    }

                }
                DateTime end = DateTime.Now;
                TimeSpan duration = end - start;
                logger.Info("Time taken to execute = "+duration);
                
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
