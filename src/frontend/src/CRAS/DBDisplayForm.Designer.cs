namespace CRAS
{
    partial class DBDisplayForm
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
            this.selectDBCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.selectTableCombo = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tableDataGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.tableDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // selectDBCombo
            // 
            this.selectDBCombo.FormattingEnabled = true;
            this.selectDBCombo.Location = new System.Drawing.Point(195, 36);
            this.selectDBCombo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.selectDBCombo.Name = "selectDBCombo";
            this.selectDBCombo.Size = new System.Drawing.Size(92, 21);
            this.selectDBCombo.TabIndex = 0;
            this.selectDBCombo.SelectedIndexChanged += new System.EventHandler(this.selectDBCombo_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(131, 38);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select DB";
            // 
            // selectTableCombo
            // 
            this.selectTableCombo.FormattingEnabled = true;
            this.selectTableCombo.Location = new System.Drawing.Point(445, 36);
            this.selectTableCombo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.selectTableCombo.Name = "selectTableCombo";
            this.selectTableCombo.Size = new System.Drawing.Size(144, 21);
            this.selectTableCombo.TabIndex = 2;
            this.selectTableCombo.SelectedIndexChanged += new System.EventHandler(this.selectTableCombo_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(357, 38);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Select Table";
            // 
            // tableDataGridView
            // 
            this.tableDataGridView.AllowUserToAddRows = false;
            this.tableDataGridView.AllowUserToDeleteRows = false;
            this.tableDataGridView.AllowUserToOrderColumns = true;
            this.tableDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableDataGridView.BackgroundColor = System.Drawing.Color.White;
            this.tableDataGridView.ColumnHeadersHeight = 29;
            this.tableDataGridView.Location = new System.Drawing.Point(32, 92);
            this.tableDataGridView.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableDataGridView.Name = "tableDataGridView";
            this.tableDataGridView.ReadOnly = true;
            this.tableDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.tableDataGridView.RowHeadersWidth = 51;
            this.tableDataGridView.RowTemplate.Height = 24;
            this.tableDataGridView.Size = new System.Drawing.Size(680, 533);
            this.tableDataGridView.TabIndex = 4;
            // 
            // DBDisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(759, 635);
            this.Controls.Add(this.tableDataGridView);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.selectTableCombo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.selectDBCombo);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "DBDisplayForm";
            this.Text = "DB Display";
            this.Load += new System.EventHandler(this.DBDisplayForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tableDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox selectDBCombo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox selectTableCombo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView tableDataGridView;
    }
}