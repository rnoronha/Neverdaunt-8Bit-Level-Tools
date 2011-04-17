namespace LevelModTools
{
    partial class AnimationTool
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
            this.FrameEntry = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.AnimationOutput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.AddFrame = new System.Windows.Forms.Button();
            this.CompressToggle = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.DataBlockUsageDisplay = new System.Windows.Forms.Label();
            this.CopyToClip = new System.Windows.Forms.Button();
            this.Clear = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.AnimLengthDisplay = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // FrameEntry
            // 
            this.FrameEntry.Location = new System.Drawing.Point(12, 38);
            this.FrameEntry.Multiline = true;
            this.FrameEntry.Name = "FrameEntry";
            this.FrameEntry.Size = new System.Drawing.Size(458, 84);
            this.FrameEntry.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Current frame";
            // 
            // AnimationOutput
            // 
            this.AnimationOutput.Location = new System.Drawing.Point(16, 168);
            this.AnimationOutput.Multiline = true;
            this.AnimationOutput.Name = "AnimationOutput";
            this.AnimationOutput.ReadOnly = true;
            this.AnimationOutput.Size = new System.Drawing.Size(454, 256);
            this.AnimationOutput.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 152);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Current animation";
            // 
            // AddFrame
            // 
            this.AddFrame.Location = new System.Drawing.Point(369, 128);
            this.AddFrame.Name = "AddFrame";
            this.AddFrame.Size = new System.Drawing.Size(101, 23);
            this.AddFrame.TabIndex = 4;
            this.AddFrame.Text = "Add to animation";
            this.AddFrame.UseVisualStyleBackColor = true;
            this.AddFrame.Click += new System.EventHandler(this.AddFrame_Click);
            // 
            // CompressToggle
            // 
            this.CompressToggle.Location = new System.Drawing.Point(369, 450);
            this.CompressToggle.Name = "CompressToggle";
            this.CompressToggle.Size = new System.Drawing.Size(101, 23);
            this.CompressToggle.TabIndex = 5;
            this.CompressToggle.Text = "Compress";
            this.CompressToggle.UseVisualStyleBackColor = true;
            this.CompressToggle.Click += new System.EventHandler(this.CompressToggle_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 460);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Data Block usage:";
            // 
            // DataBlockUsageDisplay
            // 
            this.DataBlockUsageDisplay.AutoSize = true;
            this.DataBlockUsageDisplay.Location = new System.Drawing.Point(114, 460);
            this.DataBlockUsageDisplay.Name = "DataBlockUsageDisplay";
            this.DataBlockUsageDisplay.Size = new System.Drawing.Size(21, 13);
            this.DataBlockUsageDisplay.TabIndex = 7;
            this.DataBlockUsageDisplay.Text = "0%";
            // 
            // CopyToClip
            // 
            this.CopyToClip.Location = new System.Drawing.Point(264, 450);
            this.CopyToClip.Name = "CopyToClip";
            this.CopyToClip.Size = new System.Drawing.Size(99, 23);
            this.CopyToClip.TabIndex = 8;
            this.CopyToClip.Text = "Copy to Clipboard";
            this.CopyToClip.UseVisualStyleBackColor = true;
            this.CopyToClip.Click += new System.EventHandler(this.CopyToClip_Click);
            // 
            // Clear
            // 
            this.Clear.Location = new System.Drawing.Point(168, 450);
            this.Clear.Name = "Clear";
            this.Clear.Size = new System.Drawing.Size(90, 23);
            this.Clear.TabIndex = 9;
            this.Clear.Text = "Clear Animation";
            this.Clear.UseVisualStyleBackColor = true;
            this.Clear.Click += new System.EventHandler(this.Clear_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 447);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Length of animation:";
            // 
            // AnimLengthDisplay
            // 
            this.AnimLengthDisplay.AutoSize = true;
            this.AnimLengthDisplay.Location = new System.Drawing.Point(114, 447);
            this.AnimLengthDisplay.Name = "AnimLengthDisplay";
            this.AnimLengthDisplay.Size = new System.Drawing.Size(13, 13);
            this.AnimLengthDisplay.TabIndex = 11;
            this.AnimLengthDisplay.Text = "0";
            // 
            // AnimationTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 482);
            this.Controls.Add(this.AnimLengthDisplay);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Clear);
            this.Controls.Add(this.CopyToClip);
            this.Controls.Add(this.DataBlockUsageDisplay);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.CompressToggle);
            this.Controls.Add(this.AddFrame);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.AnimationOutput);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FrameEntry);
            this.Name = "AnimationTool";
            this.Text = "AnimationTool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox FrameEntry;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox AnimationOutput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button AddFrame;
        private System.Windows.Forms.Button CompressToggle;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label DataBlockUsageDisplay;
        private System.Windows.Forms.Button CopyToClip;
        private System.Windows.Forms.Button Clear;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label AnimLengthDisplay;
    }
}