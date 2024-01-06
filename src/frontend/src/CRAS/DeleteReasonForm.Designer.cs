namespace CRAS
{
    partial class DeleteReasonForm
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
            this.sameFace = new System.Windows.Forms.Button();
            this.misidentifiedFace = new System.Windows.Forms.Button();
            this.unidentifiedFace = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(154, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please select a reason";
            // 
            // sameFace
            // 
            this.sameFace.Location = new System.Drawing.Point(28, 55);
            this.sameFace.Name = "sameFace";
            this.sameFace.Size = new System.Drawing.Size(88, 26);
            this.sameFace.TabIndex = 1;
            this.sameFace.Text = "Same Face";
            this.sameFace.UseVisualStyleBackColor = true;
            this.sameFace.Click += new System.EventHandler(this.sameFace_Click);
            // 
            // misidentifiedFace
            // 
            this.misidentifiedFace.Location = new System.Drawing.Point(162, 55);
            this.misidentifiedFace.Name = "misidentifiedFace";
            this.misidentifiedFace.Size = new System.Drawing.Size(100, 26);
            this.misidentifiedFace.TabIndex = 2;
            this.misidentifiedFace.Text = "Misidentified Face";
            this.misidentifiedFace.UseVisualStyleBackColor = true;
            this.misidentifiedFace.Click += new System.EventHandler(this.misidentifiedFace_Click);
            // 
            // unidentifiedFace
            // 
            this.unidentifiedFace.Location = new System.Drawing.Point(300, 55);
            this.unidentifiedFace.Name = "unidentifiedFace";
            this.unidentifiedFace.Size = new System.Drawing.Size(99, 26);
            this.unidentifiedFace.TabIndex = 3;
            this.unidentifiedFace.Text = "Unidentified Face";
            this.unidentifiedFace.UseVisualStyleBackColor = true;
            this.unidentifiedFace.Click += new System.EventHandler(this.unidentifiedFace_Click);
            // 
            // DeleteReasonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 105);
            this.Controls.Add(this.unidentifiedFace);
            this.Controls.Add(this.misidentifiedFace);
            this.Controls.Add(this.sameFace);
            this.Controls.Add(this.label1);
            this.Name = "DeleteReasonForm";
            this.Text = "Select reason for deletion";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button sameFace;
        private System.Windows.Forms.Button misidentifiedFace;
        private System.Windows.Forms.Button unidentifiedFace;
    }
}