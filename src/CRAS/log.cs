using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace CRAS
{
    class log
    {
        public DateTime logTime;
        public string source;
        public string module;
        public string actionType;
        public string actionValue;
        public string logMessage;
        public string line;


       public log()
        {

        }
        public log(string source, string module = "", string actionType = "", string actionValue = "", string logMessage = "", string line = "") 
        { 
            if(logTime == null) this.logTime = DateTime.Now;
            this.logTime = DateTime.Now;
            this.source = source;
            this.module = module;
            this.actionType = actionType;
            this.actionValue = actionValue;
            this.logMessage = logMessage;
            this.line = line;
        }
        public FieldInfo[] GetFields()
        {
            Type type = GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return fields;
        }

        public void Print()
        {
            FieldInfo[] fields = GetFields();

            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(this);
                Console.Write($"{field.Name}: {value} ");
            }

        }

        public void LogToFile(string filename = "log.txt")
        {
            string currentDirectory = Directory.GetCurrentDirectory();

            FieldInfo[] fields = GetFields();

            string logString = "";

            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue (this);
                logString += $"{field.Name}: {value}, ";
            }

            logString = logString.Substring(0, logString.Length - 2);

            try
            {
                using (StreamWriter writer = new StreamWriter(filename, true))
                {
                    writer.WriteLine(logString);
                }
            }
            catch
            {
                Console.WriteLine("Error writing log to file: " + filename);
            }
        }

        public void LogToSQL(NpgsqlConnection connection = null, string tableName = "Log")
        {
            if(connection == null)
            {
                connection = pgsql_utilities.ConnectToPGSQL();
            }

            try
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = connection;

                    // Get the fields of the class using reflection
                    FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

                    // Construct the SQL INSERT statement dynamically based on the fields
                    string columns = string.Join(", ", fields.Select(field => field.Name));
                    string values = string.Join(", ", fields.Select(field => $"@{field.Name}"));

                    command.CommandText = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

                    // Add parameters dynamically based on the fields
                    foreach (FieldInfo field in fields)
                    {
                        object value = field.GetValue (this);
                        if(value == null) { value = ""; }
                        command.Parameters.AddWithValue($"@{field.Name}", value);
                    }

                    command.ExecuteNonQuery();
                }

                Console.WriteLine("Data inserted into SQL table successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting data into SQL table: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }
        
    }
}
