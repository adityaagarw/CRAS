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
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.customerFlowLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(326, 378);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Status:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(326, 749);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Status:";
            // 
            // cam1StatusLabel
            // 
            this.cam1StatusLabel.AutoSize = true;
            this.cam1StatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cam1StatusLabel.ForeColor = System.Drawing.Color.Red;
            this.cam1StatusLabel.Location = new System.Drawing.Point(375, 378);
            this.cam1StatusLabel.Name = "cam1StatusLabel";
            this.cam1StatusLabel.Size = new System.Drawing.Size(53, 16);
            this.cam1StatusLabel.TabIndex = 4;
            this.cam1StatusLabel.Text = "inactive";
            // 
            // cam2StatusLabel
            // 
            this.cam2StatusLabel.AutoSize = true;
            this.cam2StatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cam2StatusLabel.ForeColor = System.Drawing.Color.Red;
            this.cam2StatusLabel.Location = new System.Drawing.Point(376, 749);
            this.cam2StatusLabel.Name = "cam2StatusLabel";
            this.cam2StatusLabel.Size = new System.Drawing.Size(53, 16);
            this.cam2StatusLabel.TabIndex = 5;
            this.cam2StatusLabel.Text = "inactive";
            // 
            // startStopFeedButton
            // 
            this.startStopFeedButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.startStopFeedButton.Location = new System.Drawing.Point(127, 9);
            this.startStopFeedButton.Name = "startStopFeedButton";
            this.startStopFeedButton.Size = new System.Drawing.Size(177, 47);
            this.startStopFeedButton.TabIndex = 6;
            this.startStopFeedButton.Text = "Start Live Feed";
            this.startStopFeedButton.UseVisualStyleBackColor = true;
            this.startStopFeedButton.Click += new System.EventHandler(this.startStopFeedButton_Click);
            // 
            // statusButton
            // 
            this.statusButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.statusButton.Location = new System.Drawing.Point(1238, 18);
            this.statusButton.Name = "statusButton";
            this.statusButton.Size = new System.Drawing.Size(75, 29);
            this.statusButton.TabIndex = 7;
            this.statusButton.Text = "Status";
            this.statusButton.UseVisualStyleBackColor = true;
            this.statusButton.Click += new System.EventHandler(this.statusButton_Click);
            // 
            // customerFlowLayout
            // 
            this.customerFlowLayout.Controls.Add(this.exitedCustomerFLP);
            this.customerFlowLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.customerFlowLayout.Location = new System.Drawing.Point(581, 62);
            this.customerFlowLayout.Name = "customerFlowLayout";
            this.customerFlowLayout.Size = new System.Drawing.Size(778, 670);
            this.customerFlowLayout.TabIndex = 11;
            this.customerFlowLayout.WrapContents = false;
            // 
            // exitedCustomerFLP
            // 
            this.exitedCustomerFLP.BackColor = System.Drawing.Color.White;
            this.exitedCustomerFLP.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.exitedCustomerFLP.Location = new System.Drawing.Point(3, 3);
            this.exitedCustomerFLP.Name = "exitedCustomerFLP";
            this.exitedCustomerFLP.Size = new System.Drawing.Size(778, 667);
            this.exitedCustomerFLP.TabIndex = 12;
            this.exitedCustomerFLP.WrapContents = false;
            // 
            // billingButton
            // 
            this.billingButton.BackgroundImage = global::CRAS.Properties.Resources.Billing;
            this.billingButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.billingButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.billingButton.Location = new System.Drawing.Point(1300, 738);
            this.billingButton.Name = "billingButton";
            this.billingButton.Size = new System.Drawing.Size(59, 46);
            this.billingButton.TabIndex = 12;
            this.billingButton.UseVisualStyleBackColor = true;
            this.billingButton.Click += new System.EventHandler(this.billingButton_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(26, 437);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(403, 309);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(26, 62);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(403, 313);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // scanStatusLabel
            // 
            this.scanStatusLabel.AutoSize = true;
            this.scanStatusLabel.Location = new System.Drawing.Point(1117, 753);
            this.scanStatusLabel.Name = "scanStatusLabel";
            this.scanStatusLabel.Size = new System.Drawing.Size(63, 16);
            this.scanStatusLabel.TabIndex = 13;
            this.scanStatusLabel.Text = "Scanning";
            this.scanStatusLabel.TextChanged += new System.EventHandler(this.scanStatusLabel_TextChanged);
            // 
            // displayComboBox
            // 
            this.displayComboBox.FormattingEnabled = true;
            this.displayComboBox.Location = new System.Drawing.Point(581, 32);
            this.displayComboBox.Name = "displayComboBox";
            this.displayComboBox.Size = new System.Drawing.Size(196, 24);
            this.displayComboBox.TabIndex = 14;
            this.displayComboBox.SelectedIndexChanged += new System.EventHandler(this.displayComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(578, 749);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(160, 16);
            this.label3.TabIndex = 15;
            this.label3.Text = "Total Customers In-Store: ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(744, 749);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(15, 16);
            this.label4.TabIndex = 16;
            this.label4.Text = "0";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1371, 796);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
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
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.customerFlowLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.FlowLayoutPanel exitedCustomerFLP;
        public System.Windows.Forms.FlowLayoutPanel customerFlowLayout;
    }
}