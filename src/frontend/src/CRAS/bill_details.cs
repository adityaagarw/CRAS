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

        public DateTime billDate { get; set; }
        public string mobile { get; set; }
        public string billAmt { get; set; }
        public string returnAmt { get; set; }
        public string qty { get; set; }
        
        public int billAmtInt { get; set; }
        public int returnAmtInt { get; set; }
        public int qtyInt { get; set; }
        public string remarks { get; set; }

        public redis_customer selected_customer { get; set; }
        public string visit_id { get; set; }
        public BindingList<redis_customer> identified_customers { get; set; }

        public bill_details()
        {
            identified_customers = new BindingList<redis_customer>();
            identified_customers.ListChanged += Identified_customers_ListChanged;
        }

        private void Identified_customers_ListChanged(object sender, ListChangedEventArgs e)
        {
            BillingForm billingForm = MainForm.GetBillingFormIfOpen();

            if (billingForm != null)
            {
                if (billingForm.current_bill.billNo == billNo)
                {
                    billingForm.InitializeCustomerFLP(this);
                }
            }
        }

        public void Print()
        {
            Console.WriteLine("Bill No:" + billNo);
            Console.WriteLine("Name: " + name);
            Console.WriteLine("Mobile: " + mobile);
            Console.WriteLine("Bill Amt (int): " + billAmtInt);
            Console.WriteLine("Return Amt (int): " + returnAmtInt);
            Console.WriteLine("Qty (int): " + qtyInt);
        }

        public void AddCustomer(redis_customer customer)
        {
            if (identified_customers == null)
            {
                identified_customers = new BindingList<redis_customer>();
            }
            identified_customers.Add(customer);
            
        }

    }
}
