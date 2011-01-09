namespace LevelModTools
{
    partial class LevelPusher
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
            this.StandardGroup = new System.Windows.Forms.GroupBox();
            this.CustomGroup = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.EntryY = new System.Windows.Forms.NumericUpDown();
            this.EntryZ = new System.Windows.Forms.NumericUpDown();
            this.EntryX = new System.Windows.Forms.NumericUpDown();
            this.StandardOffset = new System.Windows.Forms.RadioButton();
            this.CustomOffset = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.LevelName = new System.Windows.Forms.TextBox();
            this.LevelBrowse = new System.Windows.Forms.Button();
            this.Push = new System.Windows.Forms.Button();
            this.LoadBrowser = new System.Windows.Forms.OpenFileDialog();
            this.SaveBrowser = new System.Windows.Forms.SaveFileDialog();
            this.CustomGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EntryY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EntryZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EntryX)).BeginInit();
            this.SuspendLayout();
            // 
            // StandardGroup
            // 
            this.StandardGroup.Location = new System.Drawing.Point(12, 32);
            this.StandardGroup.Name = "StandardGroup";
            this.StandardGroup.Size = new System.Drawing.Size(156, 156);
            this.StandardGroup.TabIndex = 0;
            this.StandardGroup.TabStop = false;
            this.StandardGroup.Text = "Standard offset";
            // 
            // CustomGroup
            // 
            this.CustomGroup.Controls.Add(this.label3);
            this.CustomGroup.Controls.Add(this.label2);
            this.CustomGroup.Controls.Add(this.label1);
            this.CustomGroup.Controls.Add(this.EntryY);
            this.CustomGroup.Controls.Add(this.EntryZ);
            this.CustomGroup.Controls.Add(this.EntryX);
            this.CustomGroup.Location = new System.Drawing.Point(281, 32);
            this.CustomGroup.Name = "CustomGroup";
            this.CustomGroup.Size = new System.Drawing.Size(203, 156);
            this.CustomGroup.TabIndex = 1;
            this.CustomGroup.TabStop = false;
            this.CustomGroup.Text = "Custom offset";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(134, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Y";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(133, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Z";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(134, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "X";
            // 
            // EntryY
            // 
            this.EntryY.Location = new System.Drawing.Point(7, 97);
            this.EntryY.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.EntryY.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.EntryY.Name = "EntryY";
            this.EntryY.Size = new System.Drawing.Size(120, 20);
            this.EntryY.TabIndex = 2;
            // 
            // EntryZ
            // 
            this.EntryZ.Location = new System.Drawing.Point(7, 62);
            this.EntryZ.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.EntryZ.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.EntryZ.Name = "EntryZ";
            this.EntryZ.Size = new System.Drawing.Size(120, 20);
            this.EntryZ.TabIndex = 1;
            // 
            // EntryX
            // 
            this.EntryX.Location = new System.Drawing.Point(7, 20);
            this.EntryX.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.EntryX.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.EntryX.Name = "EntryX";
            this.EntryX.Size = new System.Drawing.Size(120, 20);
            this.EntryX.TabIndex = 0;
            // 
            // StandardOffset
            // 
            this.StandardOffset.AutoSize = true;
            this.StandardOffset.Location = new System.Drawing.Point(13, 13);
            this.StandardOffset.Name = "StandardOffset";
            this.StandardOffset.Size = new System.Drawing.Size(117, 17);
            this.StandardOffset.TabIndex = 2;
            this.StandardOffset.TabStop = true;
            this.StandardOffset.Text = "Use standard offset";
            this.StandardOffset.UseVisualStyleBackColor = true;
            // 
            // CustomOffset
            // 
            this.CustomOffset.AutoSize = true;
            this.CustomOffset.Location = new System.Drawing.Point(281, 13);
            this.CustomOffset.Name = "CustomOffset";
            this.CustomOffset.Size = new System.Drawing.Size(110, 17);
            this.CustomOffset.TabIndex = 3;
            this.CustomOffset.TabStop = true;
            this.CustomOffset.Text = "Use custom offset";
            this.CustomOffset.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 195);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "-X";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(151, 195);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "+X";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(174, 175);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "-Y";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(174, 41);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(20, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "+Y";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(94, 258);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(36, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Level:";
            // 
            // LevelName
            // 
            this.LevelName.Location = new System.Drawing.Point(136, 255);
            this.LevelName.Name = "LevelName";
            this.LevelName.Size = new System.Drawing.Size(219, 20);
            this.LevelName.TabIndex = 9;
            this.LevelName.TextChanged += new System.EventHandler(this.LevelName_TextChanged);
            // 
            // LevelBrowse
            // 
            this.LevelBrowse.Location = new System.Drawing.Point(361, 253);
            this.LevelBrowse.Name = "LevelBrowse";
            this.LevelBrowse.Size = new System.Drawing.Size(75, 23);
            this.LevelBrowse.TabIndex = 10;
            this.LevelBrowse.Text = "Browse";
            this.LevelBrowse.UseVisualStyleBackColor = true;
            this.LevelBrowse.Click += new System.EventHandler(this.LevelBrowse_Click);
            // 
            // Push
            // 
            this.Push.Location = new System.Drawing.Point(208, 292);
            this.Push.Name = "Push";
            this.Push.Size = new System.Drawing.Size(75, 23);
            this.Push.TabIndex = 11;
            this.Push.Text = "Push!";
            this.Push.UseVisualStyleBackColor = true;
            this.Push.Click += new System.EventHandler(this.Push_Click);
            // 
            // LoadBrowser
            // 
            this.LoadBrowser.FileName = "openFileDialog1";
            // 
            // LevelPusher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 339);
            this.Controls.Add(this.Push);
            this.Controls.Add(this.LevelBrowse);
            this.Controls.Add(this.LevelName);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.CustomOffset);
            this.Controls.Add(this.StandardOffset);
            this.Controls.Add(this.CustomGroup);
            this.Controls.Add(this.StandardGroup);
            this.Name = "LevelPusher";
            this.Text = "Level Pusher";
            this.CustomGroup.ResumeLayout(false);
            this.CustomGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EntryY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EntryZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EntryX)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox StandardGroup;
        private System.Windows.Forms.GroupBox CustomGroup;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown EntryY;
        private System.Windows.Forms.NumericUpDown EntryZ;
        private System.Windows.Forms.NumericUpDown EntryX;
        private System.Windows.Forms.RadioButton StandardOffset;
        private System.Windows.Forms.RadioButton CustomOffset;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox LevelName;
        private System.Windows.Forms.Button LevelBrowse;
        private System.Windows.Forms.Button Push;
        private System.Windows.Forms.OpenFileDialog LoadBrowser;
        private System.Windows.Forms.SaveFileDialog SaveBrowser;
    }
}

