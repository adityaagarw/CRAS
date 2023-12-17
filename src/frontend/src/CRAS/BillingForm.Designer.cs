namespace CRAS
{
    partial class BillingForm
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
            this.identifiedFacesFLP = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.rescanButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.userRatingFLP = new System.Windows.Forms.FlowLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.expRatingFLP = new System.Windows.Forms.FlowLayoutPanel();
            this.scanStatus = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.emptyTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.billNoLabel = new System.Windows.Forms.Label();
            this.billAmtLabel = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.billNameLabel = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.billMobileLabel = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.returnAmtLabel = new System.Windows.Forms.Label();
            this.returnAmt = new System.Windows.Forms.Label();
            this.previousButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.selectedCustomerUC = new CRAS.CustomerDataUC();
            this.updateButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // identifiedFacesFLP
            // 
            this.identifiedFacesFLP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.identifiedFacesFLP.Location = new System.Drawing.Point(12, 58);
            this.identifiedFacesFLP.Name = "identifiedFacesFLP";
            this.identifiedFacesFLP.Size = new System.Drawing.Size(1154, 130);
            this.identifiedFacesFLP.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Identified Faces -";
            // 
            // rescanButton
            // 
            this.rescanButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rescanButton.Location = new System.Drawing.Point(1061, 12);
            this.rescanButton.Name = "rescanButton";
            this.rescanButton.Size = new System.Drawing.Size(105, 40);
            this.rescanButton.TabIndex = 2;
            this.rescanButton.Text = "Re-Scan";
            this.rescanButton.UseVisualStyleBackColor = true;
            this.rescanButton.Click += new System.EventHandler(this.rescanButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 240);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(215, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "SELECTED CUSTOMER";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(781, 286);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "User Rating";
            // 
            // userRatingFLP
            // 
            this.userRatingFLP.Location = new System.Drawing.Point(884, 263);
            this.userRatingFLP.Name = "userRatingFLP";
            this.userRatingFLP.Size = new System.Drawing.Size(282, 55);
            this.userRatingFLP.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(781, 380);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 16);
            this.label4.TabIndex = 7;
            this.label4.Text = "Exp. Rating";
            // 
            // expRatingFLP
            // 
            this.expRatingFLP.Location = new System.Drawing.Point(884, 356);
            this.expRatingFLP.Name = "expRatingFLP";
            this.expRatingFLP.Size = new System.Drawing.Size(282, 55);
            this.expRatingFLP.TabIndex = 7;
            // 
            // scanStatus
            // 
            this.scanStatus.AutoSize = true;
            this.scanStatus.ForeColor = System.Drawing.Color.DarkGreen;
            this.scanStatus.Location = new System.Drawing.Point(133, 39);
            this.scanStatus.Name = "scanStatus";
            this.scanStatus.Size = new System.Drawing.Size(159, 16);
            this.scanStatus.TabIndex = 8;
            this.scanStatus.Text = "Scanning/Scan Complete";
            this.scanStatus.TextChanged += new System.EventHandler(this.scanStatus_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 445);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(132, 20);
            this.label5.TabIndex = 9;
            this.label5.Text = "BILL DETAILS";
            // 
            // emptyTextBox
            // 
            this.emptyTextBox.CausesValidation = false;
            this.emptyTextBox.Cursor = System.Windows.Forms.Cursors.No;
            this.emptyTextBox.Enabled = false;
            this.emptyTextBox.Location = new System.Drawing.Point(12, 480);
            this.emptyTextBox.Multiline = true;
            this.emptyTextBox.Name = "emptyTextBox";
            this.emptyTextBox.ReadOnly = true;
            this.emptyTextBox.Size = new System.Drawing.Size(1154, 56);
            this.emptyTextBox.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(25, 493);
            this.label6.Name = "label6";
            this.label6.Padding = new System.Windows.Forms.Padding(5);
            this.label6.Size = new System.Drawing.Size(76, 30);
            this.label6.TabIndex = 11;
            this.label6.Text = "Bill No";
            // 
            // billNoLabel
            // 
            this.billNoLabel.AutoSize = true;
            this.billNoLabel.BackColor = System.Drawing.SystemColors.WindowText;
            this.billNoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.billNoLabel.ForeColor = System.Drawing.Color.Cornsilk;
            this.billNoLabel.Location = new System.Drawing.Point(98, 493);
            this.billNoLabel.Name = "billNoLabel";
            this.billNoLabel.Padding = new System.Windows.Forms.Padding(5);
            this.billNoLabel.Size = new System.Drawing.Size(59, 30);
            this.billNoLabel.TabIndex = 12;
            this.billNoLabel.Text = "0000";
            this.billNoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // billAmtLabel
            // 
            this.billAmtLabel.AutoSize = true;
            this.billAmtLabel.BackColor = System.Drawing.SystemColors.WindowText;
            this.billAmtLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.billAmtLabel.ForeColor = System.Drawing.Color.Cornsilk;
            this.billAmtLabel.Location = new System.Drawing.Point(730, 493);
            this.billAmtLabel.Name = "billAmtLabel";
            this.billAmtLabel.Padding = new System.Windows.Forms.Padding(5);
            this.billAmtLabel.Size = new System.Drawing.Size(29, 30);
            this.billAmtLabel.TabIndex = 14;
            this.billAmtLabel.Text = "0";
            this.billAmtLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(648, 493);
            this.label9.Name = "label9";
            this.label9.Padding = new System.Windows.Forms.Padding(5);
            this.label9.Size = new System.Drawing.Size(86, 30);
            this.label9.TabIndex = 13;
            this.label9.Text = "Bill Amt";
            // 
            // billNameLabel
            // 
            this.billNameLabel.AutoSize = true;
            this.billNameLabel.BackColor = System.Drawing.SystemColors.WindowText;
            this.billNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.billNameLabel.ForeColor = System.Drawing.Color.Cornsilk;
            this.billNameLabel.Location = new System.Drawing.Point(256, 493);
            this.billNameLabel.Name = "billNameLabel";
            this.billNameLabel.Padding = new System.Windows.Forms.Padding(5);
            this.billNameLabel.Size = new System.Drawing.Size(127, 30);
            this.billNameLabel.TabIndex = 16;
            this.billNameLabel.Text = "ABCD WXYZ";
            this.billNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(194, 493);
            this.label11.Name = "label11";
            this.label11.Padding = new System.Windows.Forms.Padding(5);
            this.label11.Size = new System.Drawing.Size(67, 30);
            this.label11.TabIndex = 15;
            this.label11.Text = "Name";
            // 
            // billMobileLabel
            // 
            this.billMobileLabel.AutoSize = true;
            this.billMobileLabel.BackColor = System.Drawing.SystemColors.WindowText;
            this.billMobileLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.billMobileLabel.ForeColor = System.Drawing.Color.Cornsilk;
            this.billMobileLabel.Location = new System.Drawing.Point(497, 493);
            this.billMobileLabel.Name = "billMobileLabel";
            this.billMobileLabel.Padding = new System.Windows.Forms.Padding(5);
            this.billMobileLabel.Size = new System.Drawing.Size(119, 30);
            this.billMobileLabel.TabIndex = 18;
            this.billMobileLabel.Text = "9999999999";
            this.billMobileLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(430, 493);
            this.label13.Name = "label13";
            this.label13.Padding = new System.Windows.Forms.Padding(5);
            this.label13.Size = new System.Drawing.Size(74, 30);
            this.label13.TabIndex = 17;
            this.label13.Text = "Mobile";
            // 
            // returnAmtLabel
            // 
            this.returnAmtLabel.AutoSize = true;
            this.returnAmtLabel.BackColor = System.Drawing.SystemColors.WindowText;
            this.returnAmtLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.returnAmtLabel.ForeColor = System.Drawing.Color.Cornsilk;
            this.returnAmtLabel.Location = new System.Drawing.Point(934, 493);
            this.returnAmtLabel.Name = "returnAmtLabel";
            this.returnAmtLabel.Padding = new System.Windows.Forms.Padding(5);
            this.returnAmtLabel.Size = new System.Drawing.Size(29, 30);
            this.returnAmtLabel.TabIndex = 20;
            this.returnAmtLabel.Text = "0";
            this.returnAmtLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // returnAmt
            // 
            this.returnAmt.AutoSize = true;
            this.returnAmt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.returnAmt.Location = new System.Drawing.Point(824, 493);
            this.returnAmt.Name = "returnAmt";
            this.returnAmt.Padding = new System.Windows.Forms.Padding(5);
            this.returnAmt.Size = new System.Drawing.Size(114, 30);
            this.returnAmt.TabIndex = 19;
            this.returnAmt.Text = "Return Amt";
            // 
            // previousButton
            // 
            this.previousButton.BackColor = System.Drawing.Color.Cornsilk;
            this.previousButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.previousButton.Font = new System.Drawing.Font("Normande BT", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.previousButton.Location = new System.Drawing.Point(12, 553);
            this.previousButton.Name = "previousButton";
            this.previousButton.Size = new System.Drawing.Size(37, 33);
            this.previousButton.TabIndex = 21;
            this.previousButton.Text = "<";
            this.previousButton.UseVisualStyleBackColor = false;
            this.previousButton.Click += new System.EventHandler(this.previousButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.BackColor = System.Drawing.Color.Cornsilk;
            this.nextButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.nextButton.Font = new System.Drawing.Font("Normande BT", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nextButton.Location = new System.Drawing.Point(64, 553);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(37, 33);
            this.nextButton.TabIndex = 22;
            this.nextButton.Text = ">";
            this.nextButton.UseVisualStyleBackColor = false;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // selectedCustomerUC
            // 
            this.selectedCustomerUC.BackColor = System.Drawing.Color.White;
            this.selectedCustomerUC.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.selectedCustomerUC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.selectedCustomerUC.isSelected = false;
            this.selectedCustomerUC.Location = new System.Drawing.Point(12, 263);
            this.selectedCustomerUC.Name = "selectedCustomerUC";
            this.selectedCustomerUC.Size = new System.Drawing.Size(736, 148);
            this.selectedCustomerUC.TabIndex = 3;
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(1039, 544);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(127, 42);
            this.updateButton.TabIndex = 23;
            this.updateButton.Text = "UPDATE";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // BillingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1178, 598);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.previousButton);
            this.Controls.Add(this.returnAmtLabel);
            this.Controls.Add(this.returnAmt);
            this.Controls.Add(this.billMobileLabel);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.billNameLabel);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.billAmtLabel);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.billNoLabel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.emptyTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.scanStatus);
            this.Controls.Add(this.expRatingFLP);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.userRatingFLP);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.selectedCustomerUC);
            this.Controls.Add(this.rescanButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.identifiedFacesFLP);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "BillingForm";
            this.Text = "BillingForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BillingForm_FormClosing);
            this.Load += new System.EventHandler(this.BillingForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private CustomerDataUC selectedCustomerUC;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.FlowLayoutPanel userRatingFLP;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.FlowLayoutPanel expRatingFLP;
        public System.Windows.Forms.Label scanStatus;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox emptyTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label billNoLabel;
        private System.Windows.Forms.Label billAmtLabel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label billNameLabel;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label billMobileLabel;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label returnAmtLabel;
        private System.Windows.Forms.Label returnAmt;
        public System.Windows.Forms.Button rescanButton;
        public System.Windows.Forms.FlowLayoutPanel identifiedFacesFLP;
        public System.Windows.Forms.Button previousButton;
        public System.Windows.Forms.Button nextButton;
        public System.Windows.Forms.Button updateButton;
    }
}