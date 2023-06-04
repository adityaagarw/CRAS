﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            ConnectionMultiplexer redisConnection;
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
                return null;
            }
            
            return redisConnection;
        }

        public static void UpdateRedisRecord(string key, string column_name, string new_value, ConnectionMultiplexer redisConnection)
        {
            if(redisConnection != null)
            {
                IServer redisServer = redisConnection.GetServer("127.0.0.1", 6379);

                IDatabase db = redisConnection.GetDatabase();

                var data = new HashEntry[] { new HashEntry(column_name, new_value) };

                db.HashSet(key, data);
            }
        }

        public static BindingList<redis_customer> ReadAllDataFromRedis(ConnectionMultiplexer redisConnection)
        {
            BindingList<redis_customer> customer_list = new BindingList<redis_customer>();

            if (redisConnection != null)
            {
                IServer redisServer = redisConnection.GetServer("127.0.0.1", 6379);

                foreach (var key in redisServer.Keys())
                {
                    var redisValueDict = redisConnection.GetDatabase().HashGetAll(key);
                    redis_customer customer = new redis_customer();

                    customer.key = key.ToString();
                    foreach (var entry in redisValueDict)
                    {
                        if (entry.Name.ToString().Equals("customer_id")) customer.customer_id = entry.Value.ToString();
                        if (entry.Name.ToString().Equals("name")) customer.name = entry.Value.ToString();
                        if (entry.Name.ToString().Equals("phone_number")) customer.phone_number = entry.Value.ToString();
                        if (entry.Name.ToString().Equals("image")) customer.image = entry.Value;
                        if (entry.Name.ToString().Equals("return_customer")) customer.return_customer = entry.Value.ToString();
                        if (entry.Name.ToString().Equals("last_visit")) customer.last_visit = entry.Value.ToString();
                        if (entry.Name.ToString().Equals("average_time_spent"))
                        {
                            float result;
                            float.TryParse(entry.Value.ToString(), out result);
                            customer.average_time_spent = result;
                        }
                        if (entry.Name.ToString().Equals("average_purchase"))
                        {
                            float result;
                            float.TryParse(entry.Value.ToString(), out result);
                            customer.average_purchase = result;
                        }
                        if (entry.Name.ToString().Equals("maximum_purchase"))
                        {
                            float result;
                            float.TryParse(entry.Value.ToString(), out result);
                            customer.maximum_purchase = result;
                        }
                        if (entry.Name.ToString().Equals("remarks")) customer.remarks = entry.Value.ToString();
                        if (entry.Name.ToString().Equals("loyalty_level"))
                        {
                            int result;
                            int.TryParse(entry.Value.ToString(), out result);
                            customer.loyalty_level = result;
                        }
                        if (entry.Name.ToString().Equals("num_visits"))
                        {
                            int result;
                            int.TryParse(entry.Value.ToString(), out result);
                            customer.num_visits = result;
                        }
                        if (entry.Name.ToString().Equals("last_location")) customer.last_location = entry.Value.ToString();
                        if (entry.Name.ToString().Equals("category")) customer.category = entry.Value.ToString();

                    }
                    customer_list.Add(customer);
                    customer.print_record();
                    //Console.WriteLine(customer_list);
                }
            }
            return customer_list;
        }
    }
}
