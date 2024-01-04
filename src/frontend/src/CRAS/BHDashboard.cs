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
    public partial class BHDashboard : Form
    {
        public BHDashboard()
        {
            InitializeComponent();
        }

        private void dailyDatePicker_ValueChanged(object sender, EventArgs e)
        {
            PopulateStatistics(dailyDatePicker.Value.Date);
        }

        public void PopulateStatistics(DateTime date)
        {
            DataTable overview = new DataTable();
            overview = pgsql_utilities.GetDailyOverview(MainForm.pgsql_connection, date.ToString("MM-dd-yyyy"));

            int totalTime = 0;
            int.TryParse(overview.Rows[0]["TotalTime"].ToString(),out totalTime);

            int maxTime = 0;
            int.TryParse(overview.Rows[0]["MaxTime"].ToString(), out maxTime);

            int minTime = 0;
            int.TryParse(overview.Rows[0]["MinTime"].ToString(), out minTime);

            int totalVisits = 0;
            int.TryParse(overview.Rows[0]["TotalVisits"].ToString(), out totalVisits);

            int uniqueVisitors = 0;
            int.TryParse(overview.Rows[0]["UniqueVisitors"].ToString(), out uniqueVisitors);

            int returnVisitors = 0;
            int.TryParse(overview.Rows[0]["ReturnCustomers"].ToString(), out  returnVisitors);

            int newVisitors = 0;
            newVisitors = uniqueVisitors - returnVisitors;

            int avgTime = 0;
            if( totalVisits > 0) avgTime = totalTime / totalVisits;

            totalTimeSpentLabel.Text = (totalTime/60.0).ToString() + " Minutes";
            avgTimeSpentLabel.Text = (avgTime/60.0).ToString() + " Minutes";
            maxTimeSpentLabel.Text = (maxTime/60.0).ToString() + " Minutes";
            minTimeSpentLabel.Text = (minTime/60.0).ToString() + " Minutes";
            totalVisitsLabel.Text = totalVisits.ToString();
            totalUniqueVisitorsLabel.Text = uniqueVisitors.ToString();
            totalRepeatVisitorsLabel.Text = returnVisitors.ToString();
            totalNewVisitorsLabel.Text = newVisitors.ToString();

        }

        private void dailyOverviewTab_Enter(object sender, EventArgs e)
        {
            PopulateStatistics(dailyDatePicker.Value.Date);
        }
    }
}
