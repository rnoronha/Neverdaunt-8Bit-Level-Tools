namespace LevelChibifier
{
    partial class ChibifierGui
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
            this.LevelPosition = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.LevelName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.LevelBrowse = new System.Windows.Forms.Button();
            this.LoadBrowser = new System.Windows.Forms.OpenFileDialog();
            this.Chibify = new System.Windows.Forms.Button();
            this.SaveBrowser = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // LevelPosition
            // 
            this.LevelPosition.Location = new System.Drawing.Point(75, 31);
            this.LevelPosition.Name = "LevelPosition";
            this.LevelPosition.Size = new System.Drawing.Size(256, 256);
            this.LevelPosition.TabIndex = 0;
            this.LevelPosition.TabStop = false;
            this.LevelPosition.Text = "Level Position";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(72, 290);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "-X";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(311, 290);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "+X";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(337, 274);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "-Y";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(337, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "+Y";
            // 
            // LevelName
            // 
            this.LevelName.Location = new System.Drawing.Point(104, 316);
            this.LevelName.Name = "LevelName";
            this.LevelName.Size = new System.Drawing.Size(152, 20);
            this.LevelName.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(62, 319);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Level:";
            // 
            // LevelBrowse
            // 
            this.LevelBrowse.Location = new System.Drawing.Point(262, 314);
            this.LevelBrowse.Name = "LevelBrowse";
            this.LevelBrowse.Size = new System.Drawing.Size(75, 23);
            this.LevelBrowse.TabIndex = 7;
            this.LevelBrowse.Text = "Browse";
            this.LevelBrowse.UseVisualStyleBackColor = true;
            this.LevelBrowse.Click += new System.EventHandler(this.LevelBrowse_Click);
            // 
            // Chibify
            // 
            this.Chibify.Location = new System.Drawing.Point(155, 367);
            this.Chibify.Name = "Chibify";
            this.Chibify.Size = new System.Drawing.Size(75, 23);
            this.Chibify.TabIndex = 8;
            this.Chibify.Text = "Chibify!";
            this.Chibify.UseVisualStyleBackColor = true;
            this.Chibify.Click += new System.EventHandler(this.Chibify_Click);
            // 
            // ChibifierGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 420);
            this.Controls.Add(this.Chibify);
            this.Controls.Add(this.LevelBrowse);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.LevelName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LevelPosition);
            this.Name = "ChibifierGui";
            this.Text = "Chibifier";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox LevelPosition;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox LevelName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button LevelBrowse;
        private System.Windows.Forms.OpenFileDialog LoadBrowser;
        private System.Windows.Forms.Button Chibify;
        private System.Windows.Forms.SaveFileDialog SaveBrowser;
    }
}

