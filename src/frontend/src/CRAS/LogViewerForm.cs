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
    public partial class LogViewerForm : Form
    {
        public DataTable logData;
        public BindingSource bindingSource;
        public LogViewerForm()
        {
            InitializeComponent();
            

            this.Icon = Resources.log_icon1;
        }

        private void LogViewerForm_Load(object sender, EventArgs e)
        {
            fromDate.Format = DateTimePickerFormat.Custom;
            fromDate.CustomFormat = "dd/MM/yyyy HH:mm";
            fromDate.Value = DateTime.Today;

            toDate.Format = DateTimePickerFormat.Custom;
            toDate.CustomFormat = "dd/MM/yyyy HH:mm";
            toDate.Value = DateTime.Now;

            bindingSource = new BindingSource();
            logData = new DataTable();
            //logDataGrid.Rows.Clear();
            logData = pgsql_utilities.GetTableData(pgsql_utilities.ConnectToPGSQL(), "log", "", "ORDER BY logtime", "LIMIT 1000");

            bindingSource.DataSource = logData;
            //logDataGrid.DataSource = bindingSource;
            logAdvancedGrid.DataSource = bindingSource;

            /*foreach(DataColumn column in logDataGrid.Columns)
            {

            }     */       
            
        }

        public void getLogButton_Click(object sender, EventArgs e)
        {
            string fromDateValue = fromDate.Value.ToString("yyyy/MM/dd HH:mm");
            string toDateValue = toDate.Value.ToString("yyyy/MM/dd HH:mm");

            logData.Rows.Clear();
            logData = pgsql_utilities.GetTableData(pgsql_utilities.ConnectToPGSQL(), "log", $"WHERE logtime BETWEEN '{fromDateValue}' AND '{toDateValue}'");
            bindingSource.DataSource = logData;
            //logDataGrid.DataSource = bindingSource;
            logAdvancedGrid.DataSource = bindingSource;
        }

        public void UpdateLog(string fromDate, string toDate = "")
        {
            string fromDateValue = fromDate;

            string toDateValue;

            if (toDate == "") toDateValue = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            else toDateValue = toDate;

            logData.Rows.Clear();
            logData = pgsql_utilities.GetTableData(pgsql_utilities.ConnectToPGSQL(), "log", $"WHERE logtime BETWEEN '{fromDateValue}' AND '{toDateValue}'");
            bindingSource.DataSource = logData;
            //logDataGrid.DataSource = bindingSource;
            logAdvancedGrid.DataSource = bindingSource;
        }

        private void logAdvancedGrid_FilterStringChanged(object sender, EventArgs e)
        {
            bindingSource.Filter = logAdvancedGrid.FilterString; 
        }

        private void logAdvancedGrid_SortStringChanged(object sender, EventArgs e)
        {
            bindingSource.Sort = logAdvancedGrid.SortString;
        }
    }
}
