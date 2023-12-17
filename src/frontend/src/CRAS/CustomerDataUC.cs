using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CRAS
{
    public partial class CustomerDataUC : UserControl
    {
        public event EventHandler<int> ControlClicked;
        public event EventHandler<int> ControlDoubleClicked;
        public event EventHandler<int> DeleteButtonClicked;
        public event EventHandler<int> MarkAsEmployeeClicked;

        public bool isSelected { get; set; }
        public CustomerDataUC()
        {
            InitializeComponent();

        }

        private void CustomerDataUC_Load(object sender, EventArgs e)
        {
            isSelected = false;
            customerPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        public void SetImage(byte[] image)
        {
            if (image !=  null) customerPictureBox.Image = utilities.BytetoImage(image);
            else customerPictureBox.Image = null;
        }
        public void SetLabel(string labelName, string field, string value)
        {
            foreach (Label label in Controls.OfType<Label>())
            {
                if (label.Name.Equals(labelName)) label.Text = field;
                if (label.Name.Equals(labelName + "value")) label.Text = value;
            }
        }

        private void CustomerDataUC_Click(object sender, EventArgs e)
        {

        }


        public void Select()
        {
            isSelected = true;
            this.BorderStyle = BorderStyle.Fixed3D;
            this.BackColor = Color.LightYellow;
        }

        public void UnSelect()
        {
            isSelected = false;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.BackColor = Color.White;
        }

        public static void UnselectAllControls(FlowLayoutPanel panel)
        {
            foreach (CustomerDataUC customer in panel.Controls)
            {
                customer.UnSelect();
            }
        }

        private void CustomerDataUC_DoubleClick(object sender, EventArgs e)
        {
            int index = Parent.Controls.IndexOf(this);

            ControlDoubleClicked?.Invoke(this, index);
        }

        public void SetState(string state)
        {
            selectCustomerLabel.Text = state;
        }

        private void selectCustomerLabel_TextChanged(object sender, EventArgs e)
        {
            if(selectCustomerLabel.Text.ToLower().Equals("customer selected!"))
            {
                //selectCustomerLabel.Text = "Customer Selected!";
                
                List<Control> controls = Controls.Cast<Control>().ToList();
                
                foreach(Control control in controls)
                {
                    control.Show();
                }

                selectCustomerLabel.Hide();
            }

            else if (selectCustomerLabel.Text.ToLower().Equals("please select a customer!"))
            {
                //selectCustomerLabel.Text = "Please select a Customer!";

                List<Control> controls = Controls.Cast<Control>().ToList();

                foreach (Control control in controls)
                {
                    control.Hide();
                }

                selectCustomerLabel.Show();
            }
        }

        private void CustomerDataUC_MouseHover(object sender, EventArgs e)
        {

        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            DeleteButtonClicked?.Invoke(this, Parent.Controls.IndexOf(this));
        }

        private void markAsEmployee_Click(object sender, EventArgs e)
        {
            MarkAsEmployeeClicked?.Invoke(this, Parent.Controls.IndexOf(this));
        }
    }
}
