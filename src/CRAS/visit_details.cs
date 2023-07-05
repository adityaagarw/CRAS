using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRAS
{
    public class visit_details
    {
        public string key {  get; set; }
        public string customer_id { get; set; }
        public string visit_id { get; set; }

        public string store_id { get; set; }
        public DateTime entry_time { get; set; }
        public DateTime exit_time { get; set; }
        public int billed { get; set; }
        public string bill_no { get; set; }
        public DateTime bill_date { get; set; }
        public int bill_amount { get; set; }
        public int return_amount { get; set; }
        public int time_spent { get; set; }
        public string visit_remark { get; set; }

        public int customer_rating { get; set; }
        public int customer_feedback { get; set; }
        public int incomplete { get; set; }

        public void RemoveBillFromVisit(bill_details bill)
        {
            billed = 0;
            bill_no = "";
            bill_date = DateTime.MinValue;
            bill_amount = 0;
            return_amount = 0;
            if (bill.remarks != null) visit_remark = bill.remarks;
            else visit_remark = "";
            customer_rating = 0;
            customer_feedback = 0;

        }
        public void AddBillToVisit(bill_details bill, int rating = 0, int feedback = 0)
        {
            billed = 1;
            bill_no = bill.billNo;
            bill_date = bill.billDate;
            bill_amount = bill.billAmtInt;
            return_amount = bill.returnAmtInt;
            if (bill.remarks != null) visit_remark = bill.remarks;
            else visit_remark = "";
            customer_rating = rating;
            customer_feedback = feedback;

        }

        public void Print()
        {
            Console.WriteLine("Visit Details: ");
            Console.WriteLine("customer_id: " + customer_id);
            Console.WriteLine("visit_id: " + visit_id);
            Console.WriteLine("store_id: " + store_id);
            Console.WriteLine("entry_time: " + entry_time);
            Console.WriteLine("exit_time: " + exit_time);
            Console.WriteLine("billed: " + billed);
            Console.WriteLine("bill_no: " + bill_no);
            Console.WriteLine("billAmt: " + bill_amount);
            Console.WriteLine("time_spent: " + time_spent);
            Console.WriteLine("visit_remark: " + visit_remark);
            Console.WriteLine("customer_rating: " + customer_rating);
            Console.WriteLine("customer_feedback: " + customer_feedback);
            Console.WriteLine("incomplete: " + incomplete);
        }

        public HashEntry[] getHashEntry()
        {
            HashEntry[] hashEntries = new HashEntry[15];

            hashEntries[0] = new HashEntry("customer_id", customer_id);
            hashEntries[1] = new HashEntry("visit_id", visit_id);
            hashEntries[2] = new HashEntry("store_id", store_id);
            hashEntries[3] = new HashEntry("entry_time", entry_time.ToString("yyyy-MM-dd HH:mm:ss"));
            hashEntries[4] = new HashEntry("exit_time", exit_time.ToString("yyyy-MM-dd HH:mm:ss"));
            hashEntries[5] = new HashEntry("billed", billed);
            hashEntries[6] = new HashEntry("bill_no", bill_no);
            hashEntries[7] = new HashEntry("bill_date", bill_date.ToString("yyyy-MM-dd HH:mm:ss"));
            hashEntries[8] = new HashEntry("bill_amount", bill_amount);
            hashEntries[9] = new HashEntry("return_amount", return_amount);
            hashEntries[10] = new HashEntry("time_spent", time_spent.ToString());
            hashEntries[11] = new HashEntry("visit_remark", visit_remark);
            hashEntries[12] = new HashEntry("customer_rating", customer_rating);
            hashEntries[13] = new HashEntry("customer_feedback", customer_feedback);
            hashEntries[14] = new HashEntry("incomplete", incomplete);
            


            return hashEntries;
        }
    }
}
