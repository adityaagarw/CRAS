﻿using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRAS
{
    internal class pubsub_utilities
    {
        public static bool billing_subscribed = false;

        public static void NewCustomerIdentified(string customer_id)
        {
            redis_customer customer;
            visit_details visit;

            customer = redis_utilities.GetCustomerDetails(MainForm.redisConnection, customer_id)[0];
            visit = redis_utilities.GetVisitDetails(MainForm.redisConnection, customer_id)[0];

            MainForm.visits.Insert(0, visit);
            MainForm.customer_list.Insert(0, customer);
            
        }

        public static redis_customer GetCustomerFromInMem(string customer_id)
        {
            redis_customer customer;

            foreach (redis_customer x in MainForm.customer_list)
            {
                if (x.customer_id == customer_id)
                {
                    //BillingForm.identified_customers.Add(x); ;
                    //Console.WriteLine("Billing Customer Found with Customer id: " + x.customer_id + "Entry time: " + x.entry_time);
                    customer = x;
                    return customer;
                }
            }
            return null;
        }

        public static void BillingCustomerIdentified(string customer_id)
        {
            redis_customer customer;
            customer = GetCustomerFromInMem(customer_id);
            if (customer != null && MainForm.bills.Count > 0)
            {
                MainForm.bills[MainForm.bills.Count - 1].AddCustomer(customer);
                //BillingForm.identified_customers.Add(customer);
            }

        }
        public static void ReScanCustomerIdentified(string customer_id)
        {
            redis_customer customer;
            customer = GetCustomerFromInMem(customer_id);
            if (customer != null && MainForm.bills.Count > 0)
            {
                //MainForm.bills[MainForm.bills.Count - 1].AddCustomer(customer);
                MainForm.bills[BillingForm.current_bill_index].AddCustomer(customer);
                //BillingForm.identified_customers.Add(customer);
            }

        }

        public static void CustomerExited(string customer_id, MainForm mainForm = null)
        {
            redis_customer customer;
            customer = GetCustomerFromInMem(customer_id);
            MainForm.exited_customers.Add(customer);
            int index = MainForm.customer_list.IndexOf(customer);
            MainForm.customer_list.Remove(customer);
            if (mainForm != null) mainForm.Invoke(new Action(() => mainForm.customerFlowLayout.Controls.RemoveAt(index)));
           
            
        }

        public static void PublishMessage(string channelName, string message)
        {
            ISubscriber publishChannel = MainForm.redisConnection.GetSubscriber();
            publishChannel.Publish(channelName, message);
            Console.WriteLine("Published " + message + " to " + channelName);

        }

        public static void InitialiseSubscribers(string channelName, MainForm mainForm = null)
        {
            ISubscriber cameraSub = MainForm.redisConnection.GetSubscriber();

            cameraSub.Subscribe(channelName, (channel, message) =>
            {
                string messageReceived = (string)message;
                string customer_id;
                string temp_customer_id;

                if (messageReceived.StartsWith("NewCustomer"))
                {
                    customer_id = messageReceived.Split(':')[1];
                    Console.WriteLine("New Customer Entered! Customer Id:" + customer_id);
                    NewCustomerIdentified(customer_id);
                    
                }
                if (messageReceived.StartsWith("UpdateCustomer"))
                {
                    var ids = messageReceived.Split(':', ',');
                    customer_id = ids[1];
                    temp_customer_id = ids[3];
                    Console.WriteLine("Existing Customer Entered! Customer Id:" + customer_id + " Temp id: " + temp_customer_id);

                }
                if (messageReceived.StartsWith("BillingCustomer"))
                {
                    customer_id = messageReceived.Split(':')[1];
                    Console.WriteLine("Billing Customer Identified:" + customer_id);
                    BillingCustomerIdentified(customer_id);
                }

                if (messageReceived.StartsWith("RescanCustomer"))
                {
                    customer_id = messageReceived.Split(':')[1];
                    Console.WriteLine("Rescan Customer Identified:" + customer_id);
                    ReScanCustomerIdentified(customer_id);
                }

                if (messageReceived.StartsWith("EndBilling"))
                {
                    Console.WriteLine("Billing Stream Ended!");
                    MainForm.bill_scanning = 0;
                    mainForm.Invoke(new Action(() => { mainForm.scanStatusLabel.Text = "Scan Completed"; }));
                    pgsql_utilities.InsertBillDetails(MainForm.pgsql_connection, MainForm.bills[MainForm.bills.Count -1]);
                    //In case Bill Exists, then update instead of Insert
                    //billingForm.scanStatus.ForeColor = Color.Gray;
                }
                if (messageReceived.StartsWith("EndRescan"))
                {
                    Console.WriteLine("Rescan Stream Ended!");
                    MainForm.bill_scanning = 0;
                    BillingForm billingForm = MainForm.GetBillingFormIfOpen();
                if (billingForm != null)
                {
                    billingForm.Invoke(new Action(() => { billingForm.scanStatus.Text = "Scan Completed"; }));
                    pgsql_utilities.UpdateBillDetails(MainForm.pgsql_connection, billingForm.current_bill);
                    billingForm.Invoke(new Action(() => {billingForm.FormBorderStyle = FormBorderStyle.FixedToolWindow; }));

                    }
                    if (mainForm != null) mainForm.Invoke(new Action(() => { mainForm.scanStatusLabel.Text = "Scan Completed"; }));
                    //In case Bill Exists, then update instead of Insert
                    //billingForm.scanStatus.ForeColor = Color.Gray;
                }
                if (messageReceived.StartsWith("DeleteCustomer"))
                {
                    customer_id = messageReceived.Split(':')[1];
                    Console.WriteLine("Customer Exited: " + customer_id);
                    CustomerExited(customer_id, mainForm);
                }
                if (messageReceived.StartsWith("Employee"))
                {

                }
            });

        }
    }
}
