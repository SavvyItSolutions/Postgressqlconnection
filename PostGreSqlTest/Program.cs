using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data;

namespace PostGreSqlTest
{
    class Program
    {
        private DataTable dt = new DataTable();
        static void Main(string[] args)
        {


        DataSet ds = new DataSet();
        NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;User Id=postgres;" +
                               "Password=Wineoutlet@99666;Database=TestDB;");
            conn.Open();

            // Define a query

            //NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM Users.UsersTbl", conn);
            NpgsqlCommand cmd = new NpgsqlCommand("select * from users", conn);
            NpgsqlDataReader dr = cmd.ExecuteReader();
            // Execute a query
            //NpgsqlDataReader dr = cmd.ExecuteReader();

            // Read all rows and output the first column in each row
            while (dr.Read())
                Console.Write("{0}\n", dr[0]);

            // Close connection
            conn.Close();
            Console.ReadLine();
        }
    }
}
