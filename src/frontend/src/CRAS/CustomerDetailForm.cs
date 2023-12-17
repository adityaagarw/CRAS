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
    public partial class CustomerDetailForm : Form
    {
        int name_modified = 0;
        int mobile_modified = 0;
        int remarks_modified = 0;
        redis_customer customer;
        Dictionary<string, string> modifiedFields = new Dictionary<string, string>();
        public CustomerDetailForm(redis_customer data)
        {
            InitializeComponent();
            customer = data;

        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CustomerDetailForm_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = utilities.BytetoImage(customer.image);
            nameTextBox.Text = customer.name;
            mobileTextBox.Text = customer.phone_number;
            lastVisitTextBox.Text = customer.last_visit.ToString("dd-MM-yyyy HH:mm:ss");
            avgTimeTextBox.Text = customer.average_time_spent.ToString();
            avgPurchaseTextBox.Text = customer.average_bill_value.ToString();
            maxPurchasetextBox.Text = customer.maximum_purchase.ToString();
            remarksTextBox.Text = customer.remarks;
            memberSinceLabel.Text = customer.creation_date.ToShortDateString();
            categoryLabel.Text = customer.category;


        }

        

        private void updateButton_Click(object sender, EventArgs e)
        {
            if (name_modified == 1)
            {
                utilities.UpdateCustomerRecord(customer, "name", nameTextBox.Text);
                modifiedFields.Add("name", nameTextBox.Text);
            }

            if (mobile_modified == 1)
            {
                utilities.UpdateCustomerRecord(customer, "phone_number", mobileTextBox.Text);
                modifiedFields.Add("phone_number", mobileTextBox.Text);
            }

            if (remarks_modified == 1)
            {
                utilities.UpdateCustomerRecord(customer, "remarks", remarksTextBox.Text);
                modifiedFields.Add("remarks", remarksTextBox.Text);
            }
            
            if(name_modified == 1 || mobile_modified == 1 || name_modified == 1)
            {
                redis_utilities.UpdateRedisRecord(customer.key, modifiedFields, MainForm.redisConnection);
            }
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            name_modified = 1;
        }

        private void mobileTextBox_TextChanged(object sender, EventArgs e)
        {
            mobile_modified = 1;
        }

        private void remarksTextBox_TextChanged(object sender, EventArgs e)
        {
            remarks_modified = 1;
        }
    }
}
