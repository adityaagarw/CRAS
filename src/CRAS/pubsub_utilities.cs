using StackExchange.Redis;
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
            BindingList<redis_customer> customers;
            BindingList<visit_details> visits;

            visit_details visit = new visit_details();
            redis_customer customer = new redis_customer();

            customers = redis_utilities.GetCustomerDetails(MainForm.redisConnection, customer_id);
            visits = redis_utilities.GetVisitDetails(MainForm.redisConnection, customer_id);

            if (customers != null && visits != null)
            {
                if (customers.Count > 0 && visits.Count > 0)
                {
                    customer = customers[0];
                    visit = visits[0];
             
                    if (customer.customer_id != null && visit.visit_id != null)
                    {
                        Console.WriteLine("Current list size: " + MainForm.customer_list.Count);
                        MainForm.visits.Insert(0, visit);
                        MainForm.customer_list.Insert(0, customer);
                        Console.WriteLine("Inserted Customer: " + MainForm.customer_list[0].customer_id + " New list size: " + MainForm.customer_list.Count.ToString());
                    }
                }
            }
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

        public static void UpdateTempCustomer(string temp_customer_id, string customer_id, MainForm mainForm)
        {
            BindingList<redis_customer> temp_customers = redis_utilities.GetCustomerDetails(MainForm.redisConnection, temp_customer_id);
            redis_customer temp_customer = new redis_customer();

            BindingList<visit_details> temp_visits = redis_utilities.GetVisitDetails(MainForm.redisConnection, temp_customer_id);
            visit_details temp_visit = new visit_details();

            if (temp_customers != null && temp_customers.Count > 0) temp_customer = temp_customers[0];
            if(temp_visits != null && temp_visits.Count > 0) temp_visit = temp_visits[0];

            redis_customer customer = redis_utilities.GetCustomerDetails(MainForm.redisConnection,customer_id)[0];
            visit_details visit = redis_utilities.GetVisitDetails(MainForm.redisConnection, customer_id)[0];   

            int index = 0;

            if (mainForm != null)
            {
                if (temp_customer.customer_id != null && temp_visit.visit_id != null)
                {
                    index = MainForm.customer_list.IndexOf(temp_customer);

                    MainForm.customer_list[index] = customer;
                    MainForm.visits[index] = visit;

                    mainForm.Invoke(new Action(() => mainForm.customerFlowLayout.Controls.RemoveAt(index)));
                    mainForm.Invoke(new Action(() => mainForm.InsertCustomerInLayout(mainForm.customerFlowLayout, customer, index)));
                }

                else
                {
                    MainForm.customer_list.Insert(index, customer);
                    MainForm.visits.Insert(index, visit);
                }
                //mainForm.Invoke(new Action(() => mainForm.InsertCustomerInLayout(mainForm.customerFlowLayout, customer, index)));

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

        public static void AddEmployee(string customer_id, byte[] image, string name, string mobile)
        {
            
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
                    temp_customer_id = ids[1];
                    customer_id = ids[2];
                    Console.WriteLine("Existing Customer Entered! Customer Id:" + customer_id + " Temp id: " + temp_customer_id);
                    
                    UpdateTempCustomer(temp_customer_id, customer_id, mainForm);

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
                if (messageReceived.StartsWith("NewEmployeeAck"))
                {
                    AddEmployeeForm addEmployeeForm = MainForm.GetAddEmployeeFormIfOpen();
                    if (addEmployeeForm != null)
                    {
                        addEmployeeForm.Invoke(new Action(() => { addEmployeeForm.addEmployeeButton.Enabled = true; }));
                        addEmployeeForm.Invoke(new Action(() => { addEmployeeForm.ControlBox = true; }));
                        addEmployeeForm.Invoke(new Action(() => { MessageBox.Show("New Employee Added Successfully!"); }));
                    }
                }

                if(messageReceived.StartsWith("MarkAsEmployeeAck"))
                {
                    AddEmployeeForm addEmployeeForm = MainForm.GetAddEmployeeFormIfOpen();
                    if (addEmployeeForm != null)
                    {
                        addEmployeeForm.Invoke(new Action(() => { addEmployeeForm.addEmployeeButton.Enabled = true; }));
                        addEmployeeForm.Invoke(new Action(() => { addEmployeeForm.ControlBox = true; }));
                        addEmployeeForm.Invoke(new Action(() => { MessageBox.Show("Marked Existing person as Employee successfully!"); }));
                    }
                }
            });

        }
    }
}
