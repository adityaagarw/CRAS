using CRAS.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRAS
{
    public partial class LoadingForm : Form
    {
        public Timer timer = new Timer();

        public LoadingForm()
        {
            InitializeComponent();
        }

        public Label GetLoadingLabel()
        {
            Label label = loadingLabel;
            return label;
        }

        public void SetLoadingLabel(string labelText)
        {

            loadingLabel.Invoke(new Action(() => { loadingLabel.Text = labelText; }));
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void LoadingForm_Load(object sender, EventArgs e)
        {

            MainForm.backend_status = MainForm.GetBackendStatus();
            //backendString.Text = MainForm.backend_status;
            DisplayIndividualStatus(MainForm.backend_status);
            //StartTimer();

        }

        public void StartTimer()
        {
            //intervalTextbox.ReadOnly = true;
            int interval = 1000;
            //int.TryParse(intervalTextbox.Text.ToString(), out interval);
            //Timer to get backend status every 10 seconds
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            //Get backend status
            MainForm.backend_status = MainForm.GetBackendStatus();

            //Live_Status liveStatus = MainGetLiveStatusFormIfOpen();
            //if (liveStatus != null) liveStatus.refreshButton.PerformClick();
            LoadingForm_Load(sender, e);
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
            Dictionary<int, PictureBox> modulePictureBox = new Dictionary<int, PictureBox>
        {
            { entryBitmask, entryPictureBox },
            { exitBitmask, exitPictureBox },
            { billingBitmask, billingPictureBox },
            { employeeBitmask, employeePictureBox },
            { backendBitmask, backendPictureBox },
            { startingBitmask, startingPictureBox },
            { shutdownSystemBitmask, shutdownPictureBox }
        };

            // Iterate through each character in the status string
            for (int i = statusString.Length - 1; i >= 0; i--)
            {
                int bitmask = 1 << statusString.Length - i - 1;

                // Check if the module is running or not
                bool isRunning = (statusString[i] == '1');


                // Get the corresponding label
                if (modulePictureBox.ContainsKey(bitmask))
                {
                    //Label label = moduleLabels[bitmask];
                    PictureBox pictureBox = modulePictureBox[bitmask];
                    pictureBox.Image = isRunning ? Resources.green_tick_2 : Resources.red_cross_2;
                    //label.Text = isRunning ? "Running" : "Not Running";
                    //label.ForeColor = isRunning ? System.Drawing.Color.Green : System.Drawing.Color.Red;
                    //Console.WriteLine($"{label}: {(isRunning ? "Running" : "Not Running")}");

                }
            }
        }
    }
}
