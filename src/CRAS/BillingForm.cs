using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRAS
{
    public partial class BillingForm : Form
    {
        public static ConnectionMultiplexer redisConnection = MainForm.redisConnection;
        public int total_bills = 0;
        public bill_details current_bill = null;
        public static int current_bill_index = 0;
        redis_customer selected_customer_temp = new redis_customer();
        public int user_rating = 0;
        public int user_feedback = 0;

        public BillingForm(bill_details currentBill = null)
        {
            InitializeComponent();
            InitializeFLP(userRatingFLP);
            InitializeFLP(expRatingFLP);
            /*if (!pubsub_utilities.billing_subscribed)
            {
                pubsub_utilities.InitialiseSubscribers("Billing", this);
                pubsub_utilities.billing_subscribed = true;
            }
            identified_customers.ListChanged += Identified_customers_ListChanged;
            if(currentBill != null) identified_customers = currentBill.identified_customers;
            for(int i = 0; i< identified_customers.Count; i++) { PopulateCustomerUC(i) ; }
            */
        }


        private void Identified_customers_ListChanged(object sender, ListChangedEventArgs e)
        {
            /*int new_records = identified_customers.Count - identified_customers_count;

            //Console.WriteLine("Found " + new_records + " new records!!");

            for (int i = identified_customers_count; i < identified_customers_count + new_records; i++)
            {
                InsertIdentifiedCustomer(identified_customers[i]);
            }
            identified_customers_count += new_records;
            //throw new NotImplementedException();
            */
        }
        
    

        public void InsertIdentifiedCustomer(redis_customer customer)
        {
            
            //Console.WriteLine("Adding Customer Id: " + customer.customer_id + " to Flow!!!!!!");
            PictureBox pictureBox = new PictureBox();
            pictureBox.Name = identifiedFacesFLP.Controls.Count.ToString();
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.Size = new System.Drawing.Size(120, 120);
            pictureBox.Image = utilities.BytetoImage(customer.image);
            pictureBox.Click += PictureBox_Click;
            if (this.IsHandleCreated)
            {
                identifiedFacesFLP.Invoke(new Action(() => { identifiedFacesFLP.Controls.Add(pictureBox); }));
            }
            
        }

        private void ClearCustomerUC()
        {
            selectedCustomerUC.SetImage(null);
            selectedCustomerUC.SetLabel("label1", "", "");
            selectedCustomerUC.SetLabel("label2", "", "");
            selectedCustomerUC.SetLabel("label3", "", "");
            selectedCustomerUC.SetLabel("label4", "", "");
            selectedCustomerUC.SetLabel("label5", "", "");
            selectedCustomerUC.SetLabel("label6", "",  "");

            selected_customer_temp = null;
            selectedCustomerUC.SetState("Please select a Customer!");
        }
        private void PopulateCustomerUC(redis_customer customer)
        {
            selectedCustomerUC.SetImage(customer.image);
            selectedCustomerUC.SetLabel("label1", "Name: ", customer.name);
            selectedCustomerUC.SetLabel("label2", "Phone: ", customer.phone_number);
            selectedCustomerUC.SetLabel("label3", "Returning: ", customer.return_customer);
            selectedCustomerUC.SetLabel("label4", "Category: ", customer.category);
            selectedCustomerUC.SetLabel("label5", "Last Visit: ", customer.last_visit.ToString("dd-MM-yyyy HH:mm:ss"));
            selectedCustomerUC.SetLabel("label6", "Entry Time: ", customer.entry_time.ToShortTimeString());

            selected_customer_temp = customer;

            selectedCustomerUC.SetState("Customer Selected!");
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            PictureBox selectedPictureBox = (PictureBox)sender;
            selectedPictureBox.Select();
            
            foreach(Control control in identifiedFacesFLP.Controls)
            {
                ((PictureBox)control).BorderStyle = BorderStyle.None;
                
            }
            int index = int.Parse(selectedPictureBox.Name);
            redis_customer customer = current_bill.identified_customers[index];

            //MainForm.bills[current_bill_index].selected_customer = customer;
            selected_customer_temp = customer;
            selectedPictureBox.BorderStyle = BorderStyle.Fixed3D;

            if (selected_customer_temp == current_bill.selected_customer) updateButton.Enabled = false;
            else updateButton.Enabled = true;
           
            PopulateCustomerUC(customer);

            //throw new NotImplementedException();*/
        }

        private void InitializeFLP(FlowLayoutPanel panel)
        {
            if (panel.Name.Equals("userRatingFLP")) user_rating = 0;
            if (panel.Name.Equals("expRatingFLP")) user_feedback = 0;
            panel.AutoScroll = true;
            panel.FlowDirection = FlowDirection.LeftToRight;

            for(int i = 0; i < 5; i++)
            {
                StarUC starUC = new StarUC();
                starUC.StarSelected += StarUC_StarSelected;

                panel.Controls.Add(starUC);
            }
        }

        private void StarUC_StarSelected(object sender, int index)
        {
            StarUC selectedStar = (StarUC)sender;
            selectedStar.SelectStars((FlowLayoutPanel)selectedStar.Parent, index);
            if (selectedStar.Parent.Name.Equals("userRatingFLP")) user_rating = index + 1;

            if (selectedStar.Parent.Name.Equals("expRatingFLP")) user_feedback = index + 1;
            //MessageBox.Show("User Rating: " + (index + 1).ToString());
            //throw new NotImplementedException();
        }

        public void InitializeCustomerFLP(bill_details bill)
        {
            identifiedFacesFLP.Invoke(new Action(() => { identifiedFacesFLP.Controls.Clear(); }));
            
            selected_customer_temp = new redis_customer();
            if (bill.identified_customers != null)
            {
                foreach (redis_customer customer in bill.identified_customers)
                {
                    InsertIdentifiedCustomer(customer);
                }
            }

           
            //identified_customers.Clear();
            //identified_customers_count = identified_customers.Count;
        }

        public void StartScanning()
        {
            //Thread.Sleep(500);
            pubsub_utilities.PublishMessage("Frontend", "StartRescan");
            scanStatus.Text = "Scanning";
            scanStatus.ForeColor = Color.DarkGreen;
            MainForm mainForm = (MainForm)Owner;
            mainForm.scanStatusLabel.Text = "Scanning";
        }
        private void BillingForm_Load(object sender, EventArgs e)
        {
            //StartScanning();
            previousButton.Enabled = false;
            nextButton.Enabled = false;
            ClearCustomerUC();

            if (MainForm.bills.Count == 0) updateButton.Enabled = false;

            total_bills = MainForm.bills.Count;
            if(MainForm.bill_scanning == 1)
            {
                rescanButton.Enabled = false;
                rescanButton.BackColor = Color.DarkGray;
                scanStatus.Text = "Scanning";
                scanStatus.ForeColor = Color.DarkGreen;
            }
            if (total_bills > 0)
            {
                current_bill_index = total_bills - 1;
                current_bill = MainForm.bills[current_bill_index];
                InitializeCustomerFLP(current_bill);
                PopulateBillDetails(current_bill);
                if (current_bill.selected_customer != null && current_bill.selected_customer.customer_id.Length > 0) PopulateCustomerUC(current_bill.selected_customer);
            }
            if(total_bills > 1)
            {
                previousButton.Enabled = true;
            }
        }

        public void PopulateBillDetails(bill_details bill)
        {
            billNoLabel.Text = bill.billNo;
            billNameLabel.Text = bill.name;
            billMobileLabel.Text = bill.mobile;
            billAmtLabel.Text = bill.billAmt.ToString();
            returnAmtLabel.Text = bill.returnAmt.ToString();

            if(bill.returnAmt.Equals("0"))
            {
                returnAmt.Visible = false;
                returnAmtLabel.Visible = false;
            }
            
        }

        private void rescanButton_Click(object sender, EventArgs e)
        {
            if (current_bill != null)
            {
                //current_bill.identified_customers = null;
                current_bill.identified_customers.Clear();

                InitializeCustomerFLP(current_bill);
                StartScanning();
                FormBorderStyle = FormBorderStyle.None;
            }
            else { MessageBox.Show("Please Add bill to begin scanning!"); }
        }

        private void BillingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Owner.Show();
        }

        private void scanStatus_TextChanged(object sender, EventArgs e)
        {
            /*if(rescanButton.Enabled == false)
            {
                rescanButton.Enabled = true;
                rescanButton.BackColor = Color.White;
            }

            else if(rescanButton.Enabled == true)
            {
                rescanButton.Enabled = false;
                rescanButton.BackColor = Color.Gray;
            }*/
        }

        public void SetNavigationButtons()
        {
            if (current_bill_index == 0) previousButton.Enabled = false;
            if (current_bill_index < total_bills - 1) nextButton.Enabled = true;
            if (current_bill_index > 0) previousButton.Enabled = true;
            if (current_bill_index == total_bills - 1) nextButton.Enabled = false;

        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            current_bill_index--;
            ClearCustomerUC();

            SetNavigationButtons();
            current_bill = MainForm.bills[current_bill_index];
            InitializeCustomerFLP(current_bill);
            PopulateBillDetails(current_bill);

            if (identifiedFacesFLP.Controls.Count > 0 && current_bill.selected_customer != null) PopulateCustomerUC(current_bill.selected_customer);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            current_bill_index++;
            ClearCustomerUC();

            SetNavigationButtons();
            current_bill = MainForm.bills[current_bill_index];
            InitializeCustomerFLP(current_bill);
            PopulateBillDetails(current_bill);

            if (identifiedFacesFLP.Controls.Count > 0 && current_bill.selected_customer!= null) PopulateCustomerUC(current_bill.selected_customer);
             
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            bool visit_exists_in_mem;
            bool bill_exists_in_mem;
            bool customer_selected;

            //CUSTOMER HAS NOT BEEN SELECTED PREVIOUSLY FOR THIS BILL
            if(current_bill.selected_customer == null)
            {
                if (selected_customer_temp != null)
                {
                    visit_details visit = new visit_details();
                    redis_customer customer = new redis_customer();
                    string visit_source = "";
                    string customer_source = "";

                    visit = utilities.GetVisitDetails(selected_customer_temp.customer_id, ref visit_source);

                    if (visit_source.Length > 0)
                    {
                        visit.customer_id = selected_customer_temp.customer_id;
                        current_bill.selected_customer = selected_customer_temp;

                        visit.AddBillToVisit(current_bill, user_rating, user_feedback);
                          
                        //UPDATE SELECTED_CUSTOMER_TEMP TO VISIT
                        if (visit_source == "memory")
                        {
                            redis_utilities.UpdateVisitDetails(redisConnection, visit);
                        }
                        else if (visit_source == "local")
                        {
                            pgsql_utilities.UpdateVisitDetails(MainForm.pgsql_connection, visit);
                        }
                    }
                    else Console.WriteLine("Visit not found!");

                    //Customer details
                    customer = utilities.GetCustomerDetails(selected_customer_temp.customer_id, ref customer_source);
                    if (customer_source.Length > 0)
                    {
                        customer.AddBillToCustomer(current_bill);

                        if (customer_source == "memory")
                        {
                            redis_utilities.UpdateCustomerDetails(redisConnection, customer);
                        }
                        else if (customer_source == "local")
                        {
                            pgsql_utilities.UpdateCustomerDetails(MainForm.pgsql_connection, customer);
                        }
                    }
                    else Console.WriteLine("Customer Not Found");
                }

            }

        
            //CUSTOMER HAD BEEN SELECTED ALREADY. UPDATE IT IN CASE NEW CUSTOMER SELECTED
            else
            {
                visit_details visit_old = new visit_details();
                visit_details visit_new = new visit_details();
                string visit_source_old = "";
                string visit_source_new = "";

                visit_old = utilities.GetVisitDetails(current_bill.selected_customer.customer_id, ref visit_source_old);
                visit_new = utilities.GetVisitDetails(selected_customer_temp.customer_id, ref visit_source_new);

                if (visit_source_old.Length > 0 && visit_source_new.Length > 0)
                {
                    //ROLLBACK OLD VISIT
                    visit_old.RemoveBillFromVisit(current_bill);
                    if (visit_source_old == "memory")
                    {
                        redis_utilities.UpdateVisitDetails(redisConnection, visit_old);
                    }
                    else if (visit_source_old == "local")
                    {
                        pgsql_utilities.UpdateVisitDetails(MainForm.pgsql_connection, visit_old);
                    }

                    //UPDATE SELECTED_CUSTOMER_TEMP TO VISIT
                    visit_new.customer_id = selected_customer_temp.customer_id;
                    
                    visit_new.AddBillToVisit(current_bill, user_rating, user_feedback);

                    if (visit_source_new == "memory")
                    {
                        redis_utilities.UpdateVisitDetails(redisConnection, visit_new);
                    }
                    else if (visit_source_new == "local")
                    {
                        pgsql_utilities.UpdateVisitDetails(MainForm.pgsql_connection, visit_new);
                    }
                }

                else Console.WriteLine("Visit not found!");

                redis_customer customer_old = new redis_customer();
                redis_customer customer_new = new redis_customer();
                string customer_source_old = "";
                string customer_source_new = "";

                customer_old = utilities.GetCustomerDetails(current_bill.selected_customer.customer_id, ref customer_source_old);
                customer_new = utilities.GetCustomerDetails(selected_customer_temp.customer_id, ref customer_source_new);

                if (customer_source_old.Length > 0 && customer_source_new.Length > 0)
                {
                    //Rollback old customer
                    customer_old.RemoveBillFromCustomer(current_bill);
                    if (customer_source_old == "memory")
                    {
                        redis_utilities.UpdateCustomerDetails(redisConnection, customer_old);
                    }
                    else if (customer_source_old == "local")
                    {
                        pgsql_utilities.UpdateCustomerDetails(MainForm.pgsql_connection, customer_old);
                    }

                    customer_new.customer_id = selected_customer_temp.customer_id;
                    customer_new.AddBillToCustomer(current_bill);
                    if (customer_source_new == "memory")
                    {
                        redis_utilities.UpdateCustomerDetails(redisConnection, customer_new);
                    }
                    else if (customer_source_new == "local")
                    {
                        pgsql_utilities.UpdateCustomerDetails(MainForm.pgsql_connection, customer_new);
                    }
                }


                current_bill.selected_customer = selected_customer_temp;
            }

            pgsql_utilities.UpdateBillDetails(MainForm.pgsql_connection, current_bill);
        }
    }
}
