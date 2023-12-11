namespace CRAS
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cam1StatusLabel = new System.Windows.Forms.Label();
            this.cam2StatusLabel = new System.Windows.Forms.Label();
            this.startStopFeedButton = new System.Windows.Forms.Button();
            this.statusButton = new System.Windows.Forms.Button();
            this.customerFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.exitedCustomerFLP = new System.Windows.Forms.FlowLayoutPanel();
            this.billingButton = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.scanStatusLabel = new System.Windows.Forms.Label();
            this.displayComboBox = new System.Windows.Forms.ComboBox();
            this.totalCustomersLabel = new System.Windows.Forms.Label();
            this.totalCustomersValue = new System.Windows.Forms.Label();
            this.totalExitedLabel = new System.Windows.Forms.Label();
            this.totalExitedValue = new System.Windows.Forms.Label();
            this.dbButton = new System.Windows.Forms.Button();
            this.bhDashboardButton = new System.Windows.Forms.Button();
            this.addEmployee = new System.Windows.Forms.PictureBox();
            this.customerFlowLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.addEmployee)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(244, 307);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Status:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(244, 609);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Status:";
            // 
            // cam1StatusLabel
            // 
            this.cam1StatusLabel.AutoSize = true;
            this.cam1StatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cam1StatusLabel.ForeColor = System.Drawing.Color.Red;
            this.cam1StatusLabel.Location = new System.Drawing.Point(281, 307);
            this.cam1StatusLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.cam1StatusLabel.Name = "cam1StatusLabel";
            this.cam1StatusLabel.Size = new System.Drawing.Size(44, 13);
            this.cam1StatusLabel.TabIndex = 4;
            this.cam1StatusLabel.Text = "inactive";
            // 
            // cam2StatusLabel
            // 
            this.cam2StatusLabel.AutoSize = true;
            this.cam2StatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cam2StatusLabel.ForeColor = System.Drawing.Color.Red;
            this.cam2StatusLabel.Location = new System.Drawing.Point(282, 609);
            this.cam2StatusLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.cam2StatusLabel.Name = "cam2StatusLabel";
            this.cam2StatusLabel.Size = new System.Drawing.Size(44, 13);
            this.cam2StatusLabel.TabIndex = 5;
            this.cam2StatusLabel.Text = "inactive";
            // 
            // startStopFeedButton
            // 
            this.startStopFeedButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.startStopFeedButton.Location = new System.Drawing.Point(95, 7);
            this.startStopFeedButton.Margin = new System.Windows.Forms.Padding(2);
            this.startStopFeedButton.Name = "startStopFeedButton";
            this.startStopFeedButton.Size = new System.Drawing.Size(133, 38);
            this.startStopFeedButton.TabIndex = 6;
            this.startStopFeedButton.Text = "Start Live Feed";
            this.startStopFeedButton.UseVisualStyleBackColor = true;
            this.startStopFeedButton.Click += new System.EventHandler(this.startStopFeedButton_Click);
            // 
            // statusButton
            // 
            this.statusButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.statusButton.Location = new System.Drawing.Point(928, 15);
            this.statusButton.Margin = new System.Windows.Forms.Padding(2);
            this.statusButton.Name = "statusButton";
            this.statusButton.Size = new System.Drawing.Size(56, 24);
            this.statusButton.TabIndex = 7;
            this.statusButton.Text = "Status";
            this.statusButton.UseVisualStyleBackColor = true;
            this.statusButton.Click += new System.EventHandler(this.statusButton_Click);
            // 
            // customerFlowLayout
            // 
            this.customerFlowLayout.Controls.Add(this.exitedCustomerFLP);
            this.customerFlowLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.customerFlowLayout.Location = new System.Drawing.Point(436, 50);
            this.customerFlowLayout.Margin = new System.Windows.Forms.Padding(2);
            this.customerFlowLayout.Name = "customerFlowLayout";
            this.customerFlowLayout.Size = new System.Drawing.Size(584, 544);
            this.customerFlowLayout.TabIndex = 11;
            this.customerFlowLayout.WrapContents = false;
            // 
            // exitedCustomerFLP
            // 
            this.exitedCustomerFLP.BackColor = System.Drawing.Color.White;
            this.exitedCustomerFLP.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.exitedCustomerFLP.Location = new System.Drawing.Point(2, 2);
            this.exitedCustomerFLP.Margin = new System.Windows.Forms.Padding(2);
            this.exitedCustomerFLP.Name = "exitedCustomerFLP";
            this.exitedCustomerFLP.Size = new System.Drawing.Size(584, 542);
            this.exitedCustomerFLP.TabIndex = 12;
            this.exitedCustomerFLP.WrapContents = false;
            // 
            // billingButton
            // 
            this.billingButton.BackgroundImage = global::CRAS.Properties.Resources._307_3074480_cash_register_icons_cash_register_icon_png;
            this.billingButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.billingButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.billingButton.Location = new System.Drawing.Point(975, 600);
            this.billingButton.Margin = new System.Windows.Forms.Padding(2);
            this.billingButton.Name = "billingButton";
            this.billingButton.Size = new System.Drawing.Size(44, 37);
            this.billingButton.TabIndex = 12;
            this.billingButton.UseVisualStyleBackColor = true;
            this.billingButton.Click += new System.EventHandler(this.billingButton_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(20, 355);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(302, 251);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(20, 50);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(302, 254);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // scanStatusLabel
            // 
            this.scanStatusLabel.AutoSize = true;
            this.scanStatusLabel.Location = new System.Drawing.Point(806, 612);
            this.scanStatusLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.scanStatusLabel.Name = "scanStatusLabel";
            this.scanStatusLabel.Size = new System.Drawing.Size(52, 13);
            this.scanStatusLabel.TabIndex = 13;
            this.scanStatusLabel.Text = "Scanning";
            this.scanStatusLabel.TextChanged += new System.EventHandler(this.scanStatusLabel_TextChanged);
            // 
            // displayComboBox
            // 
            this.displayComboBox.FormattingEnabled = true;
            this.displayComboBox.Location = new System.Drawing.Point(436, 26);
            this.displayComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.displayComboBox.Name = "displayComboBox";
            this.displayComboBox.Size = new System.Drawing.Size(148, 21);
            this.displayComboBox.TabIndex = 14;
            this.displayComboBox.SelectedIndexChanged += new System.EventHandler(this.displayComboBox_SelectedIndexChanged);
            // 
            // totalCustomersLabel
            // 
            this.totalCustomersLabel.AutoSize = true;
            this.totalCustomersLabel.Location = new System.Drawing.Point(434, 609);
            this.totalCustomersLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.totalCustomersLabel.Name = "totalCustomersLabel";
            this.totalCustomersLabel.Size = new System.Drawing.Size(129, 13);
            this.totalCustomersLabel.TabIndex = 15;
            this.totalCustomersLabel.Text = "Total Customers In-Store: ";
            // 
            // totalCustomersValue
            // 
            this.totalCustomersValue.AutoSize = true;
            this.totalCustomersValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalCustomersValue.Location = new System.Drawing.Point(558, 609);
            this.totalCustomersValue.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.totalCustomersValue.Name = "totalCustomersValue";
            this.totalCustomersValue.Size = new System.Drawing.Size(14, 13);
            this.totalCustomersValue.TabIndex = 16;
            this.totalCustomersValue.Text = "0";
            // 
            // totalExitedLabel
            // 
            this.totalExitedLabel.AutoSize = true;
            this.totalExitedLabel.Location = new System.Drawing.Point(434, 612);
            this.totalExitedLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.totalExitedLabel.Name = "totalExitedLabel";
            this.totalExitedLabel.Size = new System.Drawing.Size(121, 13);
            this.totalExitedLabel.TabIndex = 17;
            this.totalExitedLabel.Text = "Total Exited Customers: ";
            this.totalExitedLabel.Visible = false;
            // 
            // totalExitedValue
            // 
            this.totalExitedValue.AutoSize = true;
            this.totalExitedValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalExitedValue.Location = new System.Drawing.Point(558, 612);
            this.totalExitedValue.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.totalExitedValue.Name = "totalExitedValue";
            this.totalExitedValue.Size = new System.Drawing.Size(14, 13);
            this.totalExitedValue.TabIndex = 18;
            this.totalExitedValue.Text = "0";
            this.totalExitedValue.Visible = false;
            // 
            // dbButton
            // 
            this.dbButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dbButton.Location = new System.Drawing.Point(848, 15);
            this.dbButton.Margin = new System.Windows.Forms.Padding(2);
            this.dbButton.Name = "dbButton";
            this.dbButton.Size = new System.Drawing.Size(56, 24);
            this.dbButton.TabIndex = 19;
            this.dbButton.Text = "DB";
            this.dbButton.UseVisualStyleBackColor = true;
            this.dbButton.Click += new System.EventHandler(this.dbButton_Click);
            // 
            // bhDashboardButton
            // 
            this.bhDashboardButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bhDashboardButton.Location = new System.Drawing.Point(729, 15);
            this.bhDashboardButton.Margin = new System.Windows.Forms.Padding(2);
            this.bhDashboardButton.Name = "bhDashboardButton";
            this.bhDashboardButton.Size = new System.Drawing.Size(87, 24);
            this.bhDashboardButton.TabIndex = 20;
            this.bhDashboardButton.Text = "BH Dashboard";
            this.bhDashboardButton.UseVisualStyleBackColor = true;
            this.bhDashboardButton.Click += new System.EventHandler(this.bhDashboardButton_Click);
            // 
            // addEmployee
            // 
            this.addEmployee.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("addEmployee.BackgroundImage")));
            this.addEmployee.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.addEmployee.Location = new System.Drawing.Point(928, 600);
            this.addEmployee.Margin = new System.Windows.Forms.Padding(2);
            this.addEmployee.Name = "addEmployee";
            this.addEmployee.Size = new System.Drawing.Size(42, 37);
            this.addEmployee.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.addEmployee.TabIndex = 21;
            this.addEmployee.TabStop = false;
            this.addEmployee.Click += new System.EventHandler(this.addEmployee_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1028, 647);
            this.Controls.Add(this.addEmployee);
            this.Controls.Add(this.bhDashboardButton);
            this.Controls.Add(this.dbButton);
            this.Controls.Add(this.totalExitedValue);
            this.Controls.Add(this.totalExitedLabel);
            this.Controls.Add(this.totalCustomersValue);
            this.Controls.Add(this.totalCustomersLabel);
            this.Controls.Add(this.displayComboBox);
            this.Controls.Add(this.scanStatusLabel);
            this.Controls.Add(this.billingButton);
            this.Controls.Add(this.customerFlowLayout);
            this.Controls.Add(this.statusButton);
            this.Controls.Add(this.startStopFeedButton);
            this.Controls.Add(this.cam2StatusLabel);
            this.Controls.Add(this.cam1StatusLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.customerFlowLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.addEmployee)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label cam1StatusLabel;
        private System.Windows.Forms.Label cam2StatusLabel;
        private System.Windows.Forms.Button startStopFeedButton;
        private System.Windows.Forms.Button statusButton;
        private System.Windows.Forms.Button billingButton;
        public System.Windows.Forms.Label scanStatusLabel;
        private System.Windows.Forms.ComboBox displayComboBox;
        public System.Windows.Forms.Label totalCustomersLabel;
        public System.Windows.Forms.Label totalCustomersValue;
        public System.Windows.Forms.FlowLayoutPanel exitedCustomerFLP;
        public System.Windows.Forms.FlowLayoutPanel customerFlowLayout;
        public System.Windows.Forms.Label totalExitedLabel;
        public System.Windows.Forms.Label totalExitedValue;
        private System.Windows.Forms.Button dbButton;
        private System.Windows.Forms.Button bhDashboardButton;
        public System.Windows.Forms.PictureBox addEmployee;
    }
}