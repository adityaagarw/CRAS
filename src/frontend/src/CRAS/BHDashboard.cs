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
using System.Windows.Forms.DataVisualization.Charting;
using Tmds.DBus.Protocol;

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
            PopulateStatistics(fromDatePicker.Value.Date, toDatePicker.Value.Date);
        }

        public void PopulateStatistics(DateTime fromDate, DateTime toDate)
        {
            DataTable overview = new DataTable();
            DataTable avgOverview = new DataTable();
            overview = pgsql_utilities.GetDailyOverview(MainForm.pgsql_connection, fromDate.ToString("MM-dd-yyyy"), toDate.ToString("MM-dd-yyyy"));

            avgOverview = pgsql_utilities.GetDailyOverview(MainForm.pgsql_connection, DateTime.MinValue.ToString("MM-dd-yyyy"), DateTime.MaxValue.ToString("MM-dd-yyyy"));

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

            int avgTotalTime = 0;
            int.TryParse(avgOverview.Rows[0]["TotalTime"].ToString(), out avgTotalTime);

            int avgMaxTime = 0;
            int.TryParse(avgOverview.Rows[0]["MaxTime"].ToString(), out avgMaxTime);

            int avgMinTime = 0;
            int.TryParse(avgOverview.Rows[0]["MinTime"].ToString(), out avgMinTime);

            int avgTotalVisits = 0;
            int.TryParse(avgOverview.Rows[0]["TotalVisits"].ToString(), out avgTotalVisits);

            int avgUniqueVisitors = 0;
            int.TryParse(avgOverview.Rows[0]["UniqueVisitors"].ToString(), out avgUniqueVisitors);

            int avgReturnVisitors = 0;
            int.TryParse(avgOverview.Rows[0]["ReturnCustomers"].ToString(), out avgReturnVisitors);

            int avgNewVisitors = 0;
            avgNewVisitors = avgUniqueVisitors - avgReturnVisitors;

            int avgAvgTime = 0;
            if (avgTotalVisits > 0) avgAvgTime = avgTotalTime / avgTotalVisits;

            totalTimePB.Image = totalTime > avgTotalTime ? Resources.green_triangle : Resources.red_triangle;
            maxTimePB.Image = maxTime > avgMaxTime ? Resources.green_triangle : Resources.red_triangle;
            minTimePB.Image = minTime > avgMinTime ? Resources.green_triangle : Resources.red_triangle;
            avgTimePB.Image = avgTime > avgAvgTime ? Resources.green_triangle : Resources.red_triangle;

            totalVisitsPB.Image = totalVisits > avgTotalVisits ? Resources.green_triangle : Resources.red_triangle;
            uniqueVisitorsPB.Image = uniqueVisitors > avgUniqueVisitors ? Resources.green_triangle : Resources.red_triangle;
            repeatVisitorsPB.Image = returnVisitors > avgReturnVisitors ? Resources.green_triangle : Resources.red_triangle;
            newVisitorsPB.Image = newVisitors > avgNewVisitors ? Resources.green_triangle : Resources.red_triangle;


            totalTimeSpentLabel.Text = Math.Round((totalTime/60.0),2).ToString();
            avgTimeSpentLabel.Text = Math.Round((avgTime/60.0),2).ToString();
            maxTimeSpentLabel.Text = Math.Round((maxTime / 60.0), 2).ToString();
            minTimeSpentLabel.Text = Math.Round((minTime / 60.0), 2).ToString();
            totalVisitsLabel.Text = totalVisits.ToString();
            totalUniqueVisitorsLabel.Text = uniqueVisitors.ToString();
            totalRepeatVisitorsLabel.Text = returnVisitors.ToString();
            totalNewVisitorsLabel.Text = newVisitors.ToString();

            string[] categories = { "Time Spent", "Visitors" };
            int[] min = { minTime, uniqueVisitors };
            int[] max = { maxTime, returnVisitors };
            int[] avg = { avgTime, newVisitors };
            int[] total = { totalTime, totalVisits };

            PopulateChart();
        }

        private void dailyOverviewTab_Enter(object sender, EventArgs e)
        {
            PopulateStatistics(fromDatePicker.Value.Date, toDatePicker.Value.Date);
        }

        private void PopulateChart()
        {/*
            // Clear any existing data in the chart
            dailyChart.Series.Clear();
            dailyChart.Legends.Clear();

            // Create a single series for stacked columns
            Series series = new Series("Data");
            series.ChartType = SeriesChartType.StackedColumn;

            // Add data points to the series
            for (int i = 0; i < categories.Length; i++)
            {
                // Add data points for min, max, avg, and sum
                series.Points.AddXY(categories[i], minValues[i]);
                series.Points.AddXY(categories[i], maxValues[i]);
                series.Points.AddXY(categories[i], avgValues[i]);
                series.Points.AddXY(categories[i], totalValues[i]);
            }

            // Add the series to the chart
            dailyChart.Series.Add(series);

            // Customize chart appearance
            dailyChart.ChartAreas[0].AxisX.Interval = 1; // Display each category separately
            dailyChart.ChartAreas[0].AxisX.MajorGrid.Enabled = false; // Disable grid lines for better visibility
            dailyChart.ChartAreas[0].AxisY.Title = "Values";
            dailyChart.Legends.Add(new Legend("Legend"));
            dailyChart.Legends["Legend"].Docking = Docking.Bottom;*/
            // Assuming you have data for categories and corresponding min, max, avg, sum values
            string[] categories = { "Category1", "Category2", "Category3" };
            double[] minValues = { 10, 20, 15 };
            double[] maxValues = { 30, 40, 25 };
            double[] avgValues = { 15, 30, 20 };
            double[] sumValues = { 45, 90, 60 };

            // Clear any existing data in the chart
            dailyChart.Series.Clear();
            dailyChart.Legends.Clear();

            // Create separate series for min, max, avg, and sum
            Series seriesMin = new Series("Min");
            Series seriesMax = new Series("Max");
            Series seriesAvg = new Series("Average");
            Series seriesSum = new Series("Sum");

            // Set chart type to StackedColumn
            seriesMin.ChartType = SeriesChartType.StackedColumn;
            seriesMax.ChartType = SeriesChartType.StackedColumn;
            seriesAvg.ChartType = SeriesChartType.StackedColumn;
            seriesSum.ChartType = SeriesChartType.StackedColumn;

            // Add data points to each series
            for (int i = 0; i < categories.Length; i++)
            {
                seriesMin.Points.AddXY(categories[i], minValues[i]);
                seriesMax.Points.AddXY(categories[i], maxValues[i]);
                seriesAvg.Points.AddXY(categories[i], avgValues[i]);
                seriesSum.Points.AddXY(categories[i], sumValues[i]);
            }

            // Add series to the chart
            dailyChart.Series.Add(seriesMin);
            dailyChart.Series.Add(seriesMax);
            dailyChart.Series.Add(seriesAvg);
            dailyChart.Series.Add(seriesSum);

            // Customize chart appearance
            dailyChart.ChartAreas[0].AxisX.Interval = 1; // Display each category separately
            dailyChart.ChartAreas[0].AxisX.MajorGrid.Enabled = false; // Disable grid lines for better visibility
            dailyChart.ChartAreas[0].AxisY.Title = "Values";
            dailyChart.Legends.Add(new Legend("Legend"));
            dailyChart.Legends["Legend"].Docking = Docking.Bottom;
        }
    }
}
