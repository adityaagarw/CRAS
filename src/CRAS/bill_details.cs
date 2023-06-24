using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRAS
{
    public class bill_details
    {
        public string billNo { get; set; }
        public string name { get; set; }
        public string mobile { get; set; }
        public string billAmt { get; set; }
        public string returnAmt { get; set; }
        public string qty { get; set; }
        public string remarks { get; set; }

        public BindingList<redis_customer> identified_customers { get; set; }

        public void Print()
        {
            Console.WriteLine("Bill No:" + billNo);
            Console.WriteLine("Name: " + name);
            Console.WriteLine("Mobile: " + mobile);
            Console.WriteLine("Bill Amt: " + billAmt);
            Console.WriteLine("Return Amt: " + returnAmt);
            Console.WriteLine("Qty: " + qty);
        }

        public void AddCustomer(redis_customer customer)
        {
            if(identified_customers == null) identified_customers = new BindingList<redis_customer>();
            identified_customers.Add(customer);
        }

    }
}
