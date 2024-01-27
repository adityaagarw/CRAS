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
using Tmds.DBus.Protocol;
using LiveCharts;
using LiveCharts.Wpf;
using OfficeOpenXml;

namespace CRAS
{
    public partial class BHDashboard : Form
    {
        public class BHData
        {
            public int totalTime;
            public int avgTime;
            public int maxTime;
            public int minTime;
            public int totalVisits;
            public int uniqueVisitors;
            public int returnVisitors;
            public int newVisitors;

            public BHData(int totalTime, int avgTime, int maxTime, int minTime, int totalVisits, int uniqueVisitors, int returnVisitors, int newVisitors)
            {
                this.totalTime = totalTime;
                this.avgTime = avgTime;
                this.maxTime = maxTime;
                this.minTime = minTime;
                this.totalVisits = totalVisits;
                this.uniqueVisitors = uniqueVisitors;
                this.returnVisitors = returnVisitors;
                this.newVisitors = newVisitors;
            }
        }
        public BHDashboard()
        {
            InitializeComponent();

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            dailyOverviewTimeChart.AxisX.Add(new Axis
            {
                Title = "Values",
                LabelFormatter = value => value.ToString()


            });

            dailyOverviewTimeChart.AxisY.Add(new Axis
            {
                Title = "Days",
                Labels = new[] { "Today", "3-day", "7-day", "30-day" }
            });

            dailyOverviewVisitChart.AxisX.Add(new Axis
            {
                Title = "Values",
                LabelFormatter = value => value.ToString()


            });

            dailyOverviewVisitChart.AxisY.Add(new Axis
            {
                Title = "Days",
                Labels = new[] { "Today", "3-day", "7-day", "30-day" }
            });
        }

        private void datePicker_ValueChanged(object sender, EventArgs e)
        {
            PopulateStatistics_Historical(fromDatePicker.Value.Date, toDatePicker.Value.Date);
        }

        public BHData getVisitorData(DateTime fromDate, DateTime toDate)
        {

            DataTable overview = new DataTable();
            DataTable avgOverview = new DataTable();
            overview = pgsql_utilities.GetDailyOverview(MainForm.pgsql_connection, fromDate.ToString("MM-dd-yyyy"), toDate.ToString("MM-dd-yyyy"));
            int totalTime;
            int avgTime = 0;
            int maxTime = 0;
            int minTime = 0;
            int totalVisits = 0;
            int uniqueVisitors = 0;
            int returnVisitors = 0;
            int newVisitors = 0;

            int.TryParse(overview.Rows[0]["TotalTime"].ToString(), out totalTime);
            int.TryParse(overview.Rows[0]["MaxTime"].ToString(), out maxTime);
            int.TryParse(overview.Rows[0]["MinTime"].ToString(), out minTime);
            int.TryParse(overview.Rows[0]["TotalVisits"].ToString(), out totalVisits);
            int.TryParse(overview.Rows[0]["UniqueVisitors"].ToString(), out uniqueVisitors);
            int.TryParse(overview.Rows[0]["ReturnCustomers"].ToString(), out returnVisitors);

            newVisitors = uniqueVisitors - returnVisitors;
            if (totalVisits > 0) avgTime = totalTime / totalVisits;
            BHData data = new BHData(totalTime, avgTime, maxTime, minTime, totalVisits, uniqueVisitors, returnVisitors, newVisitors);
            return data;
        }

        public void PopulateStatistics_Daily(DateTime fromDate, DateTime toDate)
        {
            DataTable overview = new DataTable();
            DataTable avgOverview = new DataTable();
            BHData dailyData = getVisitorData(fromDate, toDate);
            BHData avgData = getVisitorData(DateTime.MinValue, DateTime.MaxValue);
            BHData threeDayData = getVisitorData(fromDate.AddDays(-3), toDate);
            BHData sevenDayData = getVisitorData(fromDate.AddDays(-7), toDate);
            BHData thirtyDayData = getVisitorData(fromDate.AddDays(-30), toDate);

            totalTimePB.Image = dailyData.totalTime > avgData.totalTime ? Resources.green_triangle : Resources.red_triangle;
            maxTimePB.Image = dailyData.maxTime > avgData.maxTime ? Resources.green_triangle : Resources.red_triangle;
            minTimePB.Image = dailyData.minTime > avgData.minTime ? Resources.green_triangle : Resources.red_triangle;
            avgTimePB.Image = dailyData.avgTime > avgData.avgTime ? Resources.green_triangle : Resources.red_triangle;

            totalVisitsPB.Image = dailyData.totalVisits > avgData.totalVisits ? Resources.green_triangle : Resources.red_triangle;
            uniqueVisitorsPB.Image = dailyData.uniqueVisitors > avgData.uniqueVisitors ? Resources.green_triangle : Resources.red_triangle;
            repeatVisitorsPB.Image = dailyData.returnVisitors > avgData.returnVisitors ? Resources.green_triangle : Resources.red_triangle;
            newVisitorsPB.Image = dailyData.newVisitors > avgData.newVisitors ? Resources.green_triangle : Resources.red_triangle;


            totalTimeSpentLabelDaily.Text = Math.Round((dailyData.totalTime /60.0),2).ToString();
            avgTimeSpentLabelDaily.Text = Math.Round((dailyData.avgTime /60.0),2).ToString();
            maxTimeSpentLabelDaily.Text = Math.Round((dailyData.maxTime / 60.0), 2).ToString();
            minTimeSpentLabelDaily.Text = Math.Round((dailyData.minTime / 60.0), 2).ToString();
            totalVisitsLabelDaily.Text = dailyData.totalVisits.ToString();
            totalUniqueVisitorsLabelDaily.Text = dailyData.uniqueVisitors.ToString();
            totalRepeatVisitorsLabelDaily.Text = dailyData.returnVisitors.ToString();
            totalNewVisitorsLabelDaily.Text = dailyData.newVisitors.ToString();

            dailyData.totalTime = 400;
            dailyData.avgTime = 30;
            dailyData.maxTime = 60;
            dailyData.minTime = 10;

            threeDayData.totalTime = 700;
            threeDayData.avgTime = 100;
            threeDayData.maxTime = 150;
            threeDayData.minTime = 50;

            sevenDayData.totalTime = 300;
            sevenDayData.avgTime = 40;
            sevenDayData.maxTime = 60;
            sevenDayData.minTime = 20;

            thirtyDayData.totalTime = 200;
            thirtyDayData.avgTime = 30;
            thirtyDayData.maxTime = 60;
            thirtyDayData.minTime = 10;

            dailyData.totalVisits = 50;
            dailyData.uniqueVisitors = 45;
            dailyData.returnVisitors = 5;
            dailyData.newVisitors = 40;

            threeDayData.totalVisits = 60;
            threeDayData.uniqueVisitors = 60;
            threeDayData.returnVisitors = 50;
            threeDayData.newVisitors = 10;

            sevenDayData.totalVisits = 40;
            sevenDayData.uniqueVisitors = 39;
            sevenDayData.returnVisitors = 1;
            sevenDayData.newVisitors = 38;

            thirtyDayData.totalVisits = 48;
            thirtyDayData.uniqueVisitors = 45;
            thirtyDayData.returnVisitors = 3;
            thirtyDayData.newVisitors = 42;

            dailyOverviewTimeChart.Series.Clear();
            LiveCharts.SeriesCollection timeSeries = new LiveCharts.SeriesCollection
            {
                new StackedRowSeries
                {
                    Title = "Minimum Time",
                    Values = new LiveCharts.ChartValues<int> {dailyData.minTime, threeDayData.minTime , sevenDayData.minTime , thirtyDayData.minTime }
                },
                new StackedRowSeries
                {
                    Title = "Average Time",
                    Values = new LiveCharts.ChartValues<int> {dailyData.avgTime, threeDayData.avgTime , sevenDayData.avgTime , thirtyDayData.avgTime }
                },
                new StackedRowSeries
                {
                    Title = "Maximum Time",
                    Values = new LiveCharts.ChartValues<int> {dailyData.maxTime, threeDayData.maxTime , sevenDayData.maxTime , thirtyDayData.maxTime }
                },
                new StackedRowSeries
                {
                    Title = "Total Time",
                    Values = new LiveCharts.ChartValues<int> {dailyData.totalTime, threeDayData.totalTime , sevenDayData.totalTime , thirtyDayData.totalTime }
                }
            };
            dailyOverviewTimeChart.Series = timeSeries;

            dailyOverviewVisitChart.Series.Clear();
            LiveCharts.SeriesCollection visitSeries = new LiveCharts.SeriesCollection
            {
                new StackedRowSeries
                {
                    Title = "Return Visitors",
                    Values = new LiveCharts.ChartValues<int> {dailyData.returnVisitors, threeDayData.returnVisitors, sevenDayData.returnVisitors, thirtyDayData.returnVisitors }
                },
                new StackedRowSeries
                {
                    Title = "New Visitors",
                    Values = new LiveCharts.ChartValues<int> {dailyData.newVisitors, threeDayData.newVisitors, sevenDayData.newVisitors, thirtyDayData.newVisitors }
                },
                new StackedRowSeries
                {
                    Title = "Unique Visitors",
                    Values = new LiveCharts.ChartValues<int> {dailyData.uniqueVisitors, threeDayData.uniqueVisitors, sevenDayData.uniqueVisitors, thirtyDayData.uniqueVisitors }
                },
                new StackedRowSeries
                {
                    Title = "Total Visitors",
                    Values = new LiveCharts.ChartValues<int> {dailyData.totalVisits, threeDayData.totalVisits, sevenDayData.totalVisits, thirtyDayData.totalVisits }
                }
            };

            dailyOverviewVisitChart.Series = visitSeries;


        }

        public void PopulateStatistics_Historical(DateTime fromDate, DateTime toDate)
        {
            DataTable overview = new DataTable();
            DataTable avgOverview = new DataTable();
            overview = pgsql_utilities.GetDailyOverview(MainForm.pgsql_connection, fromDate.ToString("MM-dd-yyyy"), toDate.ToString("MM-dd-yyyy"));
            BHData historicalData = getVisitorData(fromDate, toDate);
            avgOverview = pgsql_utilities.GetDailyOverview(MainForm.pgsql_connection, DateTime.MinValue.ToString("MM-dd-yyyy"), DateTime.MaxValue.ToString("MM-dd-yyyy"));

            int totalTime = 0;
            int.TryParse(overview.Rows[0]["TotalTime"].ToString(), out totalTime);

            int maxTime = 0;
            int.TryParse(overview.Rows[0]["MaxTime"].ToString(), out maxTime);

            int minTime = 0;
            int.TryParse(overview.Rows[0]["MinTime"].ToString(), out minTime);

            int totalVisits = 0;
            int.TryParse(overview.Rows[0]["TotalVisits"].ToString(), out totalVisits);

            int uniqueVisitors = 0;
            int.TryParse(overview.Rows[0]["UniqueVisitors"].ToString(), out uniqueVisitors);

            int returnVisitors = 0;
            int.TryParse(overview.Rows[0]["ReturnCustomers"].ToString(), out returnVisitors);

            int newVisitors = 0;
            newVisitors = uniqueVisitors - returnVisitors;

            int avgTime = 0;
            if (totalVisits > 0) avgTime = totalTime / totalVisits;

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

            totalTimePBHistorical.Image = totalTime > avgTotalTime ? Resources.green_triangle : Resources.red_triangle;
            maxTimePBHistorical.Image = maxTime > avgMaxTime ? Resources.green_triangle : Resources.red_triangle;
            minTimePBHistorical.Image = minTime > avgMinTime ? Resources.green_triangle : Resources.red_triangle;
            avgTimePBHistorical.Image = avgTime > avgAvgTime ? Resources.green_triangle : Resources.red_triangle;

            totalVisitsPBHistorical.Image = totalVisits > avgTotalVisits ? Resources.green_triangle : Resources.red_triangle;
            uniqueVisitorsPBHistorical.Image = uniqueVisitors > avgUniqueVisitors ? Resources.green_triangle : Resources.red_triangle;
            repeatVisitorsPBHistorical.Image = returnVisitors > avgReturnVisitors ? Resources.green_triangle : Resources.red_triangle;
            newVisitorsPBHistorical.Image = newVisitors > avgNewVisitors ? Resources.green_triangle : Resources.red_triangle;


            totalTimeSpentLabelHistorical.Text = Math.Round((totalTime / 60.0), 2).ToString();
            avgTimeSpentLabelHistorical.Text = Math.Round((avgTime / 60.0), 2).ToString();
            maxTimeSpentLabelHistorical.Text = Math.Round((maxTime / 60.0), 2).ToString();
            minTimeSpentLabelHistorical.Text = Math.Round((minTime / 60.0), 2).ToString();
            totalVisitsLabelHistorical.Text = totalVisits.ToString();
            totalUniqueVisitorsLabelHistorical.Text = uniqueVisitors.ToString();
            totalRepeatVisitorsLabelHistorical.Text = returnVisitors.ToString();
            totalNewVisitorsLabelHistorical.Text = newVisitors.ToString();

            string[] categories = { "Time Spent", "Visitors" };
            int[] min = { minTime, uniqueVisitors };
            int[] max = { maxTime, returnVisitors };
            int[] avg = { avgTime, newVisitors };
            int[] total = { totalTime, totalVisits };
        }

        private void dailyOverviewTab_Enter(object sender, EventArgs e)
        {
            DateTime today = DateTime.Now.Date;
            dateLabel.Text = today.ToString("dddd, MMMM dd, yyyy");
            PopulateStatistics_Daily(today, today);
        }
        private void historicalTab_Enter(object sender, EventArgs e)
        {
            PopulateStatistics_Historical(fromDatePicker.Value.Date, toDatePicker.Value.Date);
        }

        private void importDataButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select Excel File";
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFileName = openFileDialog.FileName;

                // Check if the required columns exist in the Excel file
                string errorColumns = CheckExcelColumns(selectedFileName);
                if (errorColumns.Equals(""))
                {
                    // The file contains the required columns, perform your operations here
                    bool success = pgsql_utilities.InsertExcelSaleData(MainForm.pgsql_connection, selectedFileName);
                    MessageBox.Show("Inserted into DB", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);


                }
                else
                {
                    // The file does not contain the required columns
                    MessageBox.Show("File does not contain these required columns " + errorColumns);
                }
            }
        }

        private string CheckExcelColumns(string filePath)
        {
            try
            {
                using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
                {
                    string errorColumns = "";
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                    if (worksheet != null)
                    {
                        // Define the required column names
                        string[] requiredColumns = { "Date", "Total Sale", "Total Return", "Net Sale", "Sale Qty", "Return Qty", "Net Sale Qty", "Total Invoices" };

                        // Check if all required columns are present in the worksheet
                        foreach (var columnName in requiredColumns)
                        {
                            if (worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns].Any(cell => string.Equals(cell.Text.Trim(), columnName, StringComparison.OrdinalIgnoreCase)) == false)
                            {
                                errorColumns += ": " + columnName;
                                //return columnName;
                            }
                        }

                        // All required columns are present
                        return errorColumns;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }

            return "";
        }
    }
}
