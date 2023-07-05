using StackExchange.Redis;
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
        public DateTime last_visit { get; set; }
        public int average_time_spent { get; set; }
        public float average_bill_value { get; set; }
        public float average_bill_per_visit { get; set; }
        public float average_bill_per_billed_visit { get; set; }
        public float maximum_purchase { get; set; }
        public string remarks { get; set; }
        public string loyalty_level { get; set; }
        public int num_bills { get; set; }
        public int num_visits { get; set; }
       
        public int num_billed_visits { get; set; }
        public string last_location { get; set; }
        public List<string> location_list { get; set; }
        public string category { get; set; }
        
        public DateTime creation_date { get; set; }

        public int group_id { get; set; }

        // IN - MEM ONLY //
        public int incomplete { get; set; }
        public DateTime entry_time { get; set; }

        public int exited { get; set; }
        public DateTime visit_time { get; set; }

        public DateTime exit_time { get; set; }

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
            Console.WriteLine("Average Purchase: " + average_bill_value);
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
            //this.last_visit = last_visit;
            //this.average_time_spent = average_time_spent;
            this.average_bill_value= average_purchase;
            this.maximum_purchase = maximum_purchase;
            
            this.remarks = remarks;
            //this.loyalty_level = loyalty_level;
            this.num_visits = num_visits;
            this.last_location = last_location;
            this.location_list = location_list;
            this.category = category;
            this.entry_time = entry_time;
            this.creation_date = creation_date;

        }

        public void AddBillToCustomer(bill_details bill)
        {
            if (name == null) name = bill.name;
            if (phone_number == null) phone_number = bill.mobile;

            float result;

            float.TryParse(bill.billAmt, out result);
            float bill_amount = result;

            num_bills++;
            
            average_bill_value = (average_bill_value*num_bills + bill_amount)/(num_bills+1);
            average_bill_per_visit = (average_bill_per_visit * num_visits + bill_amount)/(num_visits + 1);
            
        }

        public void RemoveBillFromCustomer(bill_details bill)
        {
            float result;
            
            float.TryParse(bill.billAmt, out result);
            float bill_amount = result;

            num_bills--;

            if (num_bills > 0) average_bill_value = (average_bill_value * num_bills - bill_amount) / (num_bills);
            else average_bill_value = 0;

            if (num_visits > 0) average_bill_per_visit = (average_bill_per_visit * num_visits - bill_amount) / (num_visits);
            else average_bill_per_visit = 0;
        }

        public HashEntry[] getHashEntry()
        {

            Dictionary<string, string> modifiedFields = new Dictionary<string, string>();
            modifiedFields.Add("name", name);
            modifiedFields.Add("phone_number", phone_number);
            modifiedFields.Add("average_bill_value", average_bill_value.ToString());
            modifiedFields.Add("average_bill_per_visit", average_bill_per_visit.ToString());
            modifiedFields.Add("average_bill_per_billed_visit", average_bill_per_billed_visit.ToString());
            modifiedFields.Add("num_bills", num_bills.ToString());
            modifiedFields.Add("num_visits", num_visits.ToString());
            modifiedFields.Add("num_billed_visits", num_billed_visits.ToString());

            var data = new HashEntry[modifiedFields.Count];

            int index = 0;

            foreach (var value in modifiedFields)
            {
                data[index] = new HashEntry(value.Key, value.Value);
                index++;
            }

            return data;
        }
    }
}
