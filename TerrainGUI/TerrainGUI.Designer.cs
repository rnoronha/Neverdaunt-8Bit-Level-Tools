namespace TerrainGUI
{
    partial class TerrainGUI
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
            this.IterCounter = new System.Windows.Forms.Label();
            this.Materialize = new System.Windows.Forms.Button();
            this.Run = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SeedEntry = new System.Windows.Forms.NumericUpDown();
            this.Stop = new System.Windows.Forms.Button();
            this.ShowBitmap = new System.Windows.Forms.CheckBox();
            this.KeepCurrentSeed = new System.Windows.Forms.CheckBox();
            this.TerrainImage = new System.Windows.Forms.GroupBox();
            this.FileMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadHeightmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveHeightmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadN8MapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.SeedEntry)).BeginInit();
            this.FileMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 331);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Iteration:";
            // 
            // IterCounter
            // 
            this.IterCounter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.IterCounter.AutoSize = true;
            this.IterCounter.Location = new System.Drawing.Point(66, 331);
            this.IterCounter.Name = "IterCounter";
            this.IterCounter.Size = new System.Drawing.Size(13, 13);
            this.IterCounter.TabIndex = 1;
            this.IterCounter.Text = "0";
            // 
            // Materialize
            // 
            this.Materialize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Materialize.Enabled = false;
            this.Materialize.Location = new System.Drawing.Point(15, 352);
            this.Materialize.Name = "Materialize";
            this.Materialize.Size = new System.Drawing.Size(75, 23);
            this.Materialize.TabIndex = 2;
            this.Materialize.Text = "Materialize";
            this.Materialize.UseVisualStyleBackColor = true;
            this.Materialize.Click += new System.EventHandler(this.Materialize_Click);
            // 
            // Run
            // 
            this.Run.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Run.Location = new System.Drawing.Point(198, 352);
            this.Run.Name = "Run";
            this.Run.Size = new System.Drawing.Size(75, 23);
            this.Run.TabIndex = 3;
            this.Run.Text = "Run";
            this.Run.UseVisualStyleBackColor = true;
            this.Run.Click += new System.EventHandler(this.Run_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(157, 334);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Seed:";
            // 
            // SeedEntry
            // 
            this.SeedEntry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SeedEntry.Location = new System.Drawing.Point(198, 332);
            this.SeedEntry.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.SeedEntry.Name = "SeedEntry";
            this.SeedEntry.Size = new System.Drawing.Size(75, 20);
            this.SeedEntry.TabIndex = 6;
            // 
            // Stop
            // 
            this.Stop.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.Stop.Enabled = false;
            this.Stop.Location = new System.Drawing.Point(106, 352);
            this.Stop.Name = "Stop";
            this.Stop.Size = new System.Drawing.Size(75, 23);
            this.Stop.TabIndex = 7;
            this.Stop.Text = "Stop";
            this.Stop.UseVisualStyleBackColor = true;
            this.Stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // ShowBitmap
            // 
            this.ShowBitmap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ShowBitmap.AutoSize = true;
            this.ShowBitmap.Location = new System.Drawing.Point(12, 311);
            this.ShowBitmap.Name = "ShowBitmap";
            this.ShowBitmap.Size = new System.Drawing.Size(143, 17);
            this.ShowBitmap.TabIndex = 8;
            this.ShowBitmap.Text = "Show bitmap when done";
            this.ShowBitmap.UseVisualStyleBackColor = true;
            // 
            // KeepCurrentSeed
            // 
            this.KeepCurrentSeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.KeepCurrentSeed.AutoSize = true;
            this.KeepCurrentSeed.Enabled = false;
            this.KeepCurrentSeed.Location = new System.Drawing.Point(12, 288);
            this.KeepCurrentSeed.Name = "KeepCurrentSeed";
            this.KeepCurrentSeed.Size = new System.Drawing.Size(195, 17);
            this.KeepCurrentSeed.TabIndex = 9;
            this.KeepCurrentSeed.Text = "Start next sequence from this terrain";
            this.KeepCurrentSeed.UseVisualStyleBackColor = true;
            // 
            // TerrainImage
            // 
            this.TerrainImage.Location = new System.Drawing.Point(12, 33);
            this.TerrainImage.Name = "TerrainImage";
            this.TerrainImage.Size = new System.Drawing.Size(261, 249);
            this.TerrainImage.TabIndex = 10;
            this.TerrainImage.TabStop = false;
            // 
            // FileMenu
            // 
            this.FileMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.FileMenu.Location = new System.Drawing.Point(0, 0);
            this.FileMenu.Name = "FileMenu";
            this.FileMenu.Size = new System.Drawing.Size(285, 24);
            this.FileMenu.TabIndex = 11;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadHeightmapToolStripMenuItem,
            this.saveHeightmapToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadHeightmapToolStripMenuItem
            // 
            this.loadHeightmapToolStripMenuItem.Name = "loadHeightmapToolStripMenuItem";
            this.loadHeightmapToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.loadHeightmapToolStripMenuItem.Text = "Load heightmap";
            this.loadHeightmapToolStripMenuItem.Click += new System.EventHandler(this.loadHeightmapToolStripMenuItem_Click);
            // 
            // saveHeightmapToolStripMenuItem
            // 
            this.saveHeightmapToolStripMenuItem.Enabled = false;
            this.saveHeightmapToolStripMenuItem.Name = "saveHeightmapToolStripMenuItem";
            this.saveHeightmapToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.saveHeightmapToolStripMenuItem.Text = "Save heightmap";
            this.saveHeightmapToolStripMenuItem.Click += new System.EventHandler(this.saveHeightmapToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadN8MapToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // loadN8MapToolStripMenuItem
            // 
            this.loadN8MapToolStripMenuItem.Name = "loadN8MapToolStripMenuItem";
            this.loadN8MapToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadN8MapToolStripMenuItem.Text = "Load N8 map";
            this.loadN8MapToolStripMenuItem.Click += new System.EventHandler(this.loadN8MapToolStripMenuItem_Click);
            // 
            // TerrainGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(285, 387);
            this.Controls.Add(this.TerrainImage);
            this.Controls.Add(this.KeepCurrentSeed);
            this.Controls.Add(this.ShowBitmap);
            this.Controls.Add(this.Stop);
            this.Controls.Add(this.SeedEntry);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Run);
            this.Controls.Add(this.Materialize);
            this.Controls.Add(this.IterCounter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FileMenu);
            this.Name = "TerrainGUI";
            this.Text = "TerrainGUI";
            ((System.ComponentModel.ISupportInitialize)(this.SeedEntry)).EndInit();
            this.FileMenu.ResumeLayout(false);
            this.FileMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label IterCounter;
        private System.Windows.Forms.Button Materialize;
        private System.Windows.Forms.Button Run;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown SeedEntry;
        private System.Windows.Forms.Button Stop;
        private System.Windows.Forms.CheckBox ShowBitmap;
        private System.Windows.Forms.CheckBox KeepCurrentSeed;
        private System.Windows.Forms.GroupBox TerrainImage;
        private System.Windows.Forms.MenuStrip FileMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadHeightmapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveHeightmapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadN8MapToolStripMenuItem;
    }
}

