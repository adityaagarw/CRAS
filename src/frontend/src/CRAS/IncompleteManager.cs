using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace CRAS
{
    public partial class IncompleteManager : Form
    {
        public IncompleteManager()
        {
            InitializeComponent();
        }

        private void IncompleteManager_Load(object sender, EventArgs e)
        {
            BindingList<visit_details> visits = pgsql_utilities.GetVisitDetails(MainForm.pgsql_connection, "incomplete = '1'");
            
            string customer_ids = GetCustomerIdList(visits);

            
            BindingList<redis_customer> customers = pgsql_utilities.GetCustomerDetails(MainForm.pgsql_connection, $"customer_id in {customer_ids}");

            var mergedList = from visit in visits
                             join customer in customers on visit.customer_id equals customer.customer_id
                             select new
                             {
                                 //visit.key,
                                 visit.customer_id,
                                 visit.visit_id,
                                 visit.store_id,
                                 visit.entry_time,
                                 visit.exit_time,
                                 visit.billed,
                                 visit.bill_no,
                                 visit.bill_date,
                                 visit.bill_amount,
                                 visit.return_amount,
                                 visit.time_spent,
                                 visit.visit_remark,
                                 visit.customer_rating,
                                 visit.customer_feedback,
                                 visit.incomplete,
                                 Image = utilities.BytetoImage(customer.image) // Convert byte array to Image
                             };

            Console.WriteLine("Merged Incomplete list size: " + mergedList.Count());

            if (visits.Count > 0 )
            {
                AddColumnsToDataGridView();


                //incompleteDataGrid.DataSource = mergedList;
                
                foreach(var item in mergedList)
                {
                    incompleteDataGrid.Rows.Add(
             item.Image,
             item.customer_id,
             item.visit_id,
             item.store_id,
             item.entry_time,
             item.exit_time,
             item.billed,
             item.bill_no,
             item.bill_date,
             item.bill_amount,
             item.return_amount,
             item.time_spent,
             item.visit_remark,
             item.customer_rating,
             item.customer_feedback,
             item.incomplete);
                   

                }

            }
        }


        public string GetCustomerIdList(BindingList<visit_details> visits)
        {
            string list = "(";

            foreach(visit_details visit in visits)
            {
                list += $"'{visit.customer_id}'" + ",";
            }

            list = list.Substring(0, list.Length - 1);
            list += ")";

            return list;
        }

        private void AddColumnsToDataGridView()
        {
            // Add the image column
            DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
            imageColumn.HeaderText = "Image";
            imageColumn.Name = "image";
            incompleteDataGrid.Columns.Add(imageColumn);
            incompleteDataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            
            // Add columns for visit_details properties
            //incompleteDataGrid.Columns.Add("key", "Key");
            incompleteDataGrid.Columns.Add("customer_id", "Customer ID");
            incompleteDataGrid.Columns.Add("visit_id", "Visit ID");
            incompleteDataGrid.Columns.Add("store_id", "Store ID");
            incompleteDataGrid.Columns.Add("entry_time", "Entry Time");
            incompleteDataGrid.Columns.Add("exit_time", "Exit Time");
            incompleteDataGrid.Columns.Add("billed", "Billed");
            incompleteDataGrid.Columns.Add("bill_no", "Bill No");
            incompleteDataGrid.Columns.Add("bill_date", "Bill Date");
            incompleteDataGrid.Columns.Add("bill_amount", "Bill Amount");
            incompleteDataGrid.Columns.Add("return_amount", "Return Amount");
            incompleteDataGrid.Columns.Add("time_spent", "Time Spent");
            incompleteDataGrid.Columns.Add("visit_remark", "Visit Remark");
            incompleteDataGrid.Columns.Add("customer_rating", "Customer Rating");
            incompleteDataGrid.Columns.Add("customer_feedback", "Customer Feedback");
            incompleteDataGrid.Columns.Add("incomplete", "Incomplete");
        }
    }
}
