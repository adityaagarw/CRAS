using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRAS
{
    internal class pgsql_utilities
    {
        public static string CONNECTION_STRING = "Host=localhost;Username=cras_admin;Password=admin;Database=localdb";

        public static NpgsqlConnection ConnectToPGSQL(string connection_string = "")
        {
            if (connection_string == "") connection_string = CONNECTION_STRING;

            var connection = new NpgsqlConnection(connection_string);

            return connection;
        }

        public static void GetDataFromPGSQL(NpgsqlConnection connection, string table_name, string values = "*", string condition = "")
        {
            if(condition != "")
            {
                condition = " WHERE " + condition;
            }
            string query = "SELECT " + values + " FROM " + table_name + condition;

            connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(query, connection);

            NpgsqlDataReader reader =  command.ExecuteReader();

            while (reader.Read()) 
            {
                Console.WriteLine("{0} {1} {2}", reader.GetString(0), reader.GetString(1), reader.GetString(2));
            }
        }
    }
}
