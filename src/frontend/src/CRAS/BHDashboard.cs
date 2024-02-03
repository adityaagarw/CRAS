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
using DirectShowLib;

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

        public class SalesData
        {
            public double total_sale;
            public double total_return;
            public double net_sale;
            public int sale_qty;
            public int return_qty;
            public int net_sale_qty;
            public int total_invoices;

            public SalesData(double total_sale, double total_return, double net_sale,
                            int sale_qty,  int return_qty, int net_sale_qty,
                            int total_invoices)
            {
                this.total_sale = total_sale;
                this.total_return = total_return;
                this.net_sale = net_sale;
                this.sale_qty = sale_qty;
                this.return_qty = return_qty;
                this.net_sale_qty = net_sale_qty;
                this.total_invoices = total_invoices;
            }
        }
        public BHDashboard()
        {
            InitializeComponent();

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            dailyOverviewTimeChart.AxisX.Add(new Axis
            {
                Title = "Time Spent",
                LabelFormatter = value => value.ToString()


            });

            dailyOverviewTimeChart.AxisY.Add(new Axis
            {
                Title = "Days",
                Labels = new[] { "Today", "3-day", "7-day", "30-day" }
            });

            dailyOverviewVisitChart.AxisX.Add(new Axis
            {
                Title = "Visits",
                LabelFormatter = value => value.ToString()


            });

            dailyOverviewVisitChart.AxisY.Add(new Axis
            {
                Title = "Days",
                Labels = new[] { "Today", "3-day", "7-day", "30-day" }
            });

            historicalTimeChart.AxisX.Add(new Axis
            {
                Title = "Days"
            });
            historicalVisitChart.AxisX.Add(new Axis
            {
                Title = "Days"
            });
            historicalTimeChart.AxisY.Add(new Axis
            {
                Title = "Time Spent"
            });
            historicalVisitChart.AxisY.Add(new Axis
            {
                Title = "Visits"
            });


            dailySaleChart.AxisX.Add(new Axis
            {
                Title = "Sales",
                LabelFormatter = value => value.ToString()


            });

            dailySaleChart.AxisY.Add(new Axis
            {
                Title = "Days",
                Labels = new[] { "Today", "3-day", "7-day", "30-day" }
            });

            dailyQtyChart.AxisX.Add(new Axis
            {
                Title = "Quantity",
                LabelFormatter = value => value.ToString()


            });

            dailyQtyChart.AxisY.Add(new Axis
            {
                Title = "Days",
                Labels = new[] { "Today", "3-day", "7-day", "30-day" }
            });

            historicalSaleChart.AxisX.Add(new Axis
            {
                Title = "Days"
            });
            historicalQtyChart.AxisX.Add(new Axis
            {
                Title = "Days"
            });
            historicalSaleChart.AxisY.Add(new Axis
            {
                Title = "Sales"
            });
            historicalQtyChart.AxisY.Add(new Axis
            {
                Title = "Quantity"
            });
        }

        private void dailyDatePicker_ValueChanged(object sender, EventArgs e)
        {
            PopulateStatistics_Daily(dailyDatePicker.Value.Date);
        }

        private void datePicker_ValueChanged(object sender, EventArgs e)
        {
            PopulateStatistics_Historical(fromDatePicker.Value.Date, toDatePicker.Value.Date);
        }

        private void dailySalesDatePicker_ValueChanged(object sender, EventArgs e)
        {
            PopulateStatistics_DailySales(dailySalesDatePicker.Value.Date);
        }

        private void historicalSalesDatePicker_Value_Changed(object sender, EventArgs e)
        {
            PopulateStatistics_HistoricalSales(fromSalesDatePicker.Value.Date, toSalesDatePicker.Value.Date);
        }

        public SalesData getSalesData(DateTime fromDate, DateTime toDate)
        {
            DataTable overview = new DataTable();
            overview = pgsql_utilities.GetSalesOverview(MainForm.pgsql_connection, fromDate.ToString("MM-dd-yyyy"), toDate.ToString("MM-dd-yyyy"));
            double total_sale = 0;
            double total_return = 0;
            double net_sale = 0;
            int sale_qty = 0;
            int return_qty = 0;
            int net_sale_qty = 0;
            int total_invoices = 0;

            if (overview.Rows.Count == 0) return null;
            double.TryParse(overview.Rows[0]["TotalSale"].ToString(), out total_sale);
            double.TryParse(overview.Rows[0]["TotalReturn"].ToString(), out total_return);
            double.TryParse(overview.Rows[0]["NetSale"].ToString(), out net_sale);
            int.TryParse(overview.Rows[0]["SaleQty"].ToString(), out sale_qty);
            int.TryParse(overview.Rows[0]["ReturnQty"].ToString(), out return_qty);
            int.TryParse(overview.Rows[0]["NetSaleQty"].ToString(), out net_sale_qty);
            int.TryParse(overview.Rows[0]["TotalInvoices"].ToString(), out total_invoices);

            SalesData data = new SalesData(total_sale, total_return, net_sale, sale_qty, return_qty, net_sale_qty, total_invoices);
            return data;
        }

        public BHData getVisitorData(DateTime fromDate, DateTime toDate)
        {

            DataTable overview = new DataTable();
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

        public void PopulateStatistics_Daily(DateTime selectedDate)
        {
            DataTable overview = new DataTable();
            DataTable avgOverview = new DataTable();
            BHData dailyData = getVisitorData(selectedDate, selectedDate);
            BHData avgData = getVisitorData(DateTime.MinValue, DateTime.MaxValue);
            BHData threeDayData = getVisitorData(selectedDate.AddDays(-3), selectedDate);
            BHData sevenDayData = getVisitorData(selectedDate.AddDays(-7), selectedDate);
            BHData thirtyDayData = getVisitorData(selectedDate.AddDays(-30), selectedDate);

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
        private int GetPropertyValue(BHData data, string propertyName)
        {
            // Helper method to retrieve property values from BHData
            switch (propertyName)
            {
                case "totalTime":
                    return data.totalTime;
                case "avgTime":
                    return data.avgTime;
                case "maxTime":
                    return data.maxTime;
                case "minTime":
                    return data.minTime;
                case "totalVisits":
                    return data.totalVisits;
                case "uniqueVisitors":
                    return data.uniqueVisitors;
                case "returnVisitors":
                    return data.returnVisitors;
                case "newVisitors":
                    return data.newVisitors;
                default:
                    return 0;
            }
        }
        public static void PopulateRandomData(BHData[] points)
        {
            Random random = new Random();

            for (int i = 0; i < points.Length; i++)
            {
                int totalTime = random.Next(1, 1000);
                int avgTime = random.Next(1, 100);
                int maxTime = random.Next(1, 1000);
                int minTime = random.Next(1, 100);
                int totalVisits = random.Next(1, 10000);
                int uniqueVisitors = random.Next(1, 1000);
                int returnVisitors = random.Next(1, 500);
                int newVisitors = random.Next(1, 500);

                points[i] = new BHData(totalTime, avgTime, maxTime, minTime, totalVisits, uniqueVisitors, returnVisitors, newVisitors);
            }
        }
        public void PopulateStatistics_Historical(DateTime fromDate, DateTime toDate)
        {
            TimeSpan difference = toDate - fromDate;
            if (fromDate >= toDate) return;
            int numDays = (int)(difference.TotalDays);
            int numPoints = 10;

            if (numDays < numPoints && numDays != 0) numPoints = numDays;
            int interval = numDays / numPoints;
            
            DataTable overview = new DataTable();
            DataTable avgOverview = new DataTable();
            BHData historicalData = getVisitorData(fromDate, toDate);
            BHData avgData = getVisitorData(DateTime.MinValue, DateTime.MaxValue);
            BHData[] points = new BHData[numPoints];

            for (int i = 0; i < numPoints; i++)
            {
                DateTime intervalStartDate = fromDate.AddDays(i * interval);
                DateTime intervalEndDate = fromDate.AddDays((i + 1) * interval - 1);

                if (intervalEndDate > toDate) intervalEndDate = toDate;
                // Retrieve historical data for the current interval
                BHData intervalData = getVisitorData(intervalStartDate, intervalEndDate);

                // Assign the retrieved data to the points array
                points[i] = intervalData;
            }

            totalTimePBHistorical.Image = historicalData.totalTime > avgData.totalTime ? Resources.green_triangle : Resources.red_triangle;
            maxTimePBHistorical.Image = historicalData.maxTime > avgData.maxTime ? Resources.green_triangle : Resources.red_triangle;
            minTimePBHistorical.Image = historicalData.minTime > avgData.minTime ? Resources.green_triangle : Resources.red_triangle;
            avgTimePBHistorical.Image = historicalData.avgTime > avgData.avgTime ? Resources.green_triangle : Resources.red_triangle;

            totalVisitsPBHistorical.Image = historicalData.totalVisits > avgData.totalVisits ? Resources.green_triangle : Resources.red_triangle;
            uniqueVisitorsPBHistorical.Image = historicalData.uniqueVisitors > avgData.uniqueVisitors ? Resources.green_triangle : Resources.red_triangle;
            repeatVisitorsPBHistorical.Image = historicalData.returnVisitors > avgData.returnVisitors ? Resources.green_triangle : Resources.red_triangle;
            newVisitorsPBHistorical.Image = historicalData.newVisitors > avgData.newVisitors ? Resources.green_triangle : Resources.red_triangle;

            totalTimeSpentLabelHistorical.Text = Math.Round((historicalData.totalTime / 60.0), 2).ToString();
            avgTimeSpentLabelHistorical.Text = Math.Round((historicalData.avgTime / 60.0), 2).ToString();
            maxTimeSpentLabelHistorical.Text = Math.Round((historicalData.maxTime / 60.0), 2).ToString();
            minTimeSpentLabelHistorical.Text = Math.Round((historicalData.minTime / 60.0), 2).ToString();
            totalVisitsLabelHistorical.Text = historicalData.totalVisits.ToString();
            totalUniqueVisitorsLabelHistorical.Text = historicalData.uniqueVisitors.ToString();
            totalRepeatVisitorsLabelHistorical.Text = historicalData.returnVisitors.ToString();
            totalNewVisitorsLabelHistorical.Text = historicalData.newVisitors.ToString();

            PopulateRandomData(points);
            // Create SeriesCollection for both charts
            var seriesCollection1 = new SeriesCollection();
            var seriesCollection2 = new SeriesCollection();

            // Define the variables to plot
            string[] variables1 = { "totalTime", "avgTime", "maxTime", "minTime" };
            string[] variables2 = { "totalVisits", "uniqueVisitors", "returnVisitors", "newVisitors" };

            // Add series to the SeriesCollection for each variable
            foreach (string variable in variables1)
            {
                var series = new LineSeries
                {
                    Title = variable,
                    Values = new ChartValues<int>()
                };
                seriesCollection1.Add(series);
            }

            foreach (string variable in variables2)
            {
                var series = new LineSeries
                {
                    Title = variable,
                    Values = new ChartValues<int>()
                };
                seriesCollection2.Add(series);
            }

            // Bind the SeriesCollection to the charts
            historicalTimeChart.Series = seriesCollection1;
            historicalVisitChart.Series = seriesCollection2;

            // Populate the data from the 'points' array
            foreach (BHData point in points)
            {
                for (int i = 0; i < variables1.Length; i++)
                {
                    seriesCollection1[i].Values.Add(GetPropertyValue(point, variables1[i]));
                }

                for (int i = 0; i < variables2.Length; i++)
                {
                    seriesCollection2[i].Values.Add(GetPropertyValue(point, variables2[i]));
                }
            }
        }
        public void PopulateStatistics_DailySales(DateTime selectedDate)
        {

            SalesData dailyData = getSalesData(selectedDate, selectedDate);
            SalesData avgData = getSalesData(DateTime.MinValue, DateTime.MaxValue);
            SalesData threeDayData = getSalesData(selectedDate.AddDays(-3), selectedDate);
            SalesData sevenDayData = getSalesData(selectedDate.AddDays(-7), selectedDate);
            SalesData thirtyDayData = getSalesData(selectedDate.AddDays(-30), selectedDate);

            dailyTotalSalesPB.Image = dailyData.total_sale > avgData.total_sale ? Resources.green_triangle : Resources.red_triangle;
            dailyTotalReturnPB.Image = dailyData.total_return > avgData.total_return ? Resources.green_triangle : Resources.red_triangle;
            dailyNetSalesPB.Image = dailyData.net_sale > avgData.net_sale ? Resources.green_triangle : Resources.red_triangle;

            dailySaleQtyPB.Image = dailyData.sale_qty > avgData.sale_qty ? Resources.green_triangle : Resources.red_triangle;
            dailyReturnQtyPB.Image = dailyData.return_qty > avgData.return_qty ? Resources.green_triangle : Resources.red_triangle;
            dailyNetSaleQtyPB.Image = dailyData.net_sale_qty > avgData.net_sale_qty ? Resources.green_triangle : Resources.red_triangle;
            
            dailyTotalInvoicesPB.Image = dailyData.total_invoices > avgData.total_invoices ? Resources.green_triangle : Resources.red_triangle;

            dailyTotalSalesLabel.Text = dailyData.total_sale.ToString();
            dailyTotalReturnLabel.Text = dailyData.total_return.ToString();
            dailyNetSalesLabel.Text = dailyData.net_sale.ToString();
            dailySaleQtyLabel.Text = dailyData.sale_qty.ToString();
            dailyReturnQtyLabel.Text = dailyData.return_qty.ToString();
            dailyNetSaleQtyLabel.Text = dailyData.net_sale_qty.ToString();
            dailyTotalInvoicesLabel.Text = dailyData.total_invoices.ToString();

            dailySaleChart.Series.Clear();
            LiveCharts.SeriesCollection SaleSeries = new LiveCharts.SeriesCollection
            {
                new StackedRowSeries
                {
                    Title = "Total Sale",
                    Values = new LiveCharts.ChartValues<double> {dailyData.total_sale, threeDayData.total_sale, sevenDayData.total_sale, thirtyDayData.total_sale }
                },
                new StackedRowSeries
                {
                    Title = "Total Return",
                    Values = new LiveCharts.ChartValues<double> {dailyData.total_return, threeDayData.total_return , sevenDayData.total_return, thirtyDayData.total_return }
                },
                new StackedRowSeries
                {
                    Title = "Net Sale",
                    Values = new LiveCharts.ChartValues<double> {dailyData.net_sale, threeDayData.net_sale, sevenDayData.net_sale, thirtyDayData.net_sale }
                },
            };
            dailySaleChart.Series = SaleSeries;

            dailyQtyChart.Series.Clear();
            LiveCharts.SeriesCollection QtySeries = new LiveCharts.SeriesCollection
            {
                new StackedRowSeries
                {
                    Title = "Total Sale Qty",
                    Values = new LiveCharts.ChartValues<int> {dailyData.sale_qty, threeDayData.sale_qty, sevenDayData.sale_qty, thirtyDayData.sale_qty }
                },
                new StackedRowSeries
                {
                    Title = "Total Return Qty",
                    Values = new LiveCharts.ChartValues<int> {dailyData.return_qty, threeDayData.return_qty, sevenDayData.return_qty, thirtyDayData.return_qty }
                },
                new StackedRowSeries
                {
                    Title = "Net Sale Qty",
                    Values = new LiveCharts.ChartValues<int> {dailyData.net_sale_qty, threeDayData.net_sale_qty, sevenDayData.net_sale_qty, thirtyDayData.net_sale_qty }
                },
            };

            dailyQtyChart.Series = QtySeries;


        }

        private double GetSalesPropertyValue(SalesData data, string propertyName)
        {
            // Helper method to retrieve property values from BHData
            switch (propertyName)
            {
                case "totalSales":
                    return data.total_sale;
                case "totalReturn":
                    return data.total_return;
                case "netSales":
                    return data.net_sale;
                case "saleQty":
                    return data.sale_qty;
                case "returnQty":
                    return data.return_qty;
                case "netSaleQty":
                    return data.net_sale_qty;
                default:
                    return 0;
            }
        }

        public static void PopulateRandomSalesData(SalesData[] points)
        {
            Random random = new Random();

            for (int i = 0; i < points.Length; i++)
            {
                int totalSales = random.Next(1, 1000);
                int totalReturn = random.Next(1, 100);
                int netSales = random.Next(1, 1000);
                int saleQty = random.Next(1, 100);
                int returnQty = random.Next(1, 10000);
                int netSaleQty = random.Next(1, 1000);
                int totalInvoices = random.Next(1, 500);

                points[i] = new SalesData(totalSales, totalReturn, netSales, saleQty, returnQty, netSaleQty, totalInvoices);
            }
        }

        public void PopulateStatistics_HistoricalSales(DateTime fromDate, DateTime toDate)
        {
            TimeSpan difference = toDate - fromDate;
            if (fromDate >= toDate) return;
            int numDays = (int)(difference.TotalDays);
            int numPoints = 10;

            if (numDays < numPoints && numDays != 0) numPoints = numDays;
            int interval = numDays / numPoints;

            SalesData historicalData = getSalesData(fromDate, toDate);
            SalesData avgData = getSalesData(DateTime.MinValue, DateTime.MaxValue);
            SalesData[] points = new SalesData[numPoints];

            for (int i = 0; i < numPoints; i++)
            {
                DateTime intervalStartDate = fromDate.AddDays(i * interval);
                DateTime intervalEndDate = fromDate.AddDays((i + 1) * interval - 1);

                if (intervalEndDate > toDate) intervalEndDate = toDate;
                // Retrieve historical data for the current interval
                SalesData intervalData = getSalesData(intervalStartDate, intervalEndDate);

                // Assign the retrieved data to the points array
                points[i] = intervalData;
            }

            historicalTotalSalesPB.Image = historicalData.total_sale > avgData.total_sale ? Resources.green_triangle : Resources.red_triangle;
            historicalTotalReturnPB.Image = historicalData.total_return > avgData.total_return ? Resources.green_triangle : Resources.red_triangle;
            historicalNetSalesPB.Image = historicalData.net_sale > avgData.net_sale ? Resources.green_triangle : Resources.red_triangle;
            historicalSalesQtyPB.Image = historicalData.sale_qty > avgData.sale_qty ? Resources.green_triangle : Resources.red_triangle;
            historicalReturnQtyPB.Image = historicalData.return_qty > avgData.return_qty ? Resources.green_triangle : Resources.red_triangle;
            historicalNetSaleQtyPB.Image = historicalData.net_sale_qty > avgData.net_sale_qty ? Resources.green_triangle : Resources.red_triangle;

            historicalTotalInvoicesPB.Image = historicalData.total_invoices > avgData.total_invoices ? Resources.green_triangle : Resources.red_triangle;

            historicalTotalSalesLabel.Text = historicalData.total_sale.ToString();
            historicalTotalReturnLabel.Text = historicalData.total_return.ToString();
            historicalNetSalesLabel.Text = historicalData.net_sale.ToString();
            historicalSaleQtyLabel.Text = historicalData.sale_qty.ToString();
            historicalReturnQtyLabel.Text = historicalData.return_qty.ToString();
            historicalNetSaleQtyLabel.Text = historicalData.net_sale_qty.ToString();
            historicalTotalInvoicesLabel.Text = historicalData.total_invoices.ToString();

            PopulateRandomSalesData(points);
            // Create SeriesCollection for both charts
            var seriesCollection1 = new SeriesCollection();
            var seriesCollection2 = new SeriesCollection();

            // Define the variables to plot
            string[] variables1 = { "totalSales", "totalReturn", "netSales"};
            string[] variables2 = { "saleQty", "returnQty", "netSaleQty"};

            // Add series to the SeriesCollection for each variable
            foreach (string variable in variables1)
            {
                var series = new LineSeries
                {
                    Title = variable,
                    Values = new ChartValues<double>()
                };
                seriesCollection1.Add(series);
            }

            foreach (string variable in variables2)
            {
                var series = new LineSeries
                {
                    Title = variable,
                    Values = new ChartValues<int>()
                };
                seriesCollection2.Add(series);
            }

            // Bind the SeriesCollection to the charts
            historicalSaleChart.Series = seriesCollection1;
            historicalQtyChart.Series = seriesCollection2;

            // Populate the data from the 'points' array
            foreach (SalesData point in points)
            {
                for (int i = 0; i < variables1.Length; i++)
                {
                    seriesCollection1[i].Values.Add(GetSalesPropertyValue(point, variables1[i]));
                }

                for (int i = 0; i < variables2.Length; i++)
                {
                    seriesCollection2[i].Values.Add((int)GetSalesPropertyValue(point, variables2[i]));
                }
            }
        }
        private void dailyOverviewTab_Enter(object sender, EventArgs e)
        {
            DateTime today = DateTime.Now.Date;
            PopulateStatistics_Daily(today);
        }
        private void historicalTab_Enter(object sender, EventArgs e)
        {
            PopulateStatistics_Historical(fromDatePicker.Value.Date, toDatePicker.Value.Date);
        }
        private void dailySalesTab_Enter(object sender, EventArgs e)
        {
            DateTime today = DateTime.Now.Date;
            PopulateStatistics_DailySales(today);
        }
        private void historicalSalesTab_Enter(object sender, EventArgs e)
        {

            PopulateStatistics_HistoricalSales(fromSalesDatePicker.Value.Date, toSalesDatePicker.Value.Date);
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

        private void dailyOverviewTab_Click(object sender, EventArgs e)
        {

        }
    }
}
