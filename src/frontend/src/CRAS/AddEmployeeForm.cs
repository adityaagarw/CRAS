﻿using CRAS.Properties;
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
    public partial class AddEmployeeForm : Form
    {
        public string source;
        public string customerId = "";
        public CustomerDataUC customerDataUC;
        //public AddEmployeeForm(string openedBy, string id = "", Image image = null)
        public AddEmployeeForm(string openedBy, CustomerDataUC customer = null)
        {
            InitializeComponent();
            source = openedBy;



            if(source.Equals("MarkAsEmployee"))
            {
                customerDataUC = customer;
                employeePicture.Enabled = false;
                employeePicture.Image = customerDataUC.customerPictureBox.Image;
                customerId = customerDataUC.label4value.Text.ToString();
                nameTextBox.Text = customer.label1value.Text.ToString();
                mobileTextBox.Text = customer.label2value.Text.ToString();

                this.Text = ("Mark As Employee");
                //this.Icon = new Icon("Resources/MarkAsEmployee.ico");
                this.Icon = Resources.MarkAsEmployee;
                addEmployeeButton.Text = "Mark As Employee";
            }

            else if(source.Equals("AddNewEmployee"))
            {
                this.Text = ("Add New Employee");
                this.Icon = Resources.AddEmployee;
            }
        }

        private void employeePicture_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "JPEG Files|*.jpg;*.jpeg|All Files|*.*";
                openFileDialog.Title = "Select a JPEG File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected file path
                    string selectedFilePath = openFileDialog.FileName;

                    // Display the selected image in the PictureBox
                    employeePicture.Image = Image.FromFile(selectedFilePath);

                    // Call the Python script with the selected file path
                    //string faceEncoding = CallPythonScript(selectedFilePath);

                    // Do something with the face encoding (store it in a variable, display, etc.)
                    // For example: textBox1.Text = faceEncoding;
                }
            }
        }

        private void addEmployeeButton_Click(object sender, EventArgs e)
        {
            addEmployeeButton.Enabled = false;
            this.ControlBox = false;

            byte[] imageBytes = utilities.ImagetoByte(employeePicture.Image);

            //TBD : Resize image
            string imageString = Convert.ToBase64String(imageBytes);

            if (source.Equals("AddNewEmployee"))
            {
                string message = "NewEmployee:" + imageString + "," + nameTextBox.Text.ToString() + "," + mobileTextBox.Text.ToString();

                pubsub_utilities.PublishMessage("Employee", message);
            }
            
            else if(source.Equals("MarkAsEmployee"))
            {
                string message = "MarkAsEmployee:" + customerId + "," + nameTextBox.Text.ToString() + "," + mobileTextBox.Text.ToString();

                pubsub_utilities.PublishMessage("Employee", message);
            }
            /*foreach (byte b in imageBytes)
            {
                Console.Write($"{b:X2} ");
            }*/

            

        }

        private void AddEmployeeForm_Load(object sender, EventArgs e)
        {

        }

        public void Reset()
        {
            employeePicture.Image = null;
            employeeIDTextbox.Text = string.Empty;
            nameTextBox.Text = string.Empty;
            mobileTextBox.Text = string.Empty;
            idNumberText.Text = string.Empty;

        }
    }
}
