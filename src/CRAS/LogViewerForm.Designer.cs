namespace CRAS
{
    partial class LogViewerForm
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
            this.getLogButton = new System.Windows.Forms.Button();
            this.fromDate = new System.Windows.Forms.DateTimePicker();
            this.toDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.logAdvancedGrid = new ADGV.AdvancedDataGridView();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.liveUpdate = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.logAdvancedGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // getLogButton
            // 
            this.getLogButton.Location = new System.Drawing.Point(740, 43);
            this.getLogButton.Name = "getLogButton";
            this.getLogButton.Size = new System.Drawing.Size(75, 23);
            this.getLogButton.TabIndex = 2;
            this.getLogButton.Text = "Get Log";
            this.getLogButton.UseVisualStyleBackColor = true;
            this.getLogButton.Click += new System.EventHandler(this.getLogButton_Click);
            // 
            // fromDate
            // 
            this.fromDate.Location = new System.Drawing.Point(123, 44);
            this.fromDate.Name = "fromDate";
            this.fromDate.Size = new System.Drawing.Size(200, 20);
            this.fromDate.TabIndex = 3;
            // 
            // toDate
            // 
            this.toDate.Location = new System.Drawing.Point(466, 44);
            this.toDate.Name = "toDate";
            this.toDate.Size = new System.Drawing.Size(200, 20);
            this.toDate.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(87, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "From";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(430, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "To";
            // 
            // logAdvancedGrid
            // 
            this.logAdvancedGrid.AllowUserToAddRows = false;
            this.logAdvancedGrid.AllowUserToDeleteRows = false;
            this.logAdvancedGrid.AllowUserToOrderColumns = true;
            this.logAdvancedGrid.AutoGenerateContextFilters = true;
            this.logAdvancedGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.logAdvancedGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.logAdvancedGrid.BackgroundColor = System.Drawing.Color.White;
            this.logAdvancedGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.logAdvancedGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.logAdvancedGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.logAdvancedGrid.DateWithTime = false;
            this.logAdvancedGrid.Location = new System.Drawing.Point(33, 121);
            this.logAdvancedGrid.Name = "logAdvancedGrid";
            this.logAdvancedGrid.ReadOnly = true;
            this.logAdvancedGrid.Size = new System.Drawing.Size(872, 434);
            this.logAdvancedGrid.TabIndex = 7;
            this.logAdvancedGrid.TimeFilter = false;
            this.logAdvancedGrid.SortStringChanged += new System.EventHandler(this.logAdvancedGrid_SortStringChanged);
            this.logAdvancedGrid.FilterStringChanged += new System.EventHandler(this.logAdvancedGrid_FilterStringChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(0, 0);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(80, 17);
            this.checkBox1.TabIndex = 8;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // liveUpdate
            // 
            this.liveUpdate.AutoSize = true;
            this.liveUpdate.Location = new System.Drawing.Point(738, 85);
            this.liveUpdate.Name = "liveUpdate";
            this.liveUpdate.Size = new System.Drawing.Size(84, 17);
            this.liveUpdate.TabIndex = 9;
            this.liveUpdate.Text = "Live Update";
            this.liveUpdate.UseVisualStyleBackColor = true;
            // 
            // LogViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(930, 567);
            this.Controls.Add(this.liveUpdate);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.logAdvancedGrid);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.toDate);
            this.Controls.Add(this.fromDate);
            this.Controls.Add(this.getLogButton);
            this.Name = "LogViewerForm";
            this.Text = "Log Viewer";
            this.Load += new System.EventHandler(this.LogViewerForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.logAdvancedGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button getLogButton;
        private System.Windows.Forms.DateTimePicker toDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private ADGV.AdvancedDataGridView logAdvancedGrid;
        private System.Windows.Forms.CheckBox checkBox1;
        public System.Windows.Forms.CheckBox liveUpdate;
        public System.Windows.Forms.DateTimePicker fromDate;
    }
}