namespace CRAS
{
    partial class IncompleteManager
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
            this.incompleteDataGrid = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.incompleteDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // incompleteDataGrid
            // 
            this.incompleteDataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.incompleteDataGrid.BackgroundColor = System.Drawing.Color.White;
            this.incompleteDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.incompleteDataGrid.Location = new System.Drawing.Point(12, 32);
            this.incompleteDataGrid.Name = "incompleteDataGrid";
            this.incompleteDataGrid.Size = new System.Drawing.Size(1309, 652);
            this.incompleteDataGrid.TabIndex = 0;
            // 
            // IncompleteManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1333, 696);
            this.Controls.Add(this.incompleteDataGrid);
            this.Name = "IncompleteManager";
            this.Text = "IncompleteManager";
            this.Load += new System.EventHandler(this.IncompleteManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.incompleteDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView incompleteDataGrid;
    }
}