using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;



namespace CRAS
{
    public partial class Live_Status : Form
    {
        public Timer timer = new Timer();

        public Live_Status()
        {
            InitializeComponent();
            refreshButton.BackgroundImageLayout = ImageLayout.Zoom;

        }

        private void Live_Status_Load(object sender, EventArgs e)
        {
            String[] listOfPipes = System.IO.Directory.GetFiles(@"\\.\pipe\");
            List<string> pipes = new List<string>();
            
            foreach (String pipe in listOfPipes) 
            { 
                if(pipe.Contains("cam_")) pipes.Add(pipe);
            }
            livePipeListBox.DataSource = pipes;
            
            if(MainForm.redisConnection != null && MainForm.redisConnection.IsConnected)
            {
                redisStatusLabel.Text = "Connected";
                redisStatusLabel.ForeColor = Color.Green;
            }
            else
            {
                redisStatusLabel.Text = "Not Connected";
                redisStatusLabel.ForeColor = Color.Red;
            }

            webCamListBox.Items.Clear();

            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            int index = 0;
            foreach (FilterInfo device in videoDevices)
            {
                webCamListBox.Items.Add(device.Name + " Index: " + index++ );
            }

            MainForm.backend_status = MainForm.GetBackendStatus ();
            backendString.Text = MainForm.backend_status;
            DisplayIndividualStatus(MainForm.backend_status);

        }

        public void DisplayIndividualStatus(string statusString)
        {
            statusString = utilities.StandardiseIntToString(int.Parse(statusString), 7);

            // Define bit masks
            int entryBitmask = 0b0001;
            int exitBitmask = 0b0010;
            int billingBitmask = 0b0100;
            int employeeBitmask = 0b1000;
            int backendBitmask = 0b10000;
            int startingBitmask = 0b100000;
            int shutdownSystemBitmask = 0b1000000;

            // Create a dictionary to map each bitmask to its corresponding label
            Dictionary<int, Label> moduleLabels = new Dictionary<int, Label>
        {
            { entryBitmask, entryLabel },
            { exitBitmask, exitLabel },
            { billingBitmask, billingLabel },
            { employeeBitmask, employeeLabel },
            { backendBitmask, backendLabel },
            { startingBitmask, startingLabel },
            { shutdownSystemBitmask, shutdownLabel }
        };

            // Iterate through each character in the status string
            for (int i = statusString.Length-1; i >= 0; i--)
            {
                int bitmask = 1 << statusString.Length - i -1;

                // Check if the module is running or not
                bool isRunning = (statusString[i] == '1');


                // Get the corresponding label
                if (moduleLabels.ContainsKey(bitmask))
                {
                    Label label = moduleLabels[bitmask];
                    label.Text = isRunning ? "Running" : "Not Running";
                    label.ForeColor = isRunning ? System.Drawing.Color.Green : System.Drawing.Color.Red;
                    //Console.WriteLine($"{label}: {(isRunning ? "Running" : "Not Running")}");

                }
            }

            // Reset console color
            Console.ResetColor();

        }



        public void refreshButton_Click(object sender, EventArgs e)
        {
            Live_Status_Load(sender, e);
        }

        private void liveCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if(liveCheckbox.Checked)
            {
                intervalTextbox.ReadOnly = true;
                int interval = 1000;
                int.TryParse(intervalTextbox.Text.ToString(), out interval);
                //Timer to get backend status every 10 seconds
                timer.Interval = 1000;
                timer.Tick += Timer_Tick;
                timer.Start();
            }

            if(!liveCheckbox.Checked)
            {
                timer.Stop();
                intervalTextbox.ReadOnly= false;
            }

        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            //Get backend status
            MainForm.backend_status = MainForm.GetBackendStatus();

            //Live_Status liveStatus = MainGetLiveStatusFormIfOpen();
            //if (liveStatus != null) liveStatus.refreshButton.PerformClick();
            Live_Status_Load(sender, e);
        }

        private void Live_Status_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Stop();
        }
       
    }
}
