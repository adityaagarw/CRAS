using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRAS
{
    public class redis_customer
    {
        public string key { get; set; }
        public string customer_id { get; set; }
        public string name { get; set; }
        public string phone_number { get; set; }
        public byte[] image { get; set; }
        public string return_customer { get; set; }
        public string last_visit { get; set; }
        public float average_time_spent { get; set; }
        public float average_purchase { get; set; }
        public float maximum_purchase { get; set; }
        public string remarks { get; set; }
        public int loyalty_level { get; set; }
        public int num_visits { get; set; }
        public string last_location { get; set; }
        public List<string> location_list { get; set; }
        public string category { get; set; }
        public DateTime entry_time { get; set; }

        public DateTime creation_date { get; set; }
        public redis_customer()
        { }

        public void print_record()
        {
            Console.WriteLine("********************************************");
            Console.WriteLine("Key: " + key);
            Console.WriteLine("Customer id: " + customer_id);
            Console.WriteLine("Name: " + name); 
            Console.WriteLine("Phone: " + phone_number);
            Console.WriteLine("Returning Customer: " + return_customer);
            Console.WriteLine("Last Visit: " + last_visit);
            Console.WriteLine("Average Time Spent: " + average_time_spent);
            Console.WriteLine("Average Purchase: " + average_purchase);
            Console.WriteLine("Macimum Purchase: " +  maximum_purchase);
            Console.WriteLine("Remarks: " + remarks);
            Console.WriteLine("Loyalty Level: " + loyalty_level);
            Console.WriteLine("Last Location: " + last_location);
            Console.WriteLine("Category: " + category);
            Console.WriteLine("Entry Time: " + entry_time);
            Console.WriteLine("********************************************");
        }

        public redis_customer(string customer_id, string name, string phone_number, byte[] image, string return_customer, string last_visit, float average_time_spent, float average_purchase, float maximum_purchase, string remarks, int loyalty_level, int num_visits, string last_location, List<string> location_list, string category, DateTime entry_time, DateTime creation_date)
        {
            this.customer_id = customer_id;
            this.name = name;
            this.phone_number = phone_number;
            this.image = image;
            this.return_customer = return_customer;
            this.last_visit = last_visit;
            this.average_time_spent = average_time_spent;
            this.average_purchase = average_purchase;
            this.maximum_purchase = maximum_purchase;
            this.remarks = remarks;
            this.loyalty_level = loyalty_level;
            this.num_visits = num_visits;
            this.last_location = last_location;
            this.location_list = location_list;
            this.category = category;
            this.entry_time = entry_time;
            this.creation_date = creation_date;
        }
    }
}
