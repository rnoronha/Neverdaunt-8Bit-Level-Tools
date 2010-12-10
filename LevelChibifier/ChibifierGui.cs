using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using N8Parser.Level_Modifiers;

namespace LevelChibifier
{
    public partial class ChibifierGui : Form
    {
        string CurrentFilename = "";
        int currentX = 0;
        int currentY = 0;
        string DefaultSavePath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\N8\Saves";

        public ChibifierGui()
        {
            InitializeComponent();
            RadioButton[] butans = new RadioButton[25];
            int count = 0;
            for (int i = 25; i < 250; i += 50)
            {
                for (int j = 25; j < 250; j += 50)
                {
                    RadioButton b = new RadioButton();
                    b.AutoSize = true;
                    b.Location = new System.Drawing.Point(i, j);
                    b.Name = "X = " + i + " Y = " + j;
                    b.Size = new System.Drawing.Size(14, 13);
                    b.TabIndex = count;
                    b.UseVisualStyleBackColor = true;

                    //Have to make new variables, otherwise the lambda makes it remember i and j outside of their apparent scope.
                    //As long as we're doing that, normalize it here.
                    int x = ((i - 25) / 50) - 2;
                    int y = -(((j - 25) / 50) - 2);

                    //The joys of lambdas
                    b.CheckedChanged += (sender, args) => GridChange((RadioButton)sender, x, y);
                    if (i == 125 && j == 125)
                    {
                        b.Checked = true;
                    }


                    butans[count] = b;
                    LevelPosition.Controls.Add(b);
                    count++;
                }
            }

            LevelName.Click += new EventHandler(LevelName_Click);
        }

        void LevelName_Click(object sender, EventArgs e)
        {
            if (LevelName.Text.Trim().Length == 0)
            {
                LevelBrowse_Click(null, null);
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

        private void LevelBrowse_Click(object sender, EventArgs e)
        {
            LoadBrowser.InitialDirectory = DefaultSavePath;

            if (LoadBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CurrentFilename = LoadBrowser.FileName;
                LevelName.Text = Path.GetFileNameWithoutExtension(CurrentFilename);
            }

        }

        private void Chibify_Click(object sender, EventArgs e)
        {
            SaveBrowser.InitialDirectory = DefaultSavePath;
            SaveBrowser.FileName = Path.GetFileNameWithoutExtension(CurrentFilename) + "_chibi.ncd";
            if (CurrentFilename.Trim().Length != 0)
            {
                if (SaveBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Chibifier.ChibifyLevel(CurrentFilename, SaveBrowser.FileName, currentX, currentY);
                    MessageBox.Show("Done chibifying!");
                }
            }
            else
            {
                MessageBox.Show("Pick a level to chibify first!");
            }
        }

    }
}
