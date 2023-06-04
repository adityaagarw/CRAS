using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Threading;
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
        BindingList<redis_customer> customer_list;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //StartPythonScript("0");

            //StartPythonScript("1");
            
            //LoadingForm loadingForm = new LoadingForm();
            //loadingForm.Show();
            //await Task.Run(() => redisConnection = redis_utilities.ConnectToRedis("127.0.0.11"));
            
            redisConnection = redis_utilities.ConnectToRedis("127.0.0.1");

            customer_list = redis_utilities.ReadAllDataFromRedis(redisConnection);
            dataGridView1.DataSource = customer_list;
        }

        public void DisplayDataInDataGridView(BindingList<redis_customer> customer_list)
        {
            // Assuming you have a DataGridView control named "dataGridView1" on your form

            // Clear existing rows and columns in the DataGridView
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            
            // Check if there is any data to display
            if (customer_list.Count == 0)
                return;
            dataGridView1.DataSource = customer_list;

            /* 
            // Create columns in the DataGridView based on the fields in the first dictionary
            foreach (var field in dataList[0].Keys)
            {

                dataGridView1.Columns.Add(field, field);
            }

            // Add rows to the DataGridView with the data from each dictionary
            foreach (var dataDict in dataList)
            {
                DataGridViewRow row = new DataGridViewRow();
                foreach (var field in dataDict.Keys)
                {
                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                    cell.Value = dataDict[field];
                    row.Cells.Add(cell);
                }
                dataGridView1.Rows.Add(row);
            }*/
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
            pipeClient1.Close();
            pipeClient2.Close();
            pipeThread1?.Abort();
            pipeClient1?.Close();
            pipeThread2?.Abort();
            pipeClient2?.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop the thread and close the named pipe when the form is closing
            StopPythonScript();
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
            DisplayDataInDataGridView(customer_list);
        }

        private redis_customer UpdateCustomerRecord(string customer_id, string column_name, string new_value)
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

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            int columnIndex = e.ColumnIndex;

            string column_name = dataGridView1.Columns[columnIndex].Name.ToString();
            string new_value = dataGridView1.Rows[rowIndex].Cells[columnIndex].Value.ToString();
            string customer_id = dataGridView1.Rows[rowIndex].Cells["customer_id"].Value.ToString();
            string key = dataGridView1.Rows[rowIndex].Cells["key"].Value.ToString();

            UpdateCustomerRecord(customer_id, column_name, new_value);
            redis_utilities.UpdateRedisRecord(key, column_name, new_value, redisConnection);

        }
    }
}
