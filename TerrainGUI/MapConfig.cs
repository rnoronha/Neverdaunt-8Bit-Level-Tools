using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using N8Parser.Level_Modifiers;

namespace TerrainGUI
{
    public partial class MapConfig : Form
    {
        public Point Offset = new Point(0, 0);
        public Point Current = new Point(0, 0);
        Point DrawOffset = new Point(5, 15);
        int scale = 20;
        Bitmap Map;

        public MapConfig()
        {
            InitializeComponent();
            MapBox.MouseMove += new MouseEventHandler(MapBox_MouseMove);
            MapBox.Paint += new PaintEventHandler((x, y) => MapBox.CreateGraphics().DrawImage(Map ?? new Bitmap(0,0), DrawOffset));
            MapBox.Click += new EventHandler(MapBox_Click);
            //Clean up the bitmap when we're closed
            this.FormClosing += new FormClosingEventHandler((x,y)=>Map = null);
        }

        void MapBox_Click(object sender, EventArgs e)
        {
            Offset.X = Current.X - (int)X_Set.Value;
            Offset.Y = Current.Y + (int)Y_Set.Value;
        }

        void MapBox_MouseMove(object sender, MouseEventArgs e)
        {
            Current.X = (e.X - DrawOffset.X) / scale;
            Current.Y = (e.Y - DrawOffset.Y) / scale;

            X_Current.Text = (Current.X - Offset.X) + "";
            //N8's y axis is flipped, so the current Y is negative.
            Y_Current.Text = -(Current.Y - Offset.Y) + "";
        }

        internal void SetBitmap(Bitmap bitmap)
        {
            bitmap = Terrain.MinimizeMap(bitmap);
            bitmap = Terrain.OutlineMap(bitmap);
            bitmap = Terrain.TrimBounds(bitmap);
            bitmap = Terrain.ExpandMap(bitmap, scale);
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    int resI = i % scale;
                    int resJ = j % scale;
                    if ((resI == 0) || (resI == scale-1) || (resJ == 0) || (resJ == scale-1))
                    {
                        bitmap.SetPixel(i, j, Color.Black);
                    }
                }
            }
            this.Map = bitmap;

            //The magic numbers are approximations, but they're good enough I guess.
            //The lower limit is how small the box can be and still look good, the upper limit
            //is the raw map width + a drawing offset on both sides + a constant space requirement.
            this.Width = Math.Max(Map.Width + DrawOffset.X * 2 + 50, 251);
            this.Height = Math.Max(Map.Height + DrawOffset.Y * 2 + 150, 180);
            
        }

        private void Done_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
