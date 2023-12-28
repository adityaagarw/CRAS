namespace CRAS
{
    partial class Live_Status
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
            this.livePipeListBox = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.redisStatusLabel = new System.Windows.Forms.Label();
            this.webCamListBox = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.refreshButton = new System.Windows.Forms.Button();
            this.backendStatus = new System.Windows.Forms.GroupBox();
            this.shutdownLabel = new System.Windows.Forms.Label();
            this.startingLabel = new System.Windows.Forms.Label();
            this.backendLabel = new System.Windows.Forms.Label();
            this.employeeLabel = new System.Windows.Forms.Label();
            this.billingLabel = new System.Windows.Forms.Label();
            this.exitLabel = new System.Windows.Forms.Label();
            this.entryLabel = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.liveCheckbox = new System.Windows.Forms.CheckBox();
            this.intervalTextbox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.backendString = new System.Windows.Forms.Label();
            this.backendStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 32);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Live Pipes";
            // 
            // livePipeListBox
            // 
            this.livePipeListBox.FormattingEnabled = true;
            this.livePipeListBox.Location = new System.Drawing.Point(11, 47);
            this.livePipeListBox.Margin = new System.Windows.Forms.Padding(2);
            this.livePipeListBox.Name = "livePipeListBox";
            this.livePipeListBox.Size = new System.Drawing.Size(144, 69);
            this.livePipeListBox.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 135);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Redis DB:";
            // 
            // redisStatusLabel
            // 
            this.redisStatusLabel.AutoSize = true;
            this.redisStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.redisStatusLabel.ForeColor = System.Drawing.Color.Red;
            this.redisStatusLabel.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.redisStatusLabel.Location = new System.Drawing.Point(72, 135);
            this.redisStatusLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.redisStatusLabel.Name = "redisStatusLabel";
            this.redisStatusLabel.Size = new System.Drawing.Size(92, 13);
            this.redisStatusLabel.TabIndex = 4;
            this.redisStatusLabel.Text = "Not Connected";
            // 
            // webCamListBox
            // 
            this.webCamListBox.FormattingEnabled = true;
            this.webCamListBox.Location = new System.Drawing.Point(11, 187);
            this.webCamListBox.Margin = new System.Windows.Forms.Padding(2);
            this.webCamListBox.Name = "webCamListBox";
            this.webCamListBox.Size = new System.Drawing.Size(144, 69);
            this.webCamListBox.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(9, 171);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Webcams";
            // 
            // refreshButton
            // 
            this.refreshButton.BackgroundImage = global::CRAS.Properties.Resources.refresh_icon_614x460;
            this.refreshButton.FlatAppearance.BorderSize = 0;
            this.refreshButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.refreshButton.Location = new System.Drawing.Point(386, 0);
            this.refreshButton.Margin = new System.Windows.Forms.Padding(2);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(32, 36);
            this.refreshButton.TabIndex = 6;
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // backendStatus
            // 
            this.backendStatus.Controls.Add(this.shutdownLabel);
            this.backendStatus.Controls.Add(this.startingLabel);
            this.backendStatus.Controls.Add(this.backendLabel);
            this.backendStatus.Controls.Add(this.employeeLabel);
            this.backendStatus.Controls.Add(this.billingLabel);
            this.backendStatus.Controls.Add(this.exitLabel);
            this.backendStatus.Controls.Add(this.entryLabel);
            this.backendStatus.Controls.Add(this.label10);
            this.backendStatus.Controls.Add(this.label9);
            this.backendStatus.Controls.Add(this.label8);
            this.backendStatus.Controls.Add(this.label7);
            this.backendStatus.Controls.Add(this.label6);
            this.backendStatus.Controls.Add(this.label5);
            this.backendStatus.Controls.Add(this.label4);
            this.backendStatus.Location = new System.Drawing.Point(205, 41);
            this.backendStatus.Name = "backendStatus";
            this.backendStatus.Size = new System.Drawing.Size(200, 215);
            this.backendStatus.TabIndex = 9;
            this.backendStatus.TabStop = false;
            this.backendStatus.Text = "Backend Status";
            // 
            // shutdownLabel
            // 
            this.shutdownLabel.AutoSize = true;
            this.shutdownLabel.Location = new System.Drawing.Point(90, 193);
            this.shutdownLabel.Name = "shutdownLabel";
            this.shutdownLabel.Size = new System.Drawing.Size(27, 13);
            this.shutdownLabel.TabIndex = 13;
            this.shutdownLabel.Text = "N/A";
            // 
            // startingLabel
            // 
            this.startingLabel.AutoSize = true;
            this.startingLabel.Location = new System.Drawing.Point(90, 168);
            this.startingLabel.Name = "startingLabel";
            this.startingLabel.Size = new System.Drawing.Size(27, 13);
            this.startingLabel.TabIndex = 12;
            this.startingLabel.Text = "N/A";
            // 
            // backendLabel
            // 
            this.backendLabel.AutoSize = true;
            this.backendLabel.Location = new System.Drawing.Point(90, 140);
            this.backendLabel.Name = "backendLabel";
            this.backendLabel.Size = new System.Drawing.Size(27, 13);
            this.backendLabel.TabIndex = 11;
            this.backendLabel.Text = "N/A";
            // 
            // employeeLabel
            // 
            this.employeeLabel.AutoSize = true;
            this.employeeLabel.Location = new System.Drawing.Point(90, 115);
            this.employeeLabel.Name = "employeeLabel";
            this.employeeLabel.Size = new System.Drawing.Size(27, 13);
            this.employeeLabel.TabIndex = 10;
            this.employeeLabel.Text = "N/A";
            // 
            // billingLabel
            // 
            this.billingLabel.AutoSize = true;
            this.billingLabel.Location = new System.Drawing.Point(90, 88);
            this.billingLabel.Name = "billingLabel";
            this.billingLabel.Size = new System.Drawing.Size(27, 13);
            this.billingLabel.TabIndex = 9;
            this.billingLabel.Text = "N/A";
            // 
            // exitLabel
            // 
            this.exitLabel.AutoSize = true;
            this.exitLabel.Location = new System.Drawing.Point(90, 56);
            this.exitLabel.Name = "exitLabel";
            this.exitLabel.Size = new System.Drawing.Size(27, 13);
            this.exitLabel.TabIndex = 8;
            this.exitLabel.Text = "N/A";
            // 
            // entryLabel
            // 
            this.entryLabel.AutoSize = true;
            this.entryLabel.Location = new System.Drawing.Point(90, 24);
            this.entryLabel.Name = "entryLabel";
            this.entryLabel.Size = new System.Drawing.Size(27, 13);
            this.entryLabel.TabIndex = 7;
            this.entryLabel.Text = "N/A";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 193);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(55, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Shutdown";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 168);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(43, 13);
            this.label9.TabIndex = 5;
            this.label9.Text = "Starting";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 140);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "Backend";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 115);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Employee";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Billing";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 56);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(24, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Exit";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Entry";
            // 
            // liveCheckbox
            // 
            this.liveCheckbox.AutoSize = true;
            this.liveCheckbox.Location = new System.Drawing.Point(223, 263);
            this.liveCheckbox.Name = "liveCheckbox";
            this.liveCheckbox.Size = new System.Drawing.Size(46, 17);
            this.liveCheckbox.TabIndex = 10;
            this.liveCheckbox.Text = "Live";
            this.liveCheckbox.UseVisualStyleBackColor = true;
            this.liveCheckbox.CheckedChanged += new System.EventHandler(this.liveCheckbox_CheckedChanged);
            // 
            // intervalTextbox
            // 
            this.intervalTextbox.Location = new System.Drawing.Point(282, 262);
            this.intervalTextbox.Name = "intervalTextbox";
            this.intervalTextbox.Size = new System.Drawing.Size(40, 20);
            this.intervalTextbox.TabIndex = 11;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(341, 265);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(64, 13);
            this.label11.TabIndex = 12;
            this.label11.Text = "Interval (ms)";
            // 
            // backendString
            // 
            this.backendString.AutoSize = true;
            this.backendString.Location = new System.Drawing.Point(72, 270);
            this.backendString.Name = "backendString";
            this.backendString.Size = new System.Drawing.Size(27, 13);
            this.backendString.TabIndex = 14;
            this.backendString.Text = "N/A";
            // 
            // Live_Status
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(417, 292);
            this.Controls.Add(this.backendString);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.intervalTextbox);
            this.Controls.Add(this.liveCheckbox);
            this.Controls.Add(this.backendStatus);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.webCamListBox);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.redisStatusLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.livePipeListBox);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Live_Status";
            this.Text = "Live_Status";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Live_Status_FormClosing);
            this.Load += new System.EventHandler(this.Live_Status_Load);
            this.backendStatus.ResumeLayout(false);
            this.backendStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox livePipeListBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label redisStatusLabel;
        private System.Windows.Forms.ListBox webCamListBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox backendStatus;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label shutdownLabel;
        private System.Windows.Forms.Label startingLabel;
        private System.Windows.Forms.Label backendLabel;
        private System.Windows.Forms.Label employeeLabel;
        private System.Windows.Forms.Label billingLabel;
        private System.Windows.Forms.Label exitLabel;
        private System.Windows.Forms.Label entryLabel;
        public System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.CheckBox liveCheckbox;
        private System.Windows.Forms.TextBox intervalTextbox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label backendString;
    }
}