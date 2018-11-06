using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

public class DAL
{

    private const string SERVERNAMEFIELD = "Database";  // Connection string name from Web.config
    private static SqlConnection conn;

    public static SqlConnection OpenConnection()
    {
        if (conn != null)
            return conn;
        SqlConnection connection = new SqlConnection("Server=tcp:hacktheirldb.database.windows.net,1433;Initial Catalog=HackTheIRL_DB;Persist Security Info=False;User ID=hacktheirl;Password=2BFLQZhZ;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

        connection.Open();
        conn = connection;
        return conn;
    }

    public static DataTable CreateQuery(string request, SqlConnection connection = null)
    {
        if (connection == null)
            connection = OpenConnection();
        DataTable table = new DataTable();
        SqlDataAdapter adapter = new SqlDataAdapter(request, connection);
        adapter.Fill(table);
        adapter.Dispose();
        return table;
    }

    public static void CreateCommand(String request, SqlConnection connection = null)
    {
        if (connection == null)
            connection = OpenConnection();

        SqlCommand updateCommand = new SqlCommand(request, connection);
        int response = updateCommand.ExecuteNonQuery();
    }

}
