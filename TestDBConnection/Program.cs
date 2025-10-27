using System;
using System.Data.SqlClient;

class Program
{
    static void Main()
    {
        string connectionString = "Server=localhost\\SQLEXPRESS2019;Database=AvazehDB;User Id=sa;Password=123;TrustServerCertificate=True;";

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                Console.WriteLine("✅ Connection successful!");
                Console.WriteLine("Database: " + conn.Database);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Connection failed: " + ex.Message);
        }
    }
}
