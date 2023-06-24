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
        public int current_bill_index = 0;
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
            if(this.IsHandleCreated) identifiedFacesFLP.Invoke(new Action (()=> { identifiedFacesFLP.Controls.Add(pictureBox); }));
            
        }

        private void PopulateCustomerUC(int index)
        {
            redis_customer customer = current_bill.identified_customers[index];

            selectedCustomerUC.SetImage(customer.image);
            selectedCustomerUC.SetLabel("label1", "Name: ", customer.name);
            selectedCustomerUC.SetLabel("label2", "Phone: ", customer.phone_number);
            selectedCustomerUC.SetLabel("label3", "Returning: ", customer.return_customer);
            selectedCustomerUC.SetLabel("label4", "Category: ", customer.category);
            selectedCustomerUC.SetLabel("label5", "Last Visit: ", customer.last_visit);
            selectedCustomerUC.SetLabel("label6", "Entry Time: ", customer.entry_time.ToShortTimeString());
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
            
            selectedPictureBox.BorderStyle = BorderStyle.Fixed3D;
           
            PopulateCustomerUC(index);

            //throw new NotImplementedException();*/
        }

        private void InitializeFLP(FlowLayoutPanel panel)
        {

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
            //MessageBox.Show("User Rating: " + (index + 1).ToString());
            //throw new NotImplementedException();
        }

        private void InitializeCustomerFLP(bill_details bill)
        {
            identifiedFacesFLP.Controls.Clear();
            foreach (redis_customer customer in bill.identified_customers)
            {
                InsertIdentifiedCustomer(customer);
            }
            //identified_customers.Clear();
            //identified_customers_count = identified_customers.Count;
        }

        public void StartScanning()
        {
            //Thread.Sleep(500);
            pubsub_utilities.PublishMessage("Frontend", "StartBilling");
            //scanStatus.Text = "Scanning";
            //scanStatus.ForeColor = Color.DarkGreen;
        }
        private void BillingForm_Load(object sender, EventArgs e)
        {
            //StartScanning();
            previousButton.Enabled = false;
            nextButton.Enabled = false;

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
            }
            if(total_bills > 1)
            {
                previousButton.Enabled = true;
            }
        }

        private void PopulateBillDetails(bill_details bill)
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
                InitializeCustomerFLP(current_bill);
                StartScanning();
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

            SetNavigationButtons();
            current_bill = MainForm.bills[current_bill_index];
            InitializeCustomerFLP(current_bill);
            PopulateBillDetails(current_bill);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            current_bill_index++;

            SetNavigationButtons();
            current_bill = MainForm.bills[current_bill_index];
            InitializeCustomerFLP(current_bill);
            PopulateBillDetails(current_bill);
        }

        private void updateButton_Click(object sender, EventArgs e)
        {

        }
    }
}
