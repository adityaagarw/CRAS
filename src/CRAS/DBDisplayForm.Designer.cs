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
            this.selectDBCombo.Location = new System.Drawing.Point(125, 38);
            this.selectDBCombo.Name = "selectDBCombo";
            this.selectDBCombo.Size = new System.Drawing.Size(121, 24);
            this.selectDBCombo.TabIndex = 0;
            this.selectDBCombo.SelectedIndexChanged += new System.EventHandler(this.selectDBCombo_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select DB";
            // 
            // selectTableCombo
            // 
            this.selectTableCombo.FormattingEnabled = true;
            this.selectTableCombo.Location = new System.Drawing.Point(458, 38);
            this.selectTableCombo.Name = "selectTableCombo";
            this.selectTableCombo.Size = new System.Drawing.Size(121, 24);
            this.selectTableCombo.TabIndex = 2;
            this.selectTableCombo.SelectedIndexChanged += new System.EventHandler(this.selectTableCombo_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(342, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Select Table";
            // 
            // tableDataGridView
            // 
            this.tableDataGridView.BackgroundColor = System.Drawing.Color.White;
            this.tableDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tableDataGridView.Location = new System.Drawing.Point(43, 113);
            this.tableDataGridView.Name = "tableDataGridView";
            this.tableDataGridView.RowHeadersWidth = 51;
            this.tableDataGridView.RowTemplate.Height = 24;
            this.tableDataGridView.Size = new System.Drawing.Size(695, 325);
            this.tableDataGridView.TabIndex = 4;
            // 
            // DBDisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tableDataGridView);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.selectTableCombo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.selectDBCombo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
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