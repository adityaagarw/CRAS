namespace CRAS
{
    partial class ConfigManager
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
            this.configDataGrid = new System.Windows.Forms.DataGridView();
            this.save_restart = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.configDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // configDataGrid
            // 
            this.configDataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.configDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.configDataGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.configDataGrid.BackgroundColor = System.Drawing.Color.White;
            this.configDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.configDataGrid.Location = new System.Drawing.Point(12, 43);
            this.configDataGrid.Name = "configDataGrid";
            this.configDataGrid.Size = new System.Drawing.Size(585, 640);
            this.configDataGrid.TabIndex = 0;
            this.configDataGrid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.configDataGrid_CellEndEdit);
            // 
            // save_restart
            // 
            this.save_restart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.save_restart.Location = new System.Drawing.Point(12, 700);
            this.save_restart.Name = "save_restart";
            this.save_restart.Size = new System.Drawing.Size(585, 23);
            this.save_restart.TabIndex = 1;
            this.save_restart.Text = "Save and Restart";
            this.save_restart.UseVisualStyleBackColor = true;
            this.save_restart.Click += new System.EventHandler(this.saveRestart_Click);
            // 
            // ConfigManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(618, 735);
            this.Controls.Add(this.save_restart);
            this.Controls.Add(this.configDataGrid);
            this.Name = "ConfigManager";
            this.Text = "ConfigManager";
            this.Load += new System.EventHandler(this.ConfigManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.configDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView configDataGrid;
        private System.Windows.Forms.Button save_restart;
    }
}