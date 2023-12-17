namespace CRAS
{
    partial class BHDashboard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bhTab = new System.Windows.Forms.TabControl();
            this.dailyOverviewTab = new System.Windows.Forms.TabPage();
            this.totalRepeatVisitorsLabel = new System.Windows.Forms.Label();
            this.totalNewVisitorsLabel = new System.Windows.Forms.Label();
            this.totalUniqueVisitorsLabel = new System.Windows.Forms.Label();
            this.totalVisitsLabel = new System.Windows.Forms.Label();
            this.minTimeSpentLabel = new System.Windows.Forms.Label();
            this.maxTimeSpentLabel = new System.Windows.Forms.Label();
            this.avgTimeSpentLabel = new System.Windows.Forms.Label();
            this.totalTimeSpentLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dailyDatePicker = new System.Windows.Forms.DateTimePicker();
            this.dateLabel = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.bhTab.SuspendLayout();
            this.dailyOverviewTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // bhTab
            // 
            this.bhTab.Controls.Add(this.dailyOverviewTab);
            this.bhTab.Controls.Add(this.tabPage2);
            this.bhTab.Location = new System.Drawing.Point(12, 12);
            this.bhTab.Name = "bhTab";
            this.bhTab.SelectedIndex = 0;
            this.bhTab.Size = new System.Drawing.Size(1244, 504);
            this.bhTab.TabIndex = 0;
            // 
            // dailyOverviewTab
            // 
            this.dailyOverviewTab.Controls.Add(this.totalRepeatVisitorsLabel);
            this.dailyOverviewTab.Controls.Add(this.totalNewVisitorsLabel);
            this.dailyOverviewTab.Controls.Add(this.totalUniqueVisitorsLabel);
            this.dailyOverviewTab.Controls.Add(this.totalVisitsLabel);
            this.dailyOverviewTab.Controls.Add(this.minTimeSpentLabel);
            this.dailyOverviewTab.Controls.Add(this.maxTimeSpentLabel);
            this.dailyOverviewTab.Controls.Add(this.avgTimeSpentLabel);
            this.dailyOverviewTab.Controls.Add(this.totalTimeSpentLabel);
            this.dailyOverviewTab.Controls.Add(this.label8);
            this.dailyOverviewTab.Controls.Add(this.label7);
            this.dailyOverviewTab.Controls.Add(this.label6);
            this.dailyOverviewTab.Controls.Add(this.label5);
            this.dailyOverviewTab.Controls.Add(this.label4);
            this.dailyOverviewTab.Controls.Add(this.label3);
            this.dailyOverviewTab.Controls.Add(this.label2);
            this.dailyOverviewTab.Controls.Add(this.label1);
            this.dailyOverviewTab.Controls.Add(this.dailyDatePicker);
            this.dailyOverviewTab.Controls.Add(this.dateLabel);
            this.dailyOverviewTab.Location = new System.Drawing.Point(4, 25);
            this.dailyOverviewTab.Name = "dailyOverviewTab";
            this.dailyOverviewTab.Padding = new System.Windows.Forms.Padding(3);
            this.dailyOverviewTab.Size = new System.Drawing.Size(1236, 475);
            this.dailyOverviewTab.TabIndex = 0;
            this.dailyOverviewTab.Text = "Daily Overview";
            this.dailyOverviewTab.UseVisualStyleBackColor = true;
            this.dailyOverviewTab.Enter += new System.EventHandler(this.dailyOverviewTab_Enter);
            // 
            // totalRepeatVisitorsLabel
            // 
            this.totalRepeatVisitorsLabel.AutoSize = true;
            this.totalRepeatVisitorsLabel.Font = new System.Drawing.Font("Montserrat", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalRepeatVisitorsLabel.Location = new System.Drawing.Point(801, 362);
            this.totalRepeatVisitorsLabel.Name = "totalRepeatVisitorsLabel";
            this.totalRepeatVisitorsLabel.Size = new System.Drawing.Size(20, 24);
            this.totalRepeatVisitorsLabel.TabIndex = 17;
            this.totalRepeatVisitorsLabel.Text = "5";
            this.totalRepeatVisitorsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // totalNewVisitorsLabel
            // 
            this.totalNewVisitorsLabel.AutoSize = true;
            this.totalNewVisitorsLabel.Font = new System.Drawing.Font("Montserrat", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalNewVisitorsLabel.Location = new System.Drawing.Point(801, 302);
            this.totalNewVisitorsLabel.Name = "totalNewVisitorsLabel";
            this.totalNewVisitorsLabel.Size = new System.Drawing.Size(32, 24);
            this.totalNewVisitorsLabel.TabIndex = 16;
            this.totalNewVisitorsLabel.Text = "50";
            this.totalNewVisitorsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // totalUniqueVisitorsLabel
            // 
            this.totalUniqueVisitorsLabel.AutoSize = true;
            this.totalUniqueVisitorsLabel.Font = new System.Drawing.Font("Montserrat", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalUniqueVisitorsLabel.Location = new System.Drawing.Point(801, 244);
            this.totalUniqueVisitorsLabel.Name = "totalUniqueVisitorsLabel";
            this.totalUniqueVisitorsLabel.Size = new System.Drawing.Size(30, 24);
            this.totalUniqueVisitorsLabel.TabIndex = 15;
            this.totalUniqueVisitorsLabel.Text = "55";
            this.totalUniqueVisitorsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // totalVisitsLabel
            // 
            this.totalVisitsLabel.AutoSize = true;
            this.totalVisitsLabel.Font = new System.Drawing.Font("Montserrat", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalVisitsLabel.Location = new System.Drawing.Point(801, 189);
            this.totalVisitsLabel.Name = "totalVisitsLabel";
            this.totalVisitsLabel.Size = new System.Drawing.Size(36, 24);
            this.totalVisitsLabel.TabIndex = 14;
            this.totalVisitsLabel.Text = "110";
            this.totalVisitsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // minTimeSpentLabel
            // 
            this.minTimeSpentLabel.AutoSize = true;
            this.minTimeSpentLabel.Font = new System.Drawing.Font("Montserrat", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minTimeSpentLabel.Location = new System.Drawing.Point(205, 364);
            this.minTimeSpentLabel.Name = "minTimeSpentLabel";
            this.minTimeSpentLabel.Size = new System.Drawing.Size(121, 24);
            this.minTimeSpentLabel.TabIndex = 13;
            this.minTimeSpentLabel.Text = "500 Minutes";
            this.minTimeSpentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // maxTimeSpentLabel
            // 
            this.maxTimeSpentLabel.AutoSize = true;
            this.maxTimeSpentLabel.Font = new System.Drawing.Font("Montserrat", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maxTimeSpentLabel.Location = new System.Drawing.Point(205, 302);
            this.maxTimeSpentLabel.Name = "maxTimeSpentLabel";
            this.maxTimeSpentLabel.Size = new System.Drawing.Size(121, 24);
            this.maxTimeSpentLabel.TabIndex = 12;
            this.maxTimeSpentLabel.Text = "500 Minutes";
            this.maxTimeSpentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // avgTimeSpentLabel
            // 
            this.avgTimeSpentLabel.AutoSize = true;
            this.avgTimeSpentLabel.Font = new System.Drawing.Font("Montserrat", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.avgTimeSpentLabel.Location = new System.Drawing.Point(205, 244);
            this.avgTimeSpentLabel.Name = "avgTimeSpentLabel";
            this.avgTimeSpentLabel.Size = new System.Drawing.Size(121, 24);
            this.avgTimeSpentLabel.TabIndex = 11;
            this.avgTimeSpentLabel.Text = "500 Minutes";
            this.avgTimeSpentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // totalTimeSpentLabel
            // 
            this.totalTimeSpentLabel.AutoSize = true;
            this.totalTimeSpentLabel.Font = new System.Drawing.Font("Montserrat", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalTimeSpentLabel.Location = new System.Drawing.Point(205, 189);
            this.totalTimeSpentLabel.Name = "totalTimeSpentLabel";
            this.totalTimeSpentLabel.Size = new System.Drawing.Size(121, 24);
            this.totalTimeSpentLabel.TabIndex = 10;
            this.totalTimeSpentLabel.Text = "500 Minutes";
            this.totalTimeSpentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(645, 368);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(133, 16);
            this.label8.TabIndex = 9;
            this.label8.Text = "Total Repeat Visitors";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(663, 308);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(115, 16);
            this.label7.TabIndex = 8;
            this.label7.Text = "Total New Visitors";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(647, 250);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(131, 16);
            this.label6.TabIndex = 7;
            this.label6.Text = "Total Unique Visitors";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(49, 370);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(132, 16);
            this.label5.TabIndex = 6;
            this.label5.Text = "Minimum Time Spent";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(45, 308);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(136, 16);
            this.label4.TabIndex = 5;
            this.label4.Text = "Maximum Time Spent";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(50, 250);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Average Time Spent";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(71, 195);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Total Time Spent";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(705, 195);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Total Visits";
            // 
            // dailyDatePicker
            // 
            this.dailyDatePicker.Location = new System.Drawing.Point(522, 34);
            this.dailyDatePicker.MaxDate = new System.DateTime(2099, 12, 31, 0, 0, 0, 0);
            this.dailyDatePicker.MinDate = new System.DateTime(2023, 1, 1, 0, 0, 0, 0);
            this.dailyDatePicker.Name = "dailyDatePicker";
            this.dailyDatePicker.Size = new System.Drawing.Size(200, 22);
            this.dailyDatePicker.TabIndex = 1;
            this.dailyDatePicker.ValueChanged += new System.EventHandler(this.dailyDatePicker_ValueChanged);
            // 
            // dateLabel
            // 
            this.dateLabel.AutoSize = true;
            this.dateLabel.Location = new System.Drawing.Point(430, 39);
            this.dateLabel.Name = "dateLabel";
            this.dateLabel.Size = new System.Drawing.Size(36, 16);
            this.dateLabel.TabIndex = 0;
            this.dateLabel.Text = "Date";
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1236, 475);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // BHDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1268, 528);
            this.Controls.Add(this.bhTab);
            this.Name = "BHDashboard";
            this.Text = "Business Health Dashboard";
            this.bhTab.ResumeLayout(false);
            this.dailyOverviewTab.ResumeLayout(false);
            this.dailyOverviewTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl bhTab;
        private System.Windows.Forms.TabPage dailyOverviewTab;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dailyDatePicker;
        private System.Windows.Forms.Label dateLabel;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label totalRepeatVisitorsLabel;
        private System.Windows.Forms.Label totalNewVisitorsLabel;
        private System.Windows.Forms.Label totalUniqueVisitorsLabel;
        private System.Windows.Forms.Label totalVisitsLabel;
        private System.Windows.Forms.Label minTimeSpentLabel;
        private System.Windows.Forms.Label maxTimeSpentLabel;
        private System.Windows.Forms.Label avgTimeSpentLabel;
        private System.Windows.Forms.Label totalTimeSpentLabel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
    }
}