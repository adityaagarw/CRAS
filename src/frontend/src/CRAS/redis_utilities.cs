using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StackExchange.Redis;
namespace CRAS
{
    internal class redis_utilities
    {
        public static ConnectionMultiplexer ConnectToRedis(string host = "127.0.0.1", string port = "6379")
        {
            ConnectionMultiplexer redisConnection = null;
            int attempt = 1;
            while (redisConnection == null || !redisConnection.IsConnected)
            {
                if(MainForm.loadingForm!= null) MainForm.loadingForm.Invoke(new Action(() => { MainForm.loadingForm.SetLoadingLabel("Connecting to Redis! Attempt: " + attempt.ToString()); }));
                try
                {
                    ConfigurationOptions options = new ConfigurationOptions
                    {
                        EndPoints = { host },

                    };
                    redisConnection = ConnectionMultiplexer.Connect(options); // Replace "localhost" with your Redis server IP or hostname if needed

                }
                catch (Exception ex)
                {
                    attempt++;
                    if(MainForm.loadingForm != null) MainForm.loadingForm.Invoke(new Action(() => { MainForm.loadingForm.SetLoadingLabel("Redis connection failed! Retrying!"); })) ;
                    //MessageBox.Show($"Failed to connect to Redis server: {ex.Message}");
                }
            } 

            return redisConnection;
        }

        public static void UpdateRedisRecord(string key, Dictionary<string, string> values, ConnectionMultiplexer redisConnection)
        {
            if(redisConnection != null)
            {
                IServer redisServer = redisConnection.GetServer("127.0.0.1", 6379);

                IDatabase db = redisConnection.GetDatabase();

                var data = new HashEntry[values.Count];

                int index = 0;

                foreach(var value in values)
                {
                    data[index] = new HashEntry(value.Key, value.Value);
                    index++;
                }

                db.HashSet(key, data);
            }
        }

        public static bool DeleteRedisEntry(ConnectionMultiplexer redisConnection, string redisKey)
        {
            IDatabase db = redisConnection.GetDatabase();

            db.KeyDelete(redisKey);

            return true;
        }

        public static BindingList<redis_customer> GetCustomerDetails(ConnectionMultiplexer redisConnection, string customer_id = "*")
        {
            BindingList<redis_customer> customer_list = new BindingList<redis_customer>();

            if (redisConnection != null)
            {
                IServer redisServer = redisConnection.GetServer("127.0.0.1", 6379);
                IDatabase db = redisConnection.GetDatabase();
                foreach (var hashKey in redisServer.Keys(pattern: "customer_inmem_db:" + customer_id))
                {
                    redis_customer customer = new redis_customer();

                    customer.key = hashKey.ToString();
                    

                    customer.customer_id = db.HashGet(hashKey, "customer_id").ToString();
    
                    customer.name = db.HashGet(hashKey, "name").ToString();
                    customer.phone_number = db.HashGet(hashKey, "phone_number").ToString();
                    customer.image = db.HashGet(hashKey, "image");
                    customer.return_customer = db.HashGet(hashKey, "return_customer").ToString();
                    
                    DateTime.TryParse(db.HashGet(hashKey, "last_visit").ToString(), out DateTime result_lastVisit);
                    customer.last_visit = result_lastVisit;

                    int.TryParse(db.HashGet(hashKey, "average_time_spent").ToString(), out int resultInt);
                    customer.average_time_spent = resultInt; resultInt = 0;

                    float.TryParse(db.HashGet(hashKey, "average_bill_value").ToString(), out float result);
                    customer.average_bill_value = result; result = 0;

                    float.TryParse(db.HashGet(hashKey, "average_bill_per_visit").ToString(), out result);
                    customer.average_bill_per_visit = result; result = 0; 
                    
                    float.TryParse(db.HashGet(hashKey, "average_bill_per_billed_visit").ToString(), out result);
                    customer.average_bill_per_billed_visit = result; result = 0;

                    float.TryParse(db.HashGet(hashKey, "maximum_purchase").ToString(), out result);
                    customer.maximum_purchase = result; result = 0;

                    customer.remarks = db.HashGet(hashKey, "remarks").ToString();

                    customer.loyalty_level = db.HashGet(hashKey, "loyalty_level").ToString();

                    int.TryParse(db.HashGet(hashKey, "num_bills").ToString(), out int result_int);
                    customer.num_bills = result_int; result_int = 0;

                    int.TryParse(db.HashGet(hashKey, "num_visits").ToString(), out result_int);
                    customer.num_visits = result_int; result_int = 0;
                    
                    int.TryParse(db.HashGet(hashKey, "num_billed_visits").ToString(), out result_int);
                    customer.num_billed_visits = result_int; result_int = 0;

                    customer.last_location = db.HashGet(hashKey, "last_location").ToString();
                    customer.location_list = db.HashGet(hashKey, "location_list").ToString().Split(',').ToList();
                    customer.category = db.HashGet(hashKey, "category").ToString();
                        
                    DateTime.TryParse(db.HashGet(hashKey, "entry_time").ToString(), out DateTime result_entryTime);
                    customer.entry_time = result_entryTime;

                    DateTime.TryParse(db.HashGet(hashKey, "creation_date").ToString(), out DateTime result_creationDate);
                    customer.creation_date = result_creationDate;

                    int.TryParse(db.HashGet(hashKey, "group_id").ToString(), out result_int);
                    customer.group_id = result_int; result_int = 0;

                    int.TryParse(db.HashGet(hashKey, "incomplete").ToString(), out result_int);
                    customer.incomplete = result_int; result_int = 0;

                    int.TryParse(db.HashGet(hashKey, "exited").ToString(), out result_int);
                    customer.exited = result_int; result_int = 0;

                    DateTime.TryParse(db.HashGet(hashKey, "visit_time").ToString(), out DateTime result_visitTime);
                    customer.visit_time = result_visitTime;

                    DateTime.TryParse(db.HashGet(hashKey, "exit_time").ToString(), out DateTime result_exitTime);
                    customer.exit_time = result_exitTime;


                    //customer.print_record();
                    customer_list.Add(customer);
                    
                }
            }
            return customer_list;
        }

        public static Dictionary<string, HashEntry[]> GetAllRedisData(ConnectionMultiplexer redisConnection)
        {
            Dictionary<string, HashEntry[]> hashes = new Dictionary<string, HashEntry[]>();
            if (redisConnection != null)
            {
                visit_details visit = new visit_details();
                IServer redisServer = redisConnection.GetServer("127.0.0.1", 6379);
                IDatabase db = redisConnection.GetDatabase();
                foreach (var hashKey in redisServer.Keys(pattern: "*"))
                {
                    //Da
                    //Console.WriteLine(hashKey);
                    HashEntry[] hash = db.HashGetAll(hashKey);
                    
                    hashes.Add(hashKey, hash);
                    //foreach (HashEntry value in values) { if (!(value.Name.Equals("encoding") || value.Name.Equals("image"))) Console.WriteLine(value.Name + ": " + value.Value.ToString()); }
                }
            }
            return hashes;
        }

        public static BindingList<visit_details> GetVisitDetails(ConnectionMultiplexer redisConnection, string customer_id = "*")
        {
            BindingList<visit_details> visits = new BindingList<visit_details>();

            if (redisConnection != null)
            {
                visit_details visit = new visit_details();
                IServer redisServer = redisConnection.GetServer("127.0.0.1", 6379);
                IDatabase db = redisConnection.GetDatabase();
                foreach (var hashKey in redisServer.Keys(pattern: "visit_inmem_db:" + customer_id))
                {
                    visit.key = hashKey.ToString();

                    visit.store_id = db.HashGet(hashKey, "store_id").ToString();
                    visit.customer_id = db.HashGet(hashKey, "customer_id").ToString();
                    visit.visit_id = db.HashGet(hashKey, "visit_id").ToString();

                    DateTime.TryParse(db.HashGet(hashKey, "entry_time").ToString(), out DateTime result_entryTime);
                    visit.entry_time = result_entryTime;
                    DateTime.TryParse(db.HashGet(hashKey, "exit_time").ToString(), out DateTime result_exitTime);
                    visit.exit_time = result_exitTime;

                    int.TryParse(db.HashGet(hashKey, "billed").ToString(), out int result_int);
                    visit.billed = result_int; result_int = 0;

                    visit.bill_no = db.HashGet(hashKey, "bill_no").ToString();
                    
                    DateTime.TryParse(db.HashGet(hashKey, "bill_date").ToString(), out DateTime result_billDate);
                    visit.bill_date = result_billDate;

                    int.TryParse(db.HashGet(hashKey, "bill_amount").ToString(), out result_int);
                    visit.bill_amount = result_int; result_int = 0;

                    int.TryParse(db.HashGet(hashKey, "return_amount").ToString(), out result_int);
                    visit.return_amount = result_int; result_int = 0;

                    int.TryParse(db.HashGet(hashKey, "time_spent").ToString(), out int result_timespent);
                    visit.time_spent = result_timespent;

                    visit.visit_remark = db.HashGet(hashKey, "visit_remark").ToString();

                    int.TryParse(db.HashGet(hashKey, "customer_rating").ToString(), out result_int);
                    visit.customer_rating = result_int; result_int = 0;

                    int.TryParse(db.HashGet(hashKey, "customer_feedback").ToString(), out result_int);
                    visit.customer_feedback = result_int; result_int = 0;

                    int.TryParse(db.HashGet(hashKey, "incomplete").ToString(), out result_int);
                    visit.incomplete = result_int; result_int = 0;

                    //visit.Print();
                    visits.Add(visit);

                }
            }
            return visits;
        }

        public static void UpdateVisitDetails(ConnectionMultiplexer redisConnection, visit_details visit)
        {
            if (redisConnection != null)
            {
                visit.Print();

                IServer redisServer = redisConnection.GetServer("127.0.0.1", 6379);

                IDatabase db = redisConnection.GetDatabase();

                string key = visit.key;

                HashEntry[] visitData = visit.getHashEntry();

                db.HashSet(key, visitData);

                Console.WriteLine("Updated visit in Redis");
                
            }
        }

        public static void UpdateCustomerDetails(ConnectionMultiplexer redisConnection, redis_customer customer)
        {
            if (redisConnection != null)
            {
                //customer.Print();

                IServer redisServer = redisConnection.GetServer("127.0.0.1", 6379);

                IDatabase db = redisConnection.GetDatabase();

                string key = customer.key;

                HashEntry[] customerData = customer.getHashEntry();

                db.HashSet(key, customerData);

                Console.WriteLine("Updated Customer in Redis");

            }
        }
    }
}
