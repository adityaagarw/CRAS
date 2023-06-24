using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using TableDependency.SqlClient.Base;
using System.Security.Cryptography;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.EventArgs;

namespace CRAS
{
    public enum POSDB
    {
        SQL,
        MongoDB,
        PostGreSQL
    }
    public partial class MainForm : Form
    {
        private Thread pipeThread1;
        private Thread pipeThread2;
        private NamedPipeClientStream pipeClient1;
        private NamedPipeClientStream pipeClient2;
        private Process pythonProcess1;
        private Process pythonProcess2;
        public static ConnectionMultiplexer redisConnection;
        public static IDatabase redisDB;
        public static BindingList<redis_customer> customer_list;
        CustomerDataUC selectedCustomerUC;
        public static LoadingForm loadingForm;
        private int customer_list_count = 0;
        private int exited_customers_count = 0;
        private static int streaming = 0;
        private SqlConnection connection;
        private SqlCommand command;
        private SqlTableDependency<bill_details> sqlDependency;
        public static POSDB posdb = CRAS.POSDB.SQL;
        public static string POSSERVER = "Junior\\SQLExpress";
        public static string POSDB = "PantHouse_GRetail";
        public static string POSUID = "sa";
        public static string POSPWD = "aa";
        public static BindingList<bill_details> bills = new BindingList<bill_details>();
        public static int bill_scanning = 0;
        public static BindingList<redis_customer> exited_customers = new BindingList<redis_customer>();


        public MainForm()
        {
            InitializeComponent();
            InitializeSQLNotification();
        }

        private void ShowLoadingForm()
        {
            loadingForm = new LoadingForm();
            Application.Run(loadingForm);
        }

       
        private void MainForm_Load(object sender, EventArgs e)
        {
            Thread loadingThread = new Thread(ShowLoadingForm);
            loadingThread.Start();

            scanStatusLabel.Text = "Scan Completed";

            redisConnection = redis_utilities.ConnectToRedis("127.0.0.1", "6379");
            redisDB = redisConnection.GetDatabase();

            customer_list = redis_utilities.ReadAllDataFromRedis(redisConnection);
            //customer_list = customer_list.OrderByDescending(customer => customer.entry_time).ToList();
            customer_list = utilities.OrderBy(customer_list, "entry_time", "DESC");

            customer_list_count = customer_list.Count();

            pubsub_utilities.InitialiseSubscribers("Backend");
            pubsub_utilities.InitialiseSubscribers("Billing", this);


            //Add ListChanged Listener to customer_list
            customer_list.ListChanged += Customer_list_ListChanged;
            exited_customers.ListChanged += Exited_customers_ListChanged;
            bills.ListChanged += Bills_ListChanged;

            PopulateCustomerFlowLayout(customer_list);

            loadingForm.Invoke(new Action(() => loadingForm.Close()));

            displayComboBox.Items.Add("In-Store Customers");
            displayComboBox.Items.Add("Exited Customers");
            displayComboBox.SelectedIndex = 0;
        }

        private void Exited_customers_ListChanged(object sender, ListChangedEventArgs e)
        {
            int new_records = exited_customers.Count - exited_customers_count;

            if(new_records > 0)
            {

                for (int i = 0; i < new_records; i++)
                {
                    InsertCustomerInLayout(exitedCustomerFLP, customer_list[i], i);
                }
            }
            //throw new NotImplementedException();
        }

        private void Bills_ListChanged(object sender, ListChangedEventArgs e)
        {
            BillingForm billingForm = GetBillingFormIfOpen();

            if(billingForm != null)
            {
                BindingList<redis_customer> customers = billingForm.current_bill.identified_customers;
                if (billingForm.identifiedFacesFLP.Controls.Count != customers.Count)
                {
                    billingForm.InsertIdentifiedCustomer(customers[customers.Count-1]);
                }
            }
            //throw new NotImplementedException();
        }

        private void Customer_list_ListChanged(object sender, ListChangedEventArgs e)
        {
            //PopulateCustomerFlowLayout();
            int new_records = customer_list.Count - customer_list_count;

            if (new_records > 0)
            {
                for (int i = 0; i < new_records; i++)
                {
                    InsertCustomerInLayout(customerFlowLayout, customer_list[i], i);
                }
            }

            else if(new_records < 0)
            {

            }
            customer_list_count = customer_list.Count();
            //throw new NotImplementedException();
        }

        public void InsertCustomerInLayout(FlowLayoutPanel panel, redis_customer customer, int index = -1)
        {
            CustomerDataUC customerData = new CustomerDataUC();

            customerData.ControlDoubleClicked += UserControl_ControlDoubleClicked;
            customerData.SetImage(customer.image);
            customerData.SetLabel("label1", "Name: ", customer.name);
            customerData.SetLabel("label2", "Phone: ", customer.phone_number);
            customerData.SetLabel("label3", "Returning: ", customer.return_customer);
            customerData.SetLabel("label4", "Category: ", customer.category);
            customerData.SetLabel("label5", "Last Visit: ", customer.last_visit);
            customerData.SetLabel("label6", "Entry Time: ", customer.entry_time.ToShortTimeString());

            if (index > -1)
            {
                panel.Invoke(new Action(() => { panel.SuspendLayout(); }));
                panel.Invoke(new Action(() => { panel.Controls.Add(customerData); }));
                panel.Invoke(new Action(() => { panel.Controls.SetChildIndex(customerData, index); }));
                panel.Invoke(new Action(() => { panel.ResumeLayout(); }));
            }
            else
                panel.Invoke(new Action(() => { panel.Controls.Add(customerData); }));

            //customerFlowLayout.Controls.Add(customerData);
        }
        public void PopulateCustomerFlowLayout(BindingList<redis_customer> customers)
        {
          
                customerFlowLayout.Invoke(new Action(() => { customerFlowLayout.Controls.Clear(); }));
                customerFlowLayout.Invoke(new Action(() => { customerFlowLayout.FlowDirection = FlowDirection.TopDown; }));
                customerFlowLayout.Invoke(new Action(() => { customerFlowLayout.AutoScroll = true; }));

         
                foreach (var customer in customers)
                {
                    InsertCustomerInLayout(customerFlowLayout, customer);
                }
           
        }

        private void ShowCustomerDetail(redis_customer customer, CustomerDataUC customerData)
        {
            Form customerDetail = new CustomerDetailForm(customer);
            customerDetail.ShowDialog();

            if(customerDetail.DialogResult == DialogResult.OK)
            {
                customerData.SetLabel("label1", "Name: ", customer.name);
                customerData.SetLabel("label2", "Phone: ", customer.phone_number);
                //customerData.SetLabel("label2", "Phone: ", customer.phone_number);

            }
        }

        private void UserControl_ControlDoubleClicked(object sender, int index)
        {
            selectedCustomerUC = (CustomerDataUC)sender;
            bool selected = selectedCustomerUC.isSelected;

            CustomerDataUC.UnselectAllControls(customerFlowLayout);

            if (!selected) selectedCustomerUC.Select();
            else if (selected) selectedCustomerUC.UnSelect();

            ShowCustomerDetail(customer_list[index], selectedCustomerUC);
        }
        
        
        private void ClosePipes()
        {
            pipeClient1?.Close();
            pipeClient1?.Dispose();
            pipeThread1?.Abort();
            pipeClient2?.Close();
            pipeClient2?.Dispose();
            pipeThread2?.Abort();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop the thread and close the named pipe when the form is closing
            //StopPythonScript();
            ClosePipes();
            sqlDependency.Stop();
            sqlDependency.Dispose();
            //SqlDependency.Stop(posdb_utilities.GetConnectionString(POSSERVER, POSDB, POSUID, POSPWD, posdb));

        }

        private NamedPipeClientStream StartPipe(object camera_id)
        {
            // Connect to the named pipe
            string pipe_name = "webcam_feed" + camera_id.ToString();
            Console.WriteLine("Pipe name:" + pipe_name);

            PipeSecurity pipeSecurity = new PipeSecurity();
            PipeAccessRule psEveryone = new PipeAccessRule("Everyone", PipeAccessRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);

            pipeSecurity.AddAccessRule(psEveryone);

            NamedPipeClientStream pipeClient;

            pipeClient = new NamedPipeClientStream(".", pipe_name, PipeDirection.In, PipeOptions.Asynchronous);

            return pipeClient;
        }

        private int StartStreaming(NamedPipeClientStream pipeClient, PictureBox pictureBox)
        {
            pipeClient.Connect();
            while (true)
            {
                try
                {
                    // Read frame data from the named pipe
                    byte[] frameData = new byte[pipeClient.InBufferSize];
                    int bytesRead = pipeClient.Read(frameData, 0, frameData.Length);

                    // Display the frame in the PictureBox control
                    if (bytesRead > 0 && pipeClient.IsConnected)
                    {
                        Image frameImage = Image.FromStream(new MemoryStream(frameData));
                        pictureBox.Invoke((MethodInvoker)(() =>
                        {
                            pictureBox.Image = frameImage;
                        }));
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during reading or displaying frames
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            return 0;
        }

        private void ReceiveFrames(object camera_id)
        {
            // Connect to the named pipe
            string pipe_name = "webcam_feed" + camera_id.ToString();
            Console.WriteLine("Pipe name:" + pipe_name);

            PipeSecurity pipeSecurity = new PipeSecurity();
            PipeAccessRule psEveryone = new PipeAccessRule("Everyone", PipeAccessRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);

            pipeSecurity.AddAccessRule(psEveryone);

            NamedPipeClientStream pipeClient;

            PictureBox pictureBox = new PictureBox();
            
            if (camera_id.ToString().Equals("0"))
            {
                pipeClient1 = new NamedPipeClientStream(".", pipe_name, PipeDirection.In, PipeOptions.Asynchronous);
                pipeClient = pipeClient1;
                pipeClient.Connect();
                pictureBox = pictureBox1;
            }

            else
            {
                pipeClient2 = new NamedPipeClientStream(".", pipe_name, PipeDirection.In, PipeOptions.Asynchronous);
                pipeClient = pipeClient2;
                pipeClient.Connect();
                pictureBox = pictureBox2;
            }

            //StartStreaming(pictureBox);

            while (true)
            {
                try
                {
                    // Read frame data from the named pipe
                    byte[] frameData = new byte[pipeClient.InBufferSize];
                    int bytesRead = pipeClient.Read(frameData, 0, frameData.Length);

                    // Display the frame in the PictureBox control
                    if (bytesRead > 0 && pipeClient.IsConnected)
                    {
                        Image frameImage = Image.FromStream(new MemoryStream(frameData));
                        pictureBox.Invoke((MethodInvoker)(() =>
                        {
                            pictureBox.Image = frameImage;
                        }));
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during reading or displaying frames
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
        static class Program
        {
            /// <summary>
            /// The main entry point for the application.
            /// </summary>
            [STAThread]
            static void Main()
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }

        private void startStopFeedButton_Click(object sender, EventArgs e)
        {
            if (streaming == 0)
            {
                //StartPythonScript("0");
                //StartPythonScript("1");

                // Start the separate thread to read from the named pipe
                pipeThread1 = new Thread(ReceiveFrames);
                pipeThread1.Start("0");

                cam1StatusLabel.Text = "active";
                cam1StatusLabel.ForeColor = Color.Green;


                pipeThread2 = new Thread(ReceiveFrames);
                pipeThread2.Start("1");

                cam2StatusLabel.Text = "active";
                cam2StatusLabel.ForeColor = Color.Green;

                startStopFeedButton.Text = "Stop Live Feed";

                streaming = 1;
            }

            else
            {
                //StopPythonScript();
                ClosePipes();
                pictureBox1.Enabled = false; 
                pictureBox2.Enabled = false;

                cam1StatusLabel.Text = "inactive";
                cam1StatusLabel.ForeColor = Color.Red;
                cam2StatusLabel.Text = "inactive";
                cam2StatusLabel.ForeColor = Color.Red;

                startStopFeedButton.Text = "Start Live Feed";

                streaming = 0;
            }
        }

        private List<string> GetListOfPipes(string pipeNamePrefix)
        {
            List<string> pipes = new List<string>();
            String[] listOfPipes = System.IO.Directory.GetFiles(@"\\.\pipe\");
            foreach (String pipe in listOfPipes)
            {
                if (pipe.Contains("webcam_feed")) pipes.Add(pipe);
            }
            return pipes;
        }

        private void statusButton_Click(object sender, EventArgs e)
        {
            Live_Status status = new Live_Status();
            status.Show();
        }

        private void populateDataGridButton_Click(object sender, EventArgs e)
        {
            //DisplayDataInDataGridView(customer_list);
            PopulateCustomerFlowLayout(customer_list);
        }

        private redis_customer UpdateCustomerRecord (string customer_id, string column_name, string new_value)
        {
            foreach (redis_customer customer in customer_list) 
            { 
                if(customer.customer_id.Equals(customer_id)) 
                {
                    if(column_name.Equals("name")) customer.name = new_value;
                    return customer;
                }
            }
            return null;
        }

        private void billingButton_Click(object sender, EventArgs e)
        {
            BillingForm billingForm;
            if (bills.Count > 0)
            {
                billingForm = new BillingForm(bills[bills.Count - 1]);
            }
            else
            {
                billingForm = new BillingForm();
            }
            billingForm.Show(this);
            this.Hide();
        }

        public void InitializeSQLNotification()
        {
            string connectionString = posdb_utilities.GetConnectionString(POSSERVER, POSDB, POSUID, POSPWD, posdb);
            string query = "SELECT * FROM [dbo].[trnSales]";
            string service = "NewBillService";
            string contract = "NewBillContract";


            connection = new SqlConnection(connectionString);

            // The mapper object is used to map model properties 
            // that do not have a corresponding table column name.
            // In case all properties of your model have same name 
            // of table columns, you can avoid to use the mapper.
            var mapper = new ModelToTableMapper<bill_details>();
            mapper.AddMapping(c => c.name, "AccountName");
            mapper.AddMapping(c => c.mobile, "MobileNo");
            mapper.AddMapping(c => c.billNo, "VoucherNo");
            mapper.AddMapping(c => c.billAmt, "NetAmt");
            mapper.AddMapping(c => c.returnAmt, "ReturnAmt");
            mapper.AddMapping(c => c.qty, "TotalQty");

            // Here - as second parameter - we pass table name: 
            // this is necessary only if the model name is different from table name 
            // (in our case we have Customer vs Customers). 
            // If needed, you can also specifiy schema name.
            sqlDependency = new SqlTableDependency<bill_details>(connectionString, "trnSales", mapper: mapper); 
            
            sqlDependency.OnChanged += Changed;
            sqlDependency.Start();

            Console.WriteLine("Started SQL Listener");
        }
        public void Changed(object sender, RecordChangedEventArgs<bill_details> e)
        {
            var changedEntity = e.Entity;

            Console.WriteLine("DML operation: " + e.ChangeType);
            changedEntity.Print();
            bills.Add(changedEntity);

            pubsub_utilities.PublishMessage("Frontend", "StartBilling");
            bill_scanning = 1;
            Invoke(new Action(() => { scanStatusLabel.Text = "Scanning"; }));

        }

        public Form GetChildFormIfAny(string formName)
        {
            foreach(Form form in MdiChildren)
            {
                if(form.Name == formName)
                {
                    return form;
                }
            }
            return null;
        }

        public BillingForm GetBillingFormIfOpen()
        {
            BillingForm billingForm = Application.OpenForms.OfType<BillingForm>().FirstOrDefault();

            return billingForm;
        }
        private void scanStatusLabel_TextChanged(object sender, EventArgs e)
        {
            if(scanStatusLabel.Text == "Scanning")
            {
                bill_scanning = 1;
                BillingForm billingForm = GetBillingFormIfOpen();
                if (billingForm != null)
                {
                    Console.WriteLine("Billing form button disabled!");
                    billingForm.rescanButton.Enabled = false;
                    billingForm.rescanButton.BackColor = Color.DarkGray;
                    billingForm.scanStatus.Text = "Scanning";
                    billingForm.scanStatus.ForeColor = Color.DarkGreen;
                    billingForm.previousButton.Enabled = false;
                    billingForm.nextButton.Enabled = false;
                }
            }
            else if (scanStatusLabel.Text == "Scan Completed")
            {
                bill_scanning = 0;
                BillingForm billingForm = GetBillingFormIfOpen();
                if (billingForm != null)
                {
                    billingForm.rescanButton.Enabled = true;
                    billingForm.rescanButton.BackColor = Color.White;
                    billingForm.scanStatus.Text = "Scan Completed";
                    billingForm.scanStatus.ForeColor = Color.Gray;
                    //billingForm.previousButton.Enabled = true;
                    //billingForm.nextButton.Enabled = true;

                    billingForm.total_bills = bills.Count;
                    billingForm.SetNavigationButtons();
                }
            }
        }

        private void displayComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(displayComboBox.SelectedIndex == 1)
            {
                customerFlowLayout.Hide();
                exitedCustomerFLP.Show();
            }

            if(displayComboBox.SelectedIndex == 0)
            {
                customerFlowLayout.Show();
                exitedCustomerFLP.Hide();
            }
        }
    }
}
