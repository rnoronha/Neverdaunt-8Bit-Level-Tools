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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.label15 = new System.Windows.Forms.Label();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.CustomGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EntryY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EntryZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EntryX)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            this.groupBox1.SuspendLayout();
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
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 9);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(466, 329);
            this.tabControl1.TabIndex = 12;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.label15);
            this.tabPage1.Controls.Add(this.radioButton1);
            this.tabPage1.Controls.Add(this.radioButton2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(458, 303);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(192, 74);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(7, 97);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown1.TabIndex = 2;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(7, 62);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.numericUpDown2.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown2.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(185, 266);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 23;
            this.button1.Text = "Push!";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.Location = new System.Drawing.Point(7, 20);
            this.numericUpDown3.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numericUpDown3.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            -2147483648});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown3.TabIndex = 0;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(134, 26);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(14, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "X";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(338, 227);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 22;
            this.button2.Text = "Browse";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(113, 229);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(219, 20);
            this.textBox1.TabIndex = 21;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(71, 232);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(36, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Level:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(151, 15);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(20, 13);
            this.label11.TabIndex = 19;
            this.label11.Text = "+Y";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(151, 149);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(17, 13);
            this.label12.TabIndex = 18;
            this.label12.Text = "-Y";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(133, 64);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(14, 13);
            this.label13.TabIndex = 4;
            this.label13.Text = "Z";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(-139, 68);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(17, 13);
            this.label14.TabIndex = 16;
            this.label14.Text = "-X";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(128, 169);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(20, 13);
            this.label15.TabIndex = 17;
            this.label15.Text = "+X";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(129, -114);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(110, 17);
            this.radioButton1.TabIndex = 15;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Use custom offset";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(-139, -114);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(117, 17);
            this.radioButton2.TabIndex = 14;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Use standard offset";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(134, 104);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(14, 13);
            this.label16.TabIndex = 5;
            this.label16.Text = "Y";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.numericUpDown1);
            this.groupBox1.Controls.Add(this.numericUpDown2);
            this.groupBox1.Controls.Add(this.numericUpDown3);
            this.groupBox1.Location = new System.Drawing.Point(258, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 156);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Custom offset";
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(-11, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(156, 156);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Standard offset";
            // 
            // LevelPusher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 339);
            this.Controls.Add(this.tabControl1);
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
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}

