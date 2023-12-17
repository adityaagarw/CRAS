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
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Live Pipes";
            // 
            // livePipeListBox
            // 
            this.livePipeListBox.FormattingEnabled = true;
            this.livePipeListBox.ItemHeight = 16;
            this.livePipeListBox.Location = new System.Drawing.Point(15, 58);
            this.livePipeListBox.Name = "livePipeListBox";
            this.livePipeListBox.Size = new System.Drawing.Size(190, 84);
            this.livePipeListBox.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 166);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Redis DB:";
            // 
            // redisStatusLabel
            // 
            this.redisStatusLabel.AutoSize = true;
            this.redisStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.redisStatusLabel.ForeColor = System.Drawing.Color.Red;
            this.redisStatusLabel.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.redisStatusLabel.Location = new System.Drawing.Point(96, 166);
            this.redisStatusLabel.Name = "redisStatusLabel";
            this.redisStatusLabel.Size = new System.Drawing.Size(109, 16);
            this.redisStatusLabel.TabIndex = 4;
            this.redisStatusLabel.Text = "Not Connected";
            // 
            // webCamListBox
            // 
            this.webCamListBox.FormattingEnabled = true;
            this.webCamListBox.ItemHeight = 16;
            this.webCamListBox.Location = new System.Drawing.Point(15, 230);
            this.webCamListBox.Name = "webCamListBox";
            this.webCamListBox.Size = new System.Drawing.Size(190, 84);
            this.webCamListBox.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 211);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 16);
            this.label3.TabIndex = 8;
            this.label3.Text = "Webcams";
            // 
            // refreshButton
            // 
            this.refreshButton.BackgroundImage = global::CRAS.Properties.Resources.refresh_icon_614x460;
            this.refreshButton.FlatAppearance.BorderSize = 0;
            this.refreshButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.refreshButton.Location = new System.Drawing.Point(514, 0);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(42, 44);
            this.refreshButton.TabIndex = 6;
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // Live_Status
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(556, 360);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.webCamListBox);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.redisStatusLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.livePipeListBox);
            this.Controls.Add(this.label1);
            this.Name = "Live_Status";
            this.Text = "Live_Status";
            this.Load += new System.EventHandler(this.Live_Status_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox livePipeListBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label redisStatusLabel;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.ListBox webCamListBox;
        private System.Windows.Forms.Label label3;
    }
}