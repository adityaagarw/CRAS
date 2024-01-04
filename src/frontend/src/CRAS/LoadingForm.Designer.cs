namespace CRAS
{
    partial class LoadingForm
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.loadingLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.entryPictureBox = new System.Windows.Forms.PictureBox();
            this.exitPictureBox = new System.Windows.Forms.PictureBox();
            this.billingPictureBox = new System.Windows.Forms.PictureBox();
            this.employeePictureBox = new System.Windows.Forms.PictureBox();
            this.backendPictureBox = new System.Windows.Forms.PictureBox();
            this.startingPictureBox = new System.Windows.Forms.PictureBox();
            this.shutdownPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.entryPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exitPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.billingPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.employeePictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.backendPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startingPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.shutdownPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::CRAS.Properties.Resources.Wedges_3s_200px__1_;
            this.pictureBox1.Location = new System.Drawing.Point(109, 11);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(269, 214);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // loadingLabel
            // 
            this.loadingLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.loadingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadingLabel.Location = new System.Drawing.Point(0, 412);
            this.loadingLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.loadingLabel.Name = "loadingLabel";
            this.loadingLabel.Size = new System.Drawing.Size(505, 13);
            this.loadingLabel.TabIndex = 1;
            this.loadingLabel.Text = "Connecting";
            this.loadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(117, 255);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Entry Module";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(124, 284);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Exit Module";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(114, 308);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Billing Module";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(95, 332);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Employee Module";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(259, 255);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Backend Module";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(266, 279);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Starting Module";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(254, 303);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Shutdown Module";
            // 
            // entryPictureBox
            // 
            this.entryPictureBox.Image = global::CRAS.Properties.Resources.red_cross_2;
            this.entryPictureBox.Location = new System.Drawing.Point(195, 255);
            this.entryPictureBox.Name = "entryPictureBox";
            this.entryPictureBox.Size = new System.Drawing.Size(18, 18);
            this.entryPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.entryPictureBox.TabIndex = 9;
            this.entryPictureBox.TabStop = false;
            // 
            // exitPictureBox
            // 
            this.exitPictureBox.Image = global::CRAS.Properties.Resources.red_cross_2;
            this.exitPictureBox.Location = new System.Drawing.Point(195, 279);
            this.exitPictureBox.Name = "exitPictureBox";
            this.exitPictureBox.Size = new System.Drawing.Size(18, 18);
            this.exitPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.exitPictureBox.TabIndex = 10;
            this.exitPictureBox.TabStop = false;
            // 
            // billingPictureBox
            // 
            this.billingPictureBox.Image = global::CRAS.Properties.Resources.red_cross_2;
            this.billingPictureBox.Location = new System.Drawing.Point(195, 303);
            this.billingPictureBox.Name = "billingPictureBox";
            this.billingPictureBox.Size = new System.Drawing.Size(18, 18);
            this.billingPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.billingPictureBox.TabIndex = 11;
            this.billingPictureBox.TabStop = false;
            // 
            // employeePictureBox
            // 
            this.employeePictureBox.Image = global::CRAS.Properties.Resources.red_cross_2;
            this.employeePictureBox.Location = new System.Drawing.Point(195, 327);
            this.employeePictureBox.Name = "employeePictureBox";
            this.employeePictureBox.Size = new System.Drawing.Size(18, 18);
            this.employeePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.employeePictureBox.TabIndex = 12;
            this.employeePictureBox.TabStop = false;
            // 
            // backendPictureBox
            // 
            this.backendPictureBox.Image = global::CRAS.Properties.Resources.red_cross_2;
            this.backendPictureBox.Location = new System.Drawing.Point(353, 250);
            this.backendPictureBox.Name = "backendPictureBox";
            this.backendPictureBox.Size = new System.Drawing.Size(18, 18);
            this.backendPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.backendPictureBox.TabIndex = 13;
            this.backendPictureBox.TabStop = false;
            // 
            // startingPictureBox
            // 
            this.startingPictureBox.Image = global::CRAS.Properties.Resources.red_cross_2;
            this.startingPictureBox.Location = new System.Drawing.Point(353, 274);
            this.startingPictureBox.Name = "startingPictureBox";
            this.startingPictureBox.Size = new System.Drawing.Size(18, 18);
            this.startingPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.startingPictureBox.TabIndex = 14;
            this.startingPictureBox.TabStop = false;
            // 
            // shutdownPictureBox
            // 
            this.shutdownPictureBox.Image = global::CRAS.Properties.Resources.red_cross_2;
            this.shutdownPictureBox.Location = new System.Drawing.Point(353, 298);
            this.shutdownPictureBox.Name = "shutdownPictureBox";
            this.shutdownPictureBox.Size = new System.Drawing.Size(18, 18);
            this.shutdownPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.shutdownPictureBox.TabIndex = 15;
            this.shutdownPictureBox.TabStop = false;
            // 
            // LoadingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(505, 425);
            this.Controls.Add(this.shutdownPictureBox);
            this.Controls.Add(this.startingPictureBox);
            this.Controls.Add(this.backendPictureBox);
            this.Controls.Add(this.employeePictureBox);
            this.Controls.Add(this.billingPictureBox);
            this.Controls.Add(this.exitPictureBox);
            this.Controls.Add(this.entryPictureBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.loadingLabel);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "LoadingForm";
            this.Text = "LoadingForm";
            this.Load += new System.EventHandler(this.LoadingForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.entryPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exitPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.billingPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.employeePictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.backendPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startingPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.shutdownPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.Label loadingLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.PictureBox entryPictureBox;
        public System.Windows.Forms.PictureBox exitPictureBox;
        public System.Windows.Forms.PictureBox billingPictureBox;
        public System.Windows.Forms.PictureBox employeePictureBox;
        public System.Windows.Forms.PictureBox backendPictureBox;
        public System.Windows.Forms.PictureBox startingPictureBox;
        public System.Windows.Forms.PictureBox shutdownPictureBox;
    }
}