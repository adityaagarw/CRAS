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
using Npgsql;
using CRAS.Properties;

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
        public static BindingList<visit_details> visits = new BindingList<visit_details>();

        public static int bill_scanning = 0;
        public static BindingList<redis_customer> exited_customers = new BindingList<redis_customer>();
        //public static BindingList<> 
        public static NpgsqlConnection pgsql_connection;


        public MainForm()
        {
            InitializeComponent();
            //InitializeSQLNotification();
        }

        private void ShowLoadingForm()
        {
            loadingForm = new LoadingForm();
            Application.Run(loadingForm);
        }

       
        private void MainForm_Load(object sender, EventArgs e)
        {
            log logger = new log("Front End", "Main Form", "Application Started","", "Frontend Initialised!", "MainForm.cs line: 76");
            logger.Print();
            logger.LogToSQL();

            this.Icon = Resources.cras;

            Thread loadingThread = new Thread(ShowLoadingForm);
            loadingThread.Start();

            
            exitedCustomerFLP.Parent = customerFlowLayout.Parent;
            exitedCustomerFLP.Location = customerFlowLayout.Location;
            scanStatusLabel.Text = "Scan Completed";

            if (loadingForm != null) loadingForm.SetLoadingLabel("Connecting To Redis!");
            redisConnection = redis_utilities.ConnectToRedis("127.0.0.1", "6379");
            redisDB = redisConnection.GetDatabase();

            customer_list = redis_utilities.GetCustomerDetails(redisConnection);
            //customer_list = customer_list.OrderByDescending(customer => customer.entry_time).ToList();
            customer_list = utilities.OrderBy(customer_list, "entry_time", "DESC");

            customer_list_count = customer_list.Count();
            totalCustomersValue.Invoke(new Action(() => { totalCustomersValue.Text = customer_list_count.ToString(); }));

            if(loadingForm != null) loadingForm.SetLoadingLabel("Initializing PUBSUB!");
            pubsub_utilities.InitialiseSubscribers("Backend", this);
            pubsub_utilities.InitialiseSubscribers("Billing", this);
            pubsub_utilities.InitialiseSubscribers("Employee", this);
            pubsub_utilities.InitialiseSubscribers("Log", this);

            if (loadingForm != null) loadingForm.SetLoadingLabel("Connecting to PGSQL");
            pgsql_connection = pgsql_utilities.ConnectToPGSQL();
            pgsql_connection.Open();
            Console.WriteLine("PGSQL Connected to Server Version: " + pgsql_connection.ServerVersion.ToString());
            pgsql_connection.Close();

            //Add ListChanged Listener to customer_list
            customer_list.ListChanged += Customer_list_ListChanged;
            exited_customers.ListChanged += Exited_customers_ListChanged;
            bills = pgsql_utilities.GetBillDetails(pgsql_connection, "", "", 10);
            bills.ListChanged += Bills_ListChanged;


            PopulateCustomerFlowLayout(customer_list);

            loadingForm.Invoke(new Action(() => loadingForm.Close()));

            displayComboBox.Items.Add("In-Store Customers");
            displayComboBox.Items.Add("Exited Customers");
            displayComboBox.Items.Add("Employees");
            displayComboBox.SelectedIndex = 0;
        }

        private void Exited_customers_ListChanged(object sender, ListChangedEventArgs e)
        {
            int new_records = exited_customers.Count - exited_customers_count;

            if (new_records > 0)
            {

                for (int i = 0; i < new_records; i++)
                {
                    InsertCustomerInLayout(exitedCustomerFLP, exited_customers[exited_customers_count + i], i);
                    //Console.WriteLine("Customer added to Exited List! New Records: " + exited_customers[exited_customers_count + i].customer_id);

                }
                exited_customers_count = exited_customers.Count;
                totalExitedValue.Invoke(new Action(() => { totalExitedValue.Text = exited_customers_count.ToString(); }));
            }
            //throw new NotImplementedException();
        }

        private void Bills_ListChanged(object sender, ListChangedEventArgs e)
        {

            BillingForm billingForm = GetBillingFormIfOpen();

            if(billingForm != null)
            {
                if (billingForm.current_bill == null)
                {
                    billingForm.current_bill = bills[bills.Count - 1];
                    billingForm.Invoke(new Action(() => { billingForm.PopulateBillDetails(billingForm.current_bill); }));

                    
                }

                BindingList<redis_customer> customers = billingForm.current_bill.identified_customers;
                if (billingForm.identifiedFacesFLP.Controls.Count > 0)
                //if (customers.Count > 0)
                {
                    //Console.WriteLine("Inserting identified customer to FLP at index " + billingForm.identifiedFacesFLP.Controls.Count);
                    if (billingForm.identifiedFacesFLP.Controls.Count != customers.Count)
                    {
                        //billingForm.InsertIdentifiedCustomer(customers[customers.Count - 1]);
                        billingForm.Invoke(new Action(() => { billingForm.InitializeCustomerFLP(billingForm.current_bill); }));

                    }
                }
                else
                {
                    if (customers != null)
                    {
                        //billingForm.InsertIdentifiedCustomer(customers[0]);
                        billingForm.Invoke(new Action (() => { billingForm.InitializeCustomerFLP(billingForm.current_bill); }));

                    }
                    //else Console.WriteLine("Bill Changed but no customer present");

                }

            }

            else
            {
                //Console.WriteLine("Billing Form Not Open!");
            }
            //throw new NotImplementedException();
        }

        private void Customer_list_ListChanged(object sender, ListChangedEventArgs e)
        {
            
            int new_records = customer_list.Count - customer_list_count;


            if (new_records > 0)
            {
                for (int i = 0; i < new_records; i++)
                {
                    InsertCustomerInLayout(customerFlowLayout, customer_list[i], i);
                }
            }

            else if (new_records<0)            
            {    
                /*for (int i = 0; i < -1*new_records; i++)
                {
                    //InsertCustomerInLayout(customerFlowLayout, customer_list[i], i);
                    Console.WriteLine("Remove Customer: " + customer_list[e.NewIndex].customer_id + " from in-Store list!");
                }*/
            }
            customer_list_count = customer_list.Count();
            totalCustomersValue.Invoke(new Action(() => { totalCustomersValue.Text = customer_list_count.ToString(); }));
            //throw new NotImplementedException();
        }

        public void InsertCustomerInLayout(FlowLayoutPanel panel, redis_customer customer, int index = -1)
        {
            CustomerDataUC customerData = new CustomerDataUC();

            customerData.ControlDoubleClicked += UserControl_ControlDoubleClicked;
            customerData.SetImage(customer.image);
            customerData.SetLabel("label1", "Name: ", customer.name);
            customerData.SetLabel("label2", "Num Visits: ", customer.num_visits.ToString());
            customerData.SetLabel("label3", "Returning: ", customer.return_customer);
            customerData.SetLabel("label4", "Customer Id: ", customer.customer_id);
            customerData.SetLabel("label5", "Last Visit: ", customer.last_visit.ToString("dd-MM-yyyy HH:mm:ss"));
            customerData.SetLabel("label6", "Entry Time: ", customer.entry_time.ToShortTimeString());
            customerData.selectCustomerLabel.Hide();

            //Give delete button in CustomerFlowLayout to Delete Duplicate entries & markAsEmployeeButton to mark customer as employee
            if (panel.Name.Equals(customerFlowLayout.Name))
            {
                panel.Invoke(new Action(() => { customerData.deleteButton.Visible = true; }));
                customerData.DeleteButtonClicked += CustomerData_DeleteButtonClicked;

                panel.Invoke(new Action(() => { customerData.markAsEmployee.Visible = true; }));
                customerData.MarkAsEmployeeClicked += CustomerData_MarkAsEmployeeClicked;
            }

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

        private void CustomerData_MarkAsEmployeeClicked(object sender, int e)
        {
            //throw new NotImplementedException();
            //AddEmployeeForm addEmployeeForm = new AddEmployeeForm("MarkAsEmployee", ((CustomerDataUC)sender).label4value.Text.ToString(), ((CustomerDataUC)sender).customerPictureBox.Image);
            AddEmployeeForm addEmployeeForm = new AddEmployeeForm("MarkAsEmployee", (CustomerDataUC)sender);
            addEmployeeForm.ShowDialog();
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

            //MessageBox.Show("Sender: " + sender.ToString());
            if (displayComboBox.SelectedIndex == 0) ShowCustomerDetail(customer_list[index], selectedCustomerUC);
            else if (displayComboBox.SelectedIndex == 1) ShowCustomerDetail(exited_customers[index], selectedCustomerUC);
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
            if (sqlDependency != null)
            {
                sqlDependency.Stop();
                sqlDependency.Dispose();
            }
            //SqlDependency.Stop(posdb_utilities.GetConnectionString(POSSERVER, POSDB, POSUID, POSPWD, posdb));

            log logger = new log("Front End", "Main Form", "Application Closed", "", "UI Closed!", "MainForm.cs Line 333");
            logger.LogToSQL();
        }

        private NamedPipeClientStream StartPipe(object camera_id)
        {
            // Connect to the named pipe
            string pipe_name = "webcam_" + camera_id.ToString();
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
            string pipe_name = "cam_" + camera_id.ToString();
            Console.WriteLine("Pipe name:" + pipe_name);

            PipeSecurity pipeSecurity = new PipeSecurity();
            PipeAccessRule psEveryone = new PipeAccessRule("Everyone", PipeAccessRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);

            pipeSecurity.AddAccessRule(psEveryone);

            NamedPipeClientStream pipeClient;

            PictureBox pictureBox = new PictureBox();
            
            if (camera_id.ToString().Equals("entry"))
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
                    //Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
        private void CustomerData_DeleteButtonClicked(object sender, int e)
        {
            CustomerDataUC selectedCustomerUC = (CustomerDataUC)sender;
            int index = selectedCustomerUC.Parent.Controls.IndexOf(selectedCustomerUC);
            
            redis_customer customer = customer_list[index];
            visit_details visit = redis_utilities.GetVisitDetails(redisConnection, customer.customer_id)[0];

            if (customer != null) 
            {
                customer_list.RemoveAt(index);
                customerFlowLayout.Controls.RemoveAt(index);
                redis_utilities.DeleteRedisEntry(redisConnection, customer.key);

                if(visit!=null) redis_utilities.DeleteRedisEntry(redisConnection, visit.key);

                MessageBox.Show($"Customer {customer.customer_id} at index {index} deleted successfully!");
            }
            //MessageBox.Show("Customer Deleted: " + index.ToString() + customer_list[index].customer_id);
            //throw new NotImplementedException();
        }

        public void DeleteCustomer(int index)
        {
            redis_customer customer = customer_list[index];

            if (customer != null)
            {
                customer_list.RemoveAt(index);
                customerFlowLayout.Controls.RemoveAt(index);
                redis_utilities.DeleteRedisEntry(redisConnection, customer.key);
                MessageBox.Show($"Customer {customer.customer_id} at index {index} deleted successfully!");
            }
        }

        class Program
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
                pipeThread1.Start("entry");

                cam1StatusLabel.Text = "active";
                cam1StatusLabel.ForeColor = Color.Green;


                pipeThread2 = new Thread(ReceiveFrames);
                pipeThread2.Start("exit");

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
            mapper.AddMapping(c => c.billDate, "AddDate");
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
            
            changedEntity.billAmtInt = int.Parse(changedEntity.billAmt.Split('.')[0]);
            changedEntity.returnAmtInt = int.Parse(changedEntity.returnAmt.Split('.')[0]);
            changedEntity.qtyInt = int.Parse(changedEntity.qty.Split('.')[0]);
            changedEntity.billAmt = changedEntity.billAmt.Split('.')[0];
            changedEntity.returnAmt = changedEntity.returnAmt.Split('.')[0];
            changedEntity.qty = changedEntity.qty.Split('.')[0];
            changedEntity.Print();

            if (e.ChangeType == TableDependency.SqlClient.Base.Enums.ChangeType.Insert)
            {
                changedEntity.identified_customers.ListChanged += Bills_ListChanged;
                bills.Add(changedEntity);

                pubsub_utilities.PublishMessage("Frontend", "StartBilling");
                bill_scanning = 1;
                Invoke(new Action(() => { scanStatusLabel.Text = "Scanning"; }));
            }
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

        public static BillingForm GetBillingFormIfOpen()
        {
            BillingForm billingForm = Application.OpenForms.OfType<BillingForm>().FirstOrDefault();

            return billingForm;
        }

        public static LogViewerForm GetLogViewerFormIfOpen()
        {
            LogViewerForm logViewerForm = Application.OpenForms.OfType<LogViewerForm>().FirstOrDefault();

            return logViewerForm;
        }

        public static AddEmployeeForm GetAddEmployeeFormIfOpen()
        {
            AddEmployeeForm addEmployeeForm = Application.OpenForms.OfType<AddEmployeeForm>().FirstOrDefault();
            return addEmployeeForm;
        }

        public static LoadingForm GetLoadingFormIfOpen()
        {
            LoadingForm loadingForm = Application.OpenForms.OfType<LoadingForm>().FirstOrDefault();
            return loadingForm;
        }
        private void scanStatusLabel_TextChanged(object sender, EventArgs e)
        {
            if(scanStatusLabel.Text == "Scanning")
            {
                bill_scanning = 1;
                BillingForm billingForm = GetBillingFormIfOpen();
                if (billingForm != null)
                {
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
                totalExitedLabel.Visible = true;
                totalExitedValue.Visible = true;
                totalCustomersLabel.Visible = false;
                totalCustomersValue.Visible = false;
            }

            if(displayComboBox.SelectedIndex == 0)
            {
                customerFlowLayout.Show();
                exitedCustomerFLP.Hide();
                totalExitedLabel.Visible = false;
                totalExitedValue.Visible = false;
                totalCustomersLabel.Visible = true;
                totalCustomersValue.Visible = true;
            }
        }

        private void dbButton_Click(object sender, EventArgs e)
        {
            DBDisplayForm dBDisplayForm = new DBDisplayForm();
            dBDisplayForm.Show();
        }

        private void bhDashboardButton_Click(object sender, EventArgs e)
        {
            BHDashboard bHDashboard = new BHDashboard();
            bHDashboard.Show();
        }

        private void addEmployee_Click(object sender, EventArgs e)
        {
            AddEmployeeForm addEmployeeForm = new AddEmployeeForm("AddNewEmployee");
            addEmployeeForm.ShowDialog();
            
        }

        private void logPictureBox_Click(object sender, EventArgs e)
        {
            LogViewerForm logViewerForm = new LogViewerForm();
            logViewerForm.Show();
        }
    }
}
