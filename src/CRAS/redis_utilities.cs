using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                    MessageBox.Show($"Failed to connect to Redis server: {ex.Message}");
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
        
        
        public static BindingList<redis_customer> ReadAllDataFromRedis(ConnectionMultiplexer redisConnection, string customer_id = "*")
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
                        customer.last_visit = db.HashGet(hashKey, "last_visit").ToString();
                        
                        float.TryParse(db.HashGet(hashKey, "average_time_spent").ToString(), out float result);
                        customer.average_time_spent = result; result = 0;

                        float.TryParse(db.HashGet(hashKey, "average_purchase").ToString(), out result);
                        customer.average_purchase = result; result = 0;

                        float.TryParse(db.HashGet(hashKey, "maximum_purchase").ToString(), out result);
                        customer.maximum_purchase = result; result = 0;

                        customer.remarks = db.HashGet(hashKey, "remarks").ToString();

                        int.TryParse(db.HashGet(hashKey, "loyalty_level").ToString(), out int result_int);
                        customer.loyalty_level = result_int; result_int = 0;

                        int.TryParse(db.HashGet(hashKey, "num_visits").ToString(), out result_int);
                        customer.num_visits = result_int; result_int = 0;

                        customer.last_location = db.HashGet(hashKey, "last_location").ToString();
                        customer.location_list = db.HashGet(hashKey, "location_list").ToString().Split(',').ToList();
                        customer.category = db.HashGet(hashKey, "category").ToString();
                        
                        DateTime.TryParse(db.HashGet(hashKey, "entry_time").ToString(), out DateTime result_entryTime);
                        customer.entry_time = result_entryTime;

                        DateTime.TryParse(db.HashGet(hashKey, "creation_date").ToString(), out DateTime result_creationDate);
                        customer.creation_date = result_creationDate;

                    //customer.print_record();
                    customer_list.Add(customer);
                    
                }
            }
            return customer_list;
        }
    }
}
