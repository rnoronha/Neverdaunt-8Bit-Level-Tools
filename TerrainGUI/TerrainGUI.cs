using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using N8Parser.Level_Modifiers;
using N8Parser;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Media.Media3D;

namespace TerrainGUI
{
    public partial class TerrainGUI : Form
    {
        static BackgroundWorker bw = new BackgroundWorker();
        static OpenFileDialog opener = new OpenFileDialog();
        static SaveFileDialog saver  = new SaveFileDialog();
        static Graphics TerrainDrawing;
        public TerrainGUI()
        {
            InitializeComponent();
            this.Show();
            this.FormClosing += new FormClosingEventHandler((a, b) => { if (bw != null && bw.IsBusy) bw.CancelAsync(); });
            TerrainDrawing = TerrainImage.CreateGraphics();
        }

        private void DoTerrain(Graphics g, Bitmap Outline, int seed = 0)
        {
            HashSet<Point> holds = Terrain.BitmapToHolds(Outline);
            Random rand = new Random(seed);

            Heightmap MapSeed;

            if (KeepCurrentSeed.Enabled)
            {
                MapSeed = Terrain.Map;
            }
            else
            {
                MapSeed = new Heightmap(Outline.Width, Outline.Height);
            }

            SortedList<double, Action> actions = new SortedList<double, Action>();
            actions.Add(0.0001, () => Terrain.Normalize(-5000, 5000));
            actions.Add(0.005, () => Terrain.Hill(1, 0.5, 50, true, rand));
            actions.Add(0.009, () => Terrain.Fault(0.005, rand));
            actions.Add(0.01, () => Terrain.Hill(2, 0.5, 25, false, rand));
            actions.Add(0.75, () => Terrain.Hill(2, 0.5, 10, false, rand));
            actions.Add(0.25, () => Terrain.Hill(2, 0.5, 10, true, rand));
            Heightmap res = Terrain.Arbitrary(MapSeed, actions, 100000, holds, rand, (a) =>
                {
                    if (a % 1000 == 0)
                    {
                        bw.ReportProgress(a, Terrain.Map.ToHeatmap(10, Outline));
                    }
                    return !bw.CancellationPending;
                });
            Console.WriteLine("Max is: " + res.max() + ", min is: " + res.min());
            if (ShowBitmap.Checked)
            {
                res.ShowHeatmap(10, Outline);
            }
            Console.ReadLine();
        }

        public void DrawBitmap(Graphics g, Bitmap bits)
        {
            try
            {
                g.DrawImage(bits, 5, 10);
            }
            catch (ExternalException)
            {
                //Well we're screwed, not much we can do besides return.
            }
        }

        private void Materialize_Click(object sender, EventArgs e)
        {
            Terrain.Map.ShowHeatmap();
            Terrain.Map.normalize(-500, 2000);
            //Flip the heightmap's Y coordinates, because N8 uses y+ to mean up-screen and 
            //everything else we've been doing so far uses Y+ to mean down-screen
            Terrain.Map.Map.ForEach((x) => x.Reverse());

            List<List<Heightmap>> maps = Terrain.ChopHeightmap(10, Terrain.Map);

            Random rand = new Random();
            for (int k = 0; k < maps.Count; k++)
            {
                for (int l = 0; l < maps[k].Count; l++)
                {
                    N8Level Level = new N8Level();
                    Vector3D LowestPoint = new Vector3D(0, 0, int.MaxValue);
                    for (int i = 1; i <= 10; i++)
                    {
                        for (int j = 1; j <= 10; j++)
                        {
                            N8Block land = Level.blocks.GenerateBlock("land", "Terrain");
                            land.position.X = ((i - 5) * 400) - 200;
                            land.position.Y = ((j - 5) * 400) - 200;
                            land.position.Z = maps[k][l][i, j];

                            //Find the lowest point in the cell, but restrict it to near the center
                            if (Math.Abs(land.position.X) < 1000 && Math.Abs(land.position.Y) < 1000)
                            {
                                if (land.position.Z < LowestPoint.Z)
                                {
                                    LowestPoint = land.position;
                                }
                            }
                        }
                    }

                    for (int i = -1000; i < LowestPoint.Z; i += 70)
                    {
                        N8Block stack = Level.blocks.GenerateBlock("floor", "Stack");
                        stack.position = LowestPoint;
                        stack.position.Z = i;
                    }

                    for (int i = 0; i < 75; i++)
                    {
                        //Pick a random piece of land
                        Vector3D land = rand.NextVector(new Vector3D(1, 1, 0), new Vector3D(10, 10, 0));

                        //Put the tree's position in the middle of it
                        Vector3D TreePos = new Vector3D();
                        TreePos.X = ((land.X - 5) * 400) - 200;
                        TreePos.Y = ((land.Y - 5) * 400) - 200;
                        TreePos.Z = maps[k][l][(int)land.X, (int)land.Y];

                        //And add a random offset to put it somewhere on the land
                        //(with a random Z offset to keep z-fighting down)
                        Vector3D Offset = rand.NextVector(new Vector3D(-200, -200, 60), new Vector3D(200, 200, 70));

                        //Give it a random spin
                        Quaternion rot = new Quaternion(new Vector3D(0, 0, 1), rand.Next(360));

                        //And finally materialize the tree
                        N8Block tree = Level.blocks.GenerateBlock(Forest.TreeTypes[rand.Next() % Forest.TreeTypes.Length], Forest.TreeNames[i % Forest.TreeNames.Length]);
                        tree.position = TreePos + Offset;
                        tree.rotation = rot;
                    }
                    //Magic numbers :( there's no real way to get the map offset without calculating it by hand - the various relative positions of locked cells can change without notice.
                    //Also keep in mind that N8 uses Y+ and Y- in exactly the opposite way of everything else, hence why that term is flopped and why the y coords get reversed up above.
                    Utilities.Save(Utilities.GetDefaultSaveFolder() + @"terrain\" + (k - 17) + "," + (l - 11) + ".ncd", Level);
                }
            }

            Terrain.Map.ToHeatmap(10, null).Save(Utilities.GetDefaultSaveFolder() + @"terrain\heatmap.png", System.Drawing.Imaging.ImageFormat.Png);
            Terrain.Map.ToBitmap().Save(Utilities.GetDefaultSaveFolder() + @"terrain\heightmap.png", System.Drawing.Imaging.ImageFormat.Png);
            MessageBox.Show("Done!");
        }

        private void Run_Click(object sender, EventArgs e)
        {
            Run.Enabled = false;
            Materialize.Enabled = false;
            Stop.Enabled = true;
            SeedEntry.Enabled = true;
            FileMenu.Enabled = false;

            Bitmap temp = GetOutline();

            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.ProgressChanged += new ProgressChangedEventHandler((a, b) => { DrawBitmap(TerrainDrawing, (Bitmap)b.UserState); IterCounter.Text = b.ProgressPercentage + ""; });
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

            bw.DoWork += new DoWorkEventHandler((a, b) => DoTerrain(TerrainDrawing, temp, (int)SeedEntry.Value));
            bw.RunWorkerAsync();
        }

        private static Bitmap GetOutline()
        {

            Bitmap temp = Terrain.MinimizeMap(@"C:\Projects\N8 Parser\Map.png");
            temp = Terrain.ProcessMap(temp);
            temp = Terrain.TrimBounds(temp);
            temp = Terrain.ExpandMap(temp);
            return temp;
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            saveHeightmapToolStripMenuItem.Enabled = true;
            Materialize.Enabled = true; 
            Run.Enabled = true; 
            Stop.Enabled = false; 
            SeedEntry.Enabled = true;
            KeepCurrentSeed.Enabled = true;
            FileMenu.Enabled = true;
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            bw.CancelAsync();
        }

        private void loadHeightmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            opener.Filter = "PNG images|*.png";
            if (opener.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Terrain.Map = new Heightmap(new Bitmap(opener.FileName));
            }

            DrawBitmap(TerrainDrawing, Terrain.Map.ToHeatmap(10, GetOutline()));
            KeepCurrentSeed.Enabled = true;
        }

        private void saveHeightmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saver.Filter = "PNG image|*.png";
            if (saver.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Terrain.Map.ToBitmap().Save(saver.FileName);
                MessageBox.Show("Saved!");
            }
        }
    }
}
