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

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Live_Status_Load(sender, e);
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            Live_Status_Load(sender, e);
        }
    }
}
