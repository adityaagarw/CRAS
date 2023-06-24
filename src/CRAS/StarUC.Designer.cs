namespace CRAS
{
    partial class StarUC
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.starPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.starPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // starPictureBox
            // 
            this.starPictureBox.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.starPictureBox.BackgroundImage = global::CRAS.Properties.Resources.StarUnselected;
            this.starPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.starPictureBox.Location = new System.Drawing.Point(0, 0);
            this.starPictureBox.Name = "starPictureBox";
            this.starPictureBox.Size = new System.Drawing.Size(50, 50);
            this.starPictureBox.TabIndex = 0;
            this.starPictureBox.TabStop = false;
            this.starPictureBox.Click += new System.EventHandler(this.starPictureBox_Click);
            this.starPictureBox.MouseEnter += new System.EventHandler(this.starPictureBox_MouseEnter);
            this.starPictureBox.MouseLeave += new System.EventHandler(this.starPictureBox_MouseLeave);
            // 
            // StarUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.starPictureBox);
            this.Name = "StarUC";
            this.Size = new System.Drawing.Size(50, 50);
            ((System.ComponentModel.ISupportInitialize)(this.starPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox starPictureBox;
    }
}
