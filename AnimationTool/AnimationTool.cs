using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using N8Parser.Animation;

namespace LevelModTools
{
    public partial class AnimationTool : Form
    {
        private const int DataBlockSize = 5000;
        AnimationSequence annie;
        bool Compressed = false;

        public AnimationTool()
        {
            annie = new AnimationSequence();
            InitializeComponent();
        }

        private void AddFrame_Click(object sender, EventArgs e)
        {
            try
            {
                annie.AddFrame(new AnimationFrame(FrameEntry.Text));
                FrameEntry.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error adding that frame, please try again.\n The error was: \n" + ex.ToString());
            }

            UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            string animation;
            if (Compressed)
            {
                animation = annie.DeltaCompress().ToData();
            }
            else
            {
                animation = annie.ToData();
            }

            AnimationOutput.Text = animation;
            //Seriously, Microsoft? what the hell kind of format string is this?
            DataBlockUsageDisplay.Text = String.Format("{0:0.##}%", ((double)animation.Length / (double)DataBlockSize) * 100);
            AnimLengthDisplay.Text = annie.Length + "";
        }

        private void CompressToggle_Click(object sender, EventArgs e)
        {
            if (Compressed)
            {
                Compressed = false;
                CompressToggle.Text = "Compress";
            }
            else
            {
                Compressed = true;
                CompressToggle.Text = "Don't compress";
            }

            UpdateAnimation();
        }

        private void CopyToClip_Click(object sender, EventArgs e)
        {
            AnimationOutput.SelectAll();
            AnimationOutput.Copy();
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Do you want to delete the current animation?", "Bothering you", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                annie = new AnimationSequence();
                UpdateAnimation();
            }
        }
    }
}
