namespace CRAS
{
    partial class CustomerDetailForm
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
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lastVisitTextBox = new System.Windows.Forms.TextBox();
            this.mobileTextBox = new System.Windows.Forms.MaskedTextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.avgTimeTextBox = new System.Windows.Forms.TextBox();
            this.avgPurchaseTextBox = new System.Windows.Forms.TextBox();
            this.maxPurchasetextBox = new System.Windows.Forms.TextBox();
            this.remarksTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lastVisitsDataGrid = new System.Windows.Forms.DataGridView();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.memberSinceLabel = new System.Windows.Forms.Label();
            this.categoryLabel = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.updateButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lastVisitsDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(164, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(164, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "Mobile";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(164, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 23);
            this.label3.TabIndex = 3;
            this.label3.Text = "Last Visit";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(427, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(139, 23);
            this.label4.TabIndex = 4;
            this.label4.Text = "Avg Time Spent";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(449, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(117, 23);
            this.label5.TabIndex = 5;
            this.label5.Text = "Avg Purchase";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(449, 122);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(119, 23);
            this.label6.TabIndex = 6;
            this.label6.Text = "Max Purchase";
            // 
            // lastVisitTextBox
            // 
            this.lastVisitTextBox.Location = new System.Drawing.Point(252, 123);
            this.lastVisitTextBox.Name = "lastVisitTextBox";
            this.lastVisitTextBox.ReadOnly = true;
            this.lastVisitTextBox.Size = new System.Drawing.Size(151, 22);
            this.lastVisitTextBox.TabIndex = 3;
            // 
            // mobileTextBox
            // 
            this.mobileTextBox.Location = new System.Drawing.Point(252, 65);
            this.mobileTextBox.Mask = "+\\91-0000000000";
            this.mobileTextBox.Name = "mobileTextBox";
            this.mobileTextBox.Size = new System.Drawing.Size(151, 22);
            this.mobileTextBox.TabIndex = 2;
            this.mobileTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.mobileTextBox.TextChanged += new System.EventHandler(this.mobileTextBox_TextChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(134, 133);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(252, 14);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(151, 22);
            this.nameTextBox.TabIndex = 1;
            this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
            // 
            // avgTimeTextBox
            // 
            this.avgTimeTextBox.Location = new System.Drawing.Point(572, 12);
            this.avgTimeTextBox.Name = "avgTimeTextBox";
            this.avgTimeTextBox.ReadOnly = true;
            this.avgTimeTextBox.Size = new System.Drawing.Size(151, 22);
            this.avgTimeTextBox.TabIndex = 7;
            // 
            // avgPurchaseTextBox
            // 
            this.avgPurchaseTextBox.Location = new System.Drawing.Point(572, 66);
            this.avgPurchaseTextBox.Name = "avgPurchaseTextBox";
            this.avgPurchaseTextBox.ReadOnly = true;
            this.avgPurchaseTextBox.Size = new System.Drawing.Size(151, 22);
            this.avgPurchaseTextBox.TabIndex = 8;
            // 
            // maxPurchasetextBox
            // 
            this.maxPurchasetextBox.Location = new System.Drawing.Point(572, 122);
            this.maxPurchasetextBox.Name = "maxPurchasetextBox";
            this.maxPurchasetextBox.ReadOnly = true;
            this.maxPurchasetextBox.Size = new System.Drawing.Size(151, 22);
            this.maxPurchasetextBox.TabIndex = 9;
            // 
            // remarksTextBox
            // 
            this.remarksTextBox.Location = new System.Drawing.Point(762, 40);
            this.remarksTextBox.Multiline = true;
            this.remarksTextBox.Name = "remarksTextBox";
            this.remarksTextBox.Size = new System.Drawing.Size(454, 104);
            this.remarksTextBox.TabIndex = 10;
            this.remarksTextBox.TextChanged += new System.EventHandler(this.remarksTextBox_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(758, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 23);
            this.label7.TabIndex = 11;
            this.label7.Text = "Remarks";
            // 
            // lastVisitsDataGrid
            // 
            this.lastVisitsDataGrid.BackgroundColor = System.Drawing.Color.White;
            this.lastVisitsDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.lastVisitsDataGrid.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lastVisitsDataGrid.Location = new System.Drawing.Point(111, 182);
            this.lastVisitsDataGrid.Name = "lastVisitsDataGrid";
            this.lastVisitsDataGrid.RowHeadersWidth = 51;
            this.lastVisitsDataGrid.RowTemplate.Height = 24;
            this.lastVisitsDataGrid.Size = new System.Drawing.Size(612, 256);
            this.lastVisitsDataGrid.TabIndex = 12;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(761, 168);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(130, 23);
            this.label8.TabIndex = 13;
            this.label8.Text = "Member Since:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(1042, 168);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(89, 23);
            this.label9.TabIndex = 14;
            this.label9.Text = "Category:";
            // 
            // memberSinceLabel
            // 
            this.memberSinceLabel.AutoSize = true;
            this.memberSinceLabel.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.memberSinceLabel.Location = new System.Drawing.Point(913, 168);
            this.memberSinceLabel.Name = "memberSinceLabel";
            this.memberSinceLabel.Size = new System.Drawing.Size(96, 23);
            this.memberSinceLabel.TabIndex = 15;
            this.memberSinceLabel.Text = "01/01/2023";
            // 
            // categoryLabel
            // 
            this.categoryLabel.AutoSize = true;
            this.categoryLabel.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.categoryLabel.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.categoryLabel.Location = new System.Drawing.Point(1153, 168);
            this.categoryLabel.Name = "categoryLabel";
            this.categoryLabel.Size = new System.Drawing.Size(46, 23);
            this.categoryLabel.TabIndex = 16;
            this.categoryLabel.Text = "AAA";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(16, 182);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(89, 23);
            this.label10.TabIndex = 17;
            this.label10.Text = "Last Visits";
            // 
            // updateButton
            // 
            this.updateButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.updateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.updateButton.Location = new System.Drawing.Point(818, 371);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(141, 56);
            this.updateButton.TabIndex = 18;
            this.updateButton.Text = "UPDATE";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.Location = new System.Drawing.Point(1046, 371);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(141, 56);
            this.closeButton.TabIndex = 19;
            this.closeButton.Text = "CLOSE";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // CustomerDetailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1254, 450);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.categoryLabel);
            this.Controls.Add(this.memberSinceLabel);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lastVisitsDataGrid);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.remarksTextBox);
            this.Controls.Add(this.maxPurchasetextBox);
            this.Controls.Add(this.avgPurchaseTextBox);
            this.Controls.Add(this.avgTimeTextBox);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.mobileTextBox);
            this.Controls.Add(this.lastVisitTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "CustomerDetailForm";
            this.Text = "Customer Details";
            this.Load += new System.EventHandler(this.CustomerDetailForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lastVisitsDataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox lastVisitTextBox;
        private System.Windows.Forms.MaskedTextBox mobileTextBox;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.TextBox avgTimeTextBox;
        private System.Windows.Forms.TextBox avgPurchaseTextBox;
        private System.Windows.Forms.TextBox maxPurchasetextBox;
        private System.Windows.Forms.TextBox remarksTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataGridView lastVisitsDataGrid;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label memberSinceLabel;
        private System.Windows.Forms.Label categoryLabel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Button closeButton;
    }
}