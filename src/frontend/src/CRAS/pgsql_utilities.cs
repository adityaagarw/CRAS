using Npgsql;
using NpgsqlTypes;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRAS
{
    internal class pgsql_utilities
    {
        public static string CONNECTION_STRING = "Host=127.0.0.1;Username=cras_admin;Password=admin;Database=localdb";

        public static NpgsqlConnection ConnectToPGSQL(string connection_string = "")
        {
            if (connection_string == "") connection_string = CONNECTION_STRING;

            var connection = new NpgsqlConnection(connection_string);

            return connection;
        }

        
        public static BindingList<redis_customer> GetCustomerDetails(NpgsqlConnection connection, string whereClause = "")
        {
            BindingList<redis_customer> customer_list = new BindingList<redis_customer>();
            if(connection.State == System.Data.ConnectionState.Closed) connection.Open();

            string condition = "";

            if(whereClause.Length > 0)
            {
                condition = $" WHERE {whereClause}";
            }

            /*if(customer_id != "*" && customer_id.Length > 0)
            {
                condition = $" WHERE customer_id = '{customer_id}'";
            }*/

            string query = "SELECT customer_id, name, phone_number, image, return_customer, last_visit, average_time_spent, average_bill_value, average_bill_per_visit, average_bill_per_billed_visit, maximum_purchase, remarks, loyalty_level, num_bills, num_visits, num_billed_visits, last_location, category, creation_date, group_id FROM local_customer_db" + condition;
            //string query = "SELECT customer_id, image FROM local_customer_db" + condition;

            Console.WriteLine("Get Customer details from local_db: " + query);
            NpgsqlCommand command = new NpgsqlCommand(query, connection);

            NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                redis_customer customer = new redis_customer();
                /*
                //customer.key = reader.GetString(reader.GetOrdinal("key"));
                customer.customer_id = reader.GetGuid(reader.GetOrdinal("customer_id")).ToString();
                customer.name = reader.GetString(reader.GetOrdinal("name"));
                if (!reader.IsDBNull(reader.GetOrdinal("phone_number")))
                    customer.phone_number = reader.GetString(reader.GetOrdinal("phone_number"));
                customer.image = (byte[])reader["image"];
                customer.return_customer = reader.GetInt32(reader.GetOrdinal("return_customer")).ToString();
                customer.last_visit = reader.GetString(reader.GetOrdinal("last_visit"));
                customer.average_time_spent = reader.GetFloat(reader.GetOrdinal("average_time_spent"));
                customer.average_bill_value = reader.GetFloat(reader.GetOrdinal("average_bill_value"));
                customer.average_bill_per_visit = reader.GetFloat(reader.GetOrdinal("average_bill_per_visit"));
                customer.average_bill_per_billed_visit = reader.GetFloat(reader.GetOrdinal("average_bill_per_billed_visit"));
                customer.maximum_purchase = reader.GetFloat(reader.GetOrdinal("maximum_purchase"));
                
                customer.remarks = reader.GetString(reader.GetOrdinal("remarks"));
                customer.loyalty_level = reader.GetInt32(reader.GetOrdinal("loyalty_level"));
                customer.num_bills = reader.GetInt32(reader.GetOrdinal("num_bills"));
                customer.num_visits = reader.GetInt32(reader.GetOrdinal("num_visits"));
                customer.num_billed_visits = reader.GetInt32(reader.GetOrdinal("num_billed_visits"));
                customer.last_location = reader.GetString(reader.GetOrdinal("last_location"));

                // Parse location_list as a string and split it into a list of strings
                string locationListString = reader.GetString(reader.GetOrdinal("location_list"));
                customer.location_list = new List<string>(locationListString.Split(','));

                customer.category = reader.GetString(reader.GetOrdinal("category"));
                customer.creation_date = reader.GetDateTime(reader.GetOrdinal("creation_date"));
                customer.group_id = reader.GetInt32(reader.GetOrdinal("group_id"));
                */
                customer.customer_id = reader.IsDBNull(reader.GetOrdinal("customer_id")) ? null : reader.GetGuid(reader.GetOrdinal("customer_id")).ToString();
                customer.name = reader.IsDBNull(reader.GetOrdinal("name")) ? "" : reader.GetString(reader.GetOrdinal("name"));
                customer.phone_number = reader.IsDBNull(reader.GetOrdinal("phone_number")) ? null : reader.GetString(reader.GetOrdinal("phone_number"));
                customer.image = reader.IsDBNull(reader.GetOrdinal("image")) ? null : (byte[])reader["image"];
                customer.return_customer = reader.IsDBNull(reader.GetOrdinal("return_customer")) ? null : reader.GetInt32(reader.GetOrdinal("return_customer")).ToString();
                customer.last_visit = reader.IsDBNull(reader.GetOrdinal("last_visit")) ? default : reader.GetDateTime(reader.GetOrdinal("last_visit"));
                customer.average_time_spent = reader.IsDBNull(reader.GetOrdinal("average_time_spent")) ? 0 : (int)reader.GetInt32(reader.GetOrdinal("average_time_spent"));
                customer.average_bill_value = reader.IsDBNull(reader.GetOrdinal("average_bill_value")) ? 0 : (float)reader.GetFloat(reader.GetOrdinal("average_bill_value"));
                customer.average_bill_per_visit = reader.IsDBNull(reader.GetOrdinal("average_bill_per_visit")) ? 0 : (float)reader.GetFloat(reader.GetOrdinal("average_bill_per_visit"));
                customer.average_bill_per_billed_visit = reader.IsDBNull(reader.GetOrdinal("average_bill_per_billed_visit")) ? 0 : (float)reader.GetFloat(reader.GetOrdinal("average_bill_per_billed_visit"));
                customer.maximum_purchase = reader.IsDBNull(reader.GetOrdinal("maximum_purchase")) ? 0 : (float)reader.GetFloat(reader.GetOrdinal("maximum_purchase"));
                customer.remarks = reader.IsDBNull(reader.GetOrdinal("remarks")) ? null : reader.GetString(reader.GetOrdinal("remarks"));
                customer.loyalty_level = reader.IsDBNull(reader.GetOrdinal("loyalty_level")) ? null : reader.GetString(reader.GetOrdinal("loyalty_level"));
                customer.num_bills = reader.IsDBNull(reader.GetOrdinal("num_bills")) ? 0 : (int)reader.GetInt32(reader.GetOrdinal("num_bills"));
                customer.num_visits = reader.IsDBNull(reader.GetOrdinal("num_visits")) ? 0 : (int)reader.GetInt32(reader.GetOrdinal("num_visits"));
                customer.num_billed_visits = reader.IsDBNull(reader.GetOrdinal("num_billed_visits")) ? 0 : (int)reader.GetInt32(reader.GetOrdinal("num_billed_visits"));
                customer.last_location = reader.IsDBNull(reader.GetOrdinal("last_location")) ? null : reader.GetString(reader.GetOrdinal("last_location"));
                /*//string locationListString = reader.GetString(reader.GetOrdinal("location_list"));
                //customer.location_list = locationListString.Split(',');
                */
                customer.category = reader.IsDBNull(reader.GetOrdinal("category")) ? null : reader.GetString(reader.GetOrdinal("category"));
                customer.creation_date = reader.GetDateTime(reader.GetOrdinal("creation_date"));
                customer.group_id = reader.IsDBNull(reader.GetOrdinal("group_id")) ? 0 : (int)reader.GetInt32(reader.GetOrdinal("group_id"));
                customer_list.Add(customer);
            }
            connection.Close();

            return customer_list;
        }

        public static DataTable GetDailyOverview(NpgsqlConnection connection, string fromDate, string toDate = "")
        {
            DataTable overview = new DataTable();

            if (toDate.Length == 0) toDate = fromDate;

            connection.Open();

            string query = $"SELECT COUNT(visit_id) as TotalVisits, COUNT(customer_id) as TotalVisitors, COUNT(DISTINCT customer_id) as UniqueVisitors, MAX(time_spent) as MaxTime, MIN(time_spent) as MinTime, SUM(time_spent) as TotalTime, SUM(return_customer) as ReturnCustomers  FROM local_visit_db WHERE incomplete = '0' AND CAST(exit_time AS DATE) >= '{fromDate}' AND CAST(exit_time AS DATE) <= '{toDate}'";

            Console.WriteLine(query);

            NpgsqlCommand command = new NpgsqlCommand(query, connection);

            NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter();
            dataAdapter.SelectCommand = command;
            dataAdapter.Fill(overview);

            connection.Close();

            return overview;
        }
        
        public static DataTable GetTableData(NpgsqlConnection connection, string table_name, string whereClause = "", string orderByClause = "", string limit = "")
        {
            DataTable tableData = new DataTable();
            connection.Open();

            string query = "SELECT * FROM " + table_name;

            if (table_name.Equals("local_customer_db"))
            {
                query = "SELECT customer_id, name, phone_number, image, return_customer, last_visit, average_time_spent, average_bill_value, average_bill_per_visit, average_bill_per_billed_visit, maximum_purchase, remarks, loyalty_level, num_bills, num_visits, num_billed_visits, last_location, category FROM local_customer_db";
            }
            if (table_name.Equals("local_employee_db"))
            {
                query = "SELECT employee_id, name, phone_number, face_image from local_employee_db";
            }
            if (table_name.Equals("log"))
            {
                query = "SELECT * FROM log";
            }

            if (whereClause.Length > 0)
            {
                query += " " + whereClause;
            }

            if (orderByClause.Length > 0)
            { query += " " + orderByClause; }

            if (limit.Length > 0)
            {
                query += " " + limit;
            }

            Console.WriteLine(query);
            NpgsqlCommand command = new NpgsqlCommand(query, connection);

            NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter();
            dataAdapter.SelectCommand = command;
            dataAdapter.Fill(tableData);

            connection.Close();
            return tableData;
        }

        public static DataTable GetLogData(NpgsqlConnection connection,string table_name = "log", string whereClause = "", string limit = "")
        {
            DataTable tableData = new DataTable();
            connection.Open();

            string query = "SELECT * FROM " + table_name;

            

            NpgsqlCommand command = new NpgsqlCommand(query, connection);

            NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter();
            dataAdapter.SelectCommand = command;
            dataAdapter.Fill(tableData);

            connection.Close();
            return tableData;
        }
        public static List<string> GetListofTables(NpgsqlConnection connection)
        {
            List<string> tables = new List<string>();

            connection.Open();
            string query = "SELECT table_name from information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE'";

            Console.WriteLine("Get Visit details from local_db: " + query);
            NpgsqlCommand command = new NpgsqlCommand(query, connection);

            NpgsqlDataReader reader = command.ExecuteReader();

            while(reader.Read())
            {
                string table = reader.GetString(reader.GetOrdinal("table_name"));
                tables.Add(table);
            }

            connection.Close();
            return tables;
        }
        public static void UpdateCustomerDetails(NpgsqlConnection connection, redis_customer customer)
        {
            connection.Open();

            //string query = "UPDATE local_customer_db SET name = @name, phone_number = @phone_number, average_bill_value = @average_bill_value, average_bill_per_visit = @average_bill_per_visit, average_bill_per_billed_visit = @average_bill_per_billed_visit, num_bills = @num_bills, num_visits = @num_visits, num_billed_visits = @num_billed_visits WHERE customer_id = @customer_id";

            //using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            //{
            /*command.Parameters.AddWithValue("name", customer.name);
            command.Parameters.AddWithValue("phone_number", customer.phone_number);
            command.Parameters.AddWithValue("average_bill_value", customer.average_bill_value);
            command.Parameters.AddWithValue("average_bill_per_visit", customer.average_bill_per_visit);
            command.Parameters.AddWithValue("average_bill_per_billed_visit", customer.average_bill_per_billed_visit);
            command.Parameters.AddWithValue("num_bills", customer.num_bills);
            command.Parameters.AddWithValue("num_visits", customer.num_visits);
            command.Parameters.AddWithValue("num_billed_visits", customer.num_billed_visits);
            command.Parameters.AddWithValue("customer_id", customer.customer_id); */

            /*command.Parameters.AddWithValue("name", NpgsqlDbType.Varchar, customer.name);
            command.Parameters.AddWithValue("phone_number", NpgsqlDbType.Varchar, customer.phone_number);
            command.Parameters.AddWithValue("average_bill_value", NpgsqlDbType.Numeric, customer.average_bill_value);
            command.Parameters.AddWithValue("average_bill_per_visit", NpgsqlDbType.Numeric, customer.average_bill_per_visit);
            command.Parameters.AddWithValue("average_bill_per_billed_visit", NpgsqlDbType.Numeric, customer.average_bill_per_billed_visit);
            command.Parameters.AddWithValue("num_bills", NpgsqlDbType.Integer, customer.num_bills);
            command.Parameters.AddWithValue("num_visits", NpgsqlDbType.Integer, customer.num_visits);
            command.Parameters.AddWithValue("num_billed_visits", NpgsqlDbType.Integer, customer.num_billed_visits);
            command.Parameters.AddWithValue("customer_id", NpgsqlDbType.Uuid, customer.customer_id);
            command.ExecuteNonQuery();*/
            string visit_id = "";
            visit_details visit = new visit_details();
            visit = redis_utilities.GetVisitDetails(MainForm.redisConnection, customer.customer_id)[0];
            if (visit != null) visit_id = visit.visit_id;
            string query = $"UPDATE local_customer_db SET name = '{customer.name}', phone_number = '{customer.phone_number}', visit_id = {visit_id}, average_bill_value = {customer.average_bill_value}, average_bill_per_visit = {customer.average_bill_per_visit}, average_bill_per_billed_visit = {customer.average_bill_per_billed_visit}, num_bills = {customer.num_bills}, num_visits = {customer.num_visits}, num_billed_visits = {customer.num_billed_visits} WHERE customer_id = '{customer.customer_id}'";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            Console.WriteLine("Updating Customer in local_customer_db: " + query);
            command.ExecuteNonQuery();
            //}
        }
        public static void UpdateVisitDetails(NpgsqlConnection connection, visit_details visit)
        {
            connection.Open();

            //string query = "UPDATE local_visit_db SET store_id = @store_id, entry_time = @entry_time, exit_time = @exit_time, billed = @billed, bill_no = @bill_no, bill_date = @bill_date, bill_amount = @bill_amount,return_amount = @return_amount, time_spent = @time_spent, visit_remark = @visit_remark, customer_rating = @customer_rating, customer_feedback = @customer_feedback, incomplete = @incomplete WHERE visit_id = @visit_id";
            /*using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("visit_id", NpgsqlDbType.Uuid, visit.visit_id);
                command.Parameters.AddWithValue("store_id", NpgsqlDbType.Uuid, visit.store_id);
                command.Parameters.AddWithValue("entry_time", NpgsqlDbType.Timestamp, visit.entry_time);
                command.Parameters.AddWithValue("exit_time", NpgsqlDbType.Timestamp, visit.exit_time);
                command.Parameters.AddWithValue("billed", NpgsqlDbType.Integer, visit.billed);
                command.Parameters.AddWithValue("bill_no", NpgsqlDbType.Varchar, visit.bill_no);
                command.Parameters.AddWithValue("bill_date", NpgsqlDbType.Timestamp, visit.bill_date);
                command.Parameters.AddWithValue("bill_amount", NpgsqlDbType.Numeric, visit.bill_amount);
                command.Parameters.AddWithValue("return_amount", NpgsqlDbType.Numeric, visit.return_amount);
                command.Parameters.AddWithValue("time_spent", NpgsqlDbType.Interval, visit.time_spent);
                command.Parameters.AddWithValue("visit_remark", NpgsqlDbType.Text, visit.visit_remark);
                command.Parameters.AddWithValue("customer_rating", NpgsqlDbType.Integer, visit.customer_rating);
                command.Parameters.AddWithValue("customer_feedback", NpgsqlDbType.Integer, visit.customer_feedback);
                command.Parameters.AddWithValue("incomplete", NpgsqlDbType.Integer, visit.incomplete);

                command.ExecuteNonQuery();
            }*/
            string query = $"UPDATE local_visit_db SET store_id = '{visit.store_id}', entry_time = '{visit.entry_time}', exit_time = '{visit.exit_time}', billed = {visit.billed}, bill_no = '{visit.bill_no}', bill_date = '{visit.bill_date}', bill_amount = {visit.bill_amount}, return_amount = {visit.return_amount}, time_spent = '{visit.time_spent}', visit_remark = '{visit.visit_remark}', customer_rating = {visit.customer_rating}, customer_feedback = {visit.customer_feedback}, incomplete = {visit.incomplete} WHERE visit_id = '{visit.visit_id}'";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            Console.WriteLine("Updating Visit in local_visit_db: " + query);
            command.ExecuteNonQuery();
        }

        public static BindingList<visit_details> GetVisitDetails(NpgsqlConnection connection, string whereClause = "")
        {
            BindingList<visit_details> visits = new BindingList<visit_details>();
            connection.Open();

            string condition = "";

            /*if (customer_id != "*" && customer_id.Length > 0)
            {
                condition = $"WHERE customer_id = '{customer_id}'";
            }*/

            if (whereClause.Length > 0)
            {
                condition = $"WHERE {whereClause}";
            }

            string query = "SELECT * FROM local_visit_db " + condition + " ORDER BY entry_time DESC";

            Console.WriteLine("Get Visit details from local_visit_db: " + query);
            NpgsqlCommand command = new NpgsqlCommand(query, connection);

            NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                visit_details visit = new visit_details();
                //visit.key = reader.GetString(reader.GetOrdinal("key"));
                visit.customer_id = reader.IsDBNull(reader.GetOrdinal("customer_id")) ? default : reader.GetGuid(reader.GetOrdinal("customer_id")).ToString();
                visit.visit_id = reader.IsDBNull(reader.GetOrdinal("visit_id")) ? default : reader.GetGuid(reader.GetOrdinal("visit_id")).ToString();
                visit.store_id = reader.IsDBNull(reader.GetOrdinal("store_id")) ? default : reader.GetGuid(reader.GetOrdinal("store_id")).ToString();
                visit.entry_time = reader.IsDBNull(reader.GetOrdinal("entry_time")) ? default : reader.GetDateTime(reader.GetOrdinal("entry_time"));
                visit.exit_time = reader.IsDBNull(reader.GetOrdinal("exit_time"))? default : reader.GetDateTime(reader.GetOrdinal("exit_time"));
                visit.billed = reader.IsDBNull(reader.GetOrdinal("billed")) ? default : reader.GetInt32(reader.GetOrdinal("billed"));
                visit.bill_no = reader.IsDBNull(reader.GetOrdinal("bill_no")) ? null: reader.GetString(reader.GetOrdinal("bill_no"));
                visit.bill_date = reader.IsDBNull(reader.GetOrdinal("bill_date")) ? default : reader.GetDateTime(reader.GetOrdinal("bill_date"));
                visit.bill_amount = reader.IsDBNull(reader.GetOrdinal("bill_amount")) ? default : reader.GetInt32(reader.GetOrdinal("bill_amount"));
                visit.return_amount = reader.IsDBNull(reader.GetOrdinal("return_amount")) ? default : reader.GetInt32(reader.GetOrdinal("return_amount"));
                visit.time_spent = reader.IsDBNull(reader.GetOrdinal("time_spent")) ? default : reader.GetInt32(reader.GetOrdinal("time_spent"));
                visit.visit_remark = reader.IsDBNull(reader.GetOrdinal("visit_remark")) ? default : reader.GetString(reader.GetOrdinal("visit_remark"));
                visit.customer_rating = reader.IsDBNull(reader.GetOrdinal("customer_rating")) ? default : reader.GetInt32(reader.GetOrdinal("customer_rating"));
                visit.customer_feedback = reader.IsDBNull(reader.GetOrdinal("customer_feedback")) ? default : reader.GetInt32(reader.GetOrdinal("customer_feedback"));
                visit.incomplete = reader.IsDBNull(reader.GetOrdinal("incomplete")) ? default : reader.GetInt32(reader.GetOrdinal("incomplete"));

                visits.Add(visit);
            }
            connection.Close();
            return visits;
        }

        public static BindingList<bill_details> GetBillDetails(NpgsqlConnection connection, string bill_no = "", string bill_date = "", int count = 0)
        {
            BindingList<bill_details> bills = new BindingList<bill_details>();
            connection.Open();

            string condition = "";
            string limit = "";

            if (bill_date.Length > 0 || bill_no.Length > 0)
            {
                string and_clause = "";
                if (bill_date.Length > 0 && bill_no.Length > 0) and_clause += " AND ";
                condition = $"WHERE ";

                if (bill_date.Length > 0) condition += $"bill_date = '{bill_date}' ";

                condition += and_clause;
                
                if (bill_no.Length > 0) condition += $"bill_no = '{bill_no}' ";
            }

            if (count > 0) limit = $"LIMIT {count}";

            string query = "SELECT * FROM local_billing_db " + condition + " ORDER BY bill_date " + limit;

            Console.WriteLine("Get Bill details from local_db: " + query);
            NpgsqlCommand command = new NpgsqlCommand(query, connection);

            NpgsqlDataReader reader = command.ExecuteReader();

            List<string> customer_ids = new List<string>();
            List<string[]> customer_lists = new List<string[]>();
            List<string[]> visit_lists = new List<string[]>();


            while (reader.Read())
            {
                NpgsqlConnection temp_con2 = ConnectToPGSQL();

                bill_details bill = new bill_details();

                bill.billNo = reader["bill_no"].ToString();
                bill.name = reader["name"].ToString();
                bill.billDate = (DateTime)reader["bill_date"];
                bill.mobile = reader["phone_number"].ToString();
                bill.billAmt = reader["bill_amount"].ToString();
                bill.returnAmt = reader["return_amount"].ToString();
                bill.qty = reader["quantity"].ToString();
                bill.billAmtInt = Convert.ToInt32(reader["bill_amount"]);
                bill.returnAmtInt = Convert.ToInt32(reader["return_amount"]);
                bill.qtyInt = Convert.ToInt32(reader["quantity"]);

                string customer_id = reader["customer_id"].ToString();

                string customer_source = "";
                if(customer_id.Length > 0) bill.selected_customer = utilities.GetCustomerDetails(customer_id, ref customer_source);

                bill.visit_id = reader["visit_id"].ToString();
                string[] visit_list = reader["visit_list"] as string[];
                string[] customer_list = reader["customer_list"] as string[];

                foreach (string customer in customer_list)
                {
                    if (customer.Length > 0)
                    {
                        NpgsqlConnection temp_con = ConnectToPGSQL();
                        redis_customer identified_customer = utilities.GetCustomerDetails(customer,ref customer_source);

                        if (identified_customer != null) bill.identified_customers.Add(identified_customer);
                    }
                }
                /*visit_lists.Add(
                customer_lists.Add(customer_list);
                */
                bills.Add(bill);
            }
            connection.Close();
            /*
            foreach (bill_details bill in bills)
            {
                bill.selected_customer = GetCustomerDetails(connection, bill.sele)[0];

                foreach (string customer )
                redis_customer identified_customer = GetCustomerDetails(connection, customer)[0];

                if (identified_customer != null) bill.identified_customers.Add(identified_customer);
            }*/

            return bills;
        }

        public static void UpdateBillDetails(NpgsqlConnection connection, bill_details bill)
        {

            connection.Open();

            //string query = "UPDATE local_billing_db SET name = @name, mobile = @mobile, bill_amount = @bill_amount, return_amount = @return_amount, quantity = @quantity, customer_id = @customer_id, visit_id = @visit_id, customer_list = @customer_list, visit_list = @visit_list WHERE bill_no = @bill_no AND bill_date = @bill_date";
            //Console.WriteLine("Updating local_billing_db: " + query);

            string name = "";
            string customer_list = "{";
            string visit_list = "{";
            string temp = "";
            int index = 0;
            string selected_customer_id = null;
            string selected_visit_id = null;

            if (bill.identified_customers != null && bill.identified_customers.Count>0)
            {
                foreach (redis_customer customer in bill.identified_customers)
                {

                    customer_list += customer.customer_id + ",";
                    visit_list += utilities.GetVisitDetails(customer.customer_id, ref temp).visit_id + ",";
                    index++;
                }
                customer_list = customer_list.Remove(customer_list.Length - 1, 1);
                visit_list = visit_list.Remove(visit_list.Length - 1, 1);
            }
            customer_list += "}";
            visit_list += "}";

            if (bill.selected_customer != null) { selected_customer_id = bill.selected_customer.customer_id; }
            if (bill.visit_id != null) { selected_visit_id = bill.visit_id; }

            /*using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("bill_no", NpgsqlDbType.Varchar, bill.billNo);
                command.Parameters.AddWithValue("bill_date", NpgsqlDbType.Timestamp, bill.billDate);
                command.Parameters.AddWithValue("bill_amount", NpgsqlDbType.Numeric, bill.billAmtInt);
                command.Parameters.AddWithValue("return_amount", NpgsqlDbType.Numeric, bill.returnAmtInt);
                command.Parameters.AddWithValue("quantity", NpgsqlDbType.Integer, bill.qtyInt);
                command.Parameters.AddWithValue("name", NpgsqlDbType.Varchar, bill.name);
                command.Parameters.AddWithValue("phone_number", NpgsqlDbType.Varchar, bill.mobile);
                command.Parameters.AddWithValue("customer_id", NpgsqlDbType.Varchar, bill.selected_customer);
                command.Parameters.AddWithValue("visit_id", NpgsqlDbType.Varchar, bill.visit_id);
                command.Parameters.AddWithValue("customer_list", NpgsqlDbType.Array | NpgsqlDbType.Text, customer_list);
                command.Parameters.AddWithValue("visit_list", NpgsqlDbType.Array | NpgsqlDbType.Text, visit_list);


                command.ExecuteNonQuery();
            }*/

            string query = $"UPDATE local_billing_db SET name = '{bill.name}', phone_number = '{bill.mobile}', bill_amount = {bill.billAmtInt}, return_amount = {bill.returnAmtInt}, quantity = {bill.qtyInt}, customer_id = '{selected_customer_id}', visit_id = '{selected_visit_id}', customer_list = '{customer_list}', visit_list = '{visit_list}' WHERE bill_no = '{bill.billNo}' AND bill_date = TO_TIMESTAMP('{bill.billDate.ToString("yyyy-MM-dd HH:mm:ss")}', 'YYYY-MM-DD HH24:MI:SS')";
            Console.WriteLine("Updating local_billing_db: " + query);
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.ExecuteNonQuery();
            connection.Close();

        }

        public static int GetMaxSessionId(NpgsqlConnection connection)
        {
            int maxSessionId = 0;

            connection.Open();

                string query = "SELECT MAX(sessionid) AS MaxSessionId FROM Session";
            Console.WriteLine(query);

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Check if the returned value is not DBNull
                            if (reader["MaxSessionId"] != DBNull.Value)
                            {
                                maxSessionId = Convert.ToInt32(reader["MaxSessionId"]);
                            }
                        }
                    }
                }
                connection.Close();
            return maxSessionId;
        }

        public static int GetMaxSubSessionId(NpgsqlConnection connection, int sessionId)
        {
            int maxSubSessionId = 0;

            connection.Open();

            string query = $"SELECT MAX(sub_sessionid) AS MaxSubSessionId FROM SubSession WHERE sessionid = '{sessionId}'";
            Console.WriteLine(query);

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Check if the returned value is not DBNull
                        if (reader["MaxSubSessionId"] != DBNull.Value)
                        {
                            maxSubSessionId = Convert.ToInt32(reader["MaxSubSessionId"]);
                        }
                    }
                }
            }

            connection.Close(); 
            return maxSubSessionId;
        }

        public static int InsertSubSession(NpgsqlConnection connection, int sessionId, int subSessionId, string startTime = "")
        {
            int success = 0;
            if(startTime.Length == 0) startTime = DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss");
            
                connection.Open();

                string query = $"INSERT INTO SubSession (sessionid, sub_sessionid, start_time) " +
                               $"VALUES ('{sessionId}', '{subSessionId}', '{startTime}')";

                Console.WriteLine(query);
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    /*command.Parameters.AddWithValue("@sessionId", sessionId);
                    command.Parameters.AddWithValue("@subSessionId", subSessionId);
                    command.Parameters.AddWithValue("@startTime", startTime);
                    command.Parameters.AddWithValue("@endTime", endTime);*/

                    command.ExecuteNonQuery();
                success = 1;
                }
            connection.Close();
            return success;
        }

        public static void UpdateSubSession(NpgsqlConnection connection, int sessionId, int subSessionId, string endTime = "")
        {
            if(endTime.Length == 0) endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            connection.Open();

            string query = $"UPDATE SubSession SET end_time = '{endTime.ToString()}' WHERE sessionid = '{sessionId.ToString()}' AND sub_sessionid = '{subSessionId.ToString()}'";

            Console.WriteLine(query);

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                /*command.Parameters.AddWithValue("@sessionId", sessionId);
                command.Parameters.AddWithValue("@subSessionId", subSessionId);
                command.Parameters.AddWithValue("@startTime", startTime);
                command.Parameters.AddWithValue("@endTime", endTime);*/

                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        public static void IncrementTotalFaces(NpgsqlConnection connection, int sessionId = -1, int noOfFaces = 1)
        {
            if (sessionId == -1) sessionId = MainForm.session;
            connection.Open();

            string query = $"UPDATE Session SET total_faces = total_faces + {noOfFaces} WHERE sessionid = '{sessionId}'";

            Console.WriteLine(query);

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {

                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        public static void UpdateSessionAccuracy(NpgsqlConnection connection, int sessionId, int sameFaces, int unidentifiedFaces, int misidentifiedFaces, int accuracy)
        {
            connection.Open();

            string query = $"UPDATE Session SET same_faces = '{sameFaces}', unidentified_faces = '{unidentifiedFaces}', misidentified_faces = '{misidentifiedFaces}', accuracy = '{accuracy}' WHERE sessionid = '{sessionId}'";

            Console.WriteLine(query);

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
         
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        public static void InsertBillDetails(NpgsqlConnection connection,bill_details bill, string table_name = "local_billing_db")
        {
            connection.Open();

            Console.WriteLine("INSERTING BILL DETAILS TO PGSQL!");

            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = connection;

            string phone_number = "null";
            string bill_no = bill.billNo;
            DateTime bill_date = bill.billDate;
            string bill_amount = bill.billAmt;
            string return_amount = bill.returnAmt;
            string quantity = bill.qty;
            string name = bill.name;
            if(bill.mobile != null) phone_number = bill.mobile;
            string selected_customer_id = null;
            string selected_visit_id = null;
            string customer_list = "{";
            string visit_list = "{";
            string temp="";
            int index = 0;

            if (bill.identified_customers!=null && bill.identified_customers.Count > 0)
            {
                foreach (redis_customer customer in bill.identified_customers)
                {

                    customer_list += customer.customer_id + ",";
                    visit_list += utilities.GetVisitDetails(customer.customer_id,ref temp).visit_id + ",";
                    index++;
                }
                customer_list = customer_list.Remove(customer_list.Length - 1, 1);
                visit_list = visit_list.Remove(visit_list.Length - 1, 1);
            }
            customer_list += "}";
            visit_list += "}";

            //Console.WriteLine(customer_list);



            if (bill.selected_customer != null) { selected_customer_id = bill.selected_customer.customer_id; }
            if(bill.visit_id != null) { selected_visit_id = bill.visit_id; }
            
            string query = $"INSERT INTO {table_name} (bill_no, bill_date, bill_amount, return_amount, quantity, name, phone_number, customer_id, visit_id, customer_list, visit_list) VALUES({bill_no}, TO_TIMESTAMP('{bill_date.ToString("yyyy-MM-dd HH:mm:ss")}', 'YYYY-MM-DD HH24:MI:SS'), {bill_amount}, {return_amount}, {quantity}, '{name}', {phone_number}, '{selected_customer_id}', '{selected_visit_id}', '{customer_list}', '{visit_list}')";
            Console.WriteLine("Executing Insert Query: " + query);
            command.CommandText = query;

            command.ExecuteNonQuery();

            connection.Close();
            
        }
    }
}
