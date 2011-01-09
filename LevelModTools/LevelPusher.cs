using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using N8Parser;
using System.IO;
using System.Windows.Media.Media3D;

namespace LevelModTools
{
    public partial class LevelPusher : Form
    {
        List<RadioButton> RadioButtons = new List<RadioButton>(9);
        int currentX;
        int currentY;
        string CurrentFilename = "";

        public LevelPusher()
        {
            InitializeComponent();
            int count = 0;
            for (int i = 25; i < 150; i += 50)
            {
                for (int j = 25; j < 150; j += 50)
                {
                    RadioButton b = new RadioButton();
                    b.AutoSize = true;
                    b.Location = new System.Drawing.Point(i, j);
                    b.Name = "X = " + i + " Y = " + j;
                    b.Size = new System.Drawing.Size(14, 13);
                    b.TabIndex = count;
                    b.UseVisualStyleBackColor = true;

                    //Same sort of thing as in the LevelChibifier gui, except we're only
                    //doing it three times here.
                    int x = ((i - 25) / 50) - 1;
                    int y = -(((j - 25) / 50) - 1);

                    b.CheckedChanged += (sender, args) => GridChange((RadioButton)sender, x, y);
                    if (i == 75 && j == 75)
                    {
                        b.Checked = true;
                    }


                    RadioButtons.Add(b);
                    StandardGroup.Controls.Add(b);
                    count++;
                }
            }
            StandardOffset.CheckedChanged += new EventHandler(StandardOffset_CheckedChanged);
            CustomOffset.CheckedChanged += new EventHandler(CustomOffset_CheckedChanged);
            StandardOffset.Select();
            CustomGroup.Enabled = false;
        }

        void CustomOffset_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                StandardGroup.Enabled = false;
                CustomGroup.Enabled = true;
            }
        }

        void StandardOffset_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                StandardGroup.Enabled = true;
                CustomGroup.Enabled = false;
            }
        }
        void GridChange(RadioButton sender, int x, int y)
        {
            if (sender.Checked)
            {
                currentX = x;
                currentY = y;
            }
        }

        private void LevelName_TextChanged(object sender, EventArgs e)
        {
            if (LevelName.Text.Trim().Length == 0)
            {
                LevelBrowse_Click(null, null);
            }
        }

        private void LevelBrowse_Click(object sender, EventArgs e)
        {
            LoadBrowser.InitialDirectory = Utilities.GetDefaultSaveFolder();

            if (LoadBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CurrentFilename = LoadBrowser.FileName;
                LevelName.Text = Path.GetFileNameWithoutExtension(CurrentFilename);
            }


        }

        private void Push_Click(object sender, EventArgs e)
        {
            SaveBrowser.InitialDirectory = Utilities.GetDefaultSaveFolder();
            SaveBrowser.FileName = Path.GetFileNameWithoutExtension(CurrentFilename) + "_moved.ncd";
            if (CurrentFilename.Trim().Length != 0)
            {
                if (SaveBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    int translateX;
                    int translateY;
                    int translateZ;

                    if (StandardGroup.Enabled)
                    {
                        translateZ = 0;
                        translateX = currentX * 1000;
                        translateY = currentY * 1000;
                    }
                    else if (CustomGroup.Enabled)
                    {
                        translateZ = (int)EntryZ.Value;
                        translateX = (int)EntryX.Value;
                        translateY = (int)EntryY.Value;
                    }
                    else
                    {
                        throw new Exception("Whoops, it looks like neither group was enabled?");
                    }

                    N8Parser.Level_Modifiers.Translator.TranslateLevel(
                        LoadBrowser.FileName, new Vector3D(translateX, translateY, translateZ), 
                        SaveBrowser.FileName);
                    
                    MessageBox.Show("Done pushing!");
                }
            }
            else
            {
                MessageBox.Show("Pick a level to push first!");
            }
        }

    }
}
