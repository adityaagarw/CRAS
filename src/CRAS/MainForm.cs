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

namespace CRAS
{
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
        BindingList<redis_customer> customer_list;
        CustomerDataUC selectedCustomerUC;
        public static LoadingForm loadingForm;
        public MainForm()
        {
            InitializeComponent();
        }

        /*private ConnectionMultiplexer EstablishRedisConnection()
        {
            // Keep trying to establish a connection until successful
            while (redisConnection == null || !redisConnection.IsConnected)
            {
                try
                {
                    redisConnection = redis_utilities.ConnectToRedis("127.0.0.1");
                }
                catch (RedisConnectionException)
                {
                    // Connection failed, wait for a while before retrying
                    Thread.Sleep(1000);
                }
            }
        }*/

        private void ShowLoadingForm()
        {
            loadingForm = new LoadingForm();
            Application.Run(loadingForm);
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            //StartPythonScript("0");

            //StartPythonScript("1");

            //loadingForm.Show();

            //redisConnection = redis_utilities.ConnectToRedis("127.0.0.1");
            Thread loadingThread = new Thread(ShowLoadingForm);
            loadingThread.Start();

            redisConnection = redis_utilities.ConnectToRedis("127.0.0.1", "6379");
            redisDB = redisConnection.GetDatabase();

            loadingForm.Invoke(new Action(() => loadingForm.Close()));

            customer_list = redis_utilities.ReadAllDataFromRedis(redisConnection);
            customer_list = utilities.OrderBy(customer_list, "entry_time", "DESC");

            
            InitialiseSubscribers();

            //Add ListChanged Listener to customer_list
            customer_list.ListChanged += Customer_list_ListChanged;
            PopulateCustomerFlowLayout();
        }

        private void InitialiseSubscribers()
        {
            ISubscriber newCustomerSub = redisConnection.GetSubscriber();
            ISubscriber existingCustomerSub = redisConnection.GetSubscriber();
            ISubscriber employeeSub = redisConnection.GetSubscriber();

            newCustomerSub.Subscribe("Backend", (channel, message) => Console.WriteLine(message));
        }

        private void Customer_list_ListChanged(object sender, ListChangedEventArgs e)
        {
            PopulateCustomerFlowLayout();
            //throw new NotImplementedException();
        }

        public void PopulateCustomerFlowLayout()
        {
            customerFlowLayout.Controls.Clear();
            customerFlowLayout.FlowDirection = FlowDirection.TopDown;
            customerFlowLayout.AutoScroll = true;

            foreach (var customer in customer_list)
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


                customerFlowLayout.Controls.Add(customerData);
            }
        }

        private void UserControl_ControlClicked(object sender, int index)
        {
            selectedCustomerUC = (CustomerDataUC)sender;
            bool selected = selectedCustomerUC.isSelected;

            CustomerDataUC.UnselectAllControls(customerFlowLayout);
            
            if (!selected) selectedCustomerUC.Select();
            else if(selected) selectedCustomerUC.UnSelect();
        }

        private void OpenCustomerDetail(redis_customer customer, CustomerDataUC customerData)
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

            OpenCustomerDetail(customer_list[index], selectedCustomerUC);
        }
        
        private void StartPythonScript(string cameraId)
        {
            // Path to the Python executable and script
            string pythonPath = "C:\\Program Files (x86)\\Microsoft Visual Studio\\Shared\\Python39_64\\python";  // Replace with the actual path to your Python executable
            string scriptPath = "F:\\CRAS\\CRAS_Sample\\alpha\\src\\test_models.py";  // Replace with the actual path to your Python script

            // Command-line arguments to pass to the Python script
            string arguments = $"\"{scriptPath}\" {"--vidsrc " + cameraId}";

            // Create a new Python process
            Process pythonProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pythonPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            if (cameraId == "0")
            {
                pythonProcess1 = pythonProcess;
                pythonProcess1.Start();
            }

            else
            {
                pythonProcess2 = pythonProcess;
                pythonProcess2.Start();
            }
            
        }

        private void StopPythonScript()
        {
            // Stop the Python process if it is running
            if (pythonProcess1 != null && !pythonProcess1.HasExited)
            {
                pythonProcess1.Kill();
                pythonProcess1.Dispose();
            }

            if (pythonProcess2 != null && !pythonProcess2.HasExited)
            {
                pythonProcess2.Kill();
                pythonProcess2.Dispose();
            }
        }

        private void ClosePipes()
        {
            pipeThread1?.Abort();
            pipeClient1?.Dispose();
            pipeThread2?.Abort();
            pipeClient2?.Dispose();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop the thread and close the named pipe when the form is closing
            //StopPythonScript();
            ClosePipes();
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
            if (startStopFeedButton.Text.ToString().Equals("Start Live Feed"))
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
                
            }

            else
            {
                //StopPythonScript();
                //ClosePipes();
                pictureBox1.Enabled = false; 
                pictureBox2.Enabled = false;

                cam1StatusLabel.Text = "inactive";
                cam1StatusLabel.ForeColor = Color.Red;
                cam2StatusLabel.Text = "inactive";
                cam2StatusLabel.ForeColor = Color.Red;

                startStopFeedButton.Text = "Start Live Feed";
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
            PopulateCustomerFlowLayout();
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

    }
}
