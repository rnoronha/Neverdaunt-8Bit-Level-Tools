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
        static MapConfig map = new MapConfig();
        static BackgroundWorker bw = new BackgroundWorker();
        static OpenFileDialog opener = new OpenFileDialog();
        static SaveFileDialog saver  = new SaveFileDialog();
        static Graphics TerrainDrawing;
        static Point BitmapOffset = new Point(17, 11);
        static Bitmap N8Map;

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

            MapSeed.Holds = holds;
            /*
            SortedList<double, Action> actions = new SortedList<double, Action>();
            actions.Add(0.0001, () => Terrain.Normalize(-5000, 5000));
            actions.Add(0.005, () => Terrain.Hill(1, 0.5, 50, true, rand));
            actions.Add(0.009, () => Terrain.Fault(0.005, rand));
            actions.Add(0.01, () => Terrain.Hill(2, 0.5, 25, false, rand));
            actions.Add(0.75, () => Terrain.Hill(2, 0.5, 10, false, rand));
            actions.Add(0.25, () => Terrain.Hill(2, 0.5, 10, true, rand));
            Heightmap res = Terrain.Arbitrary(MapSeed, actions, 100000, rand, (a) =>
                {
                    if (a % 1000 == 0)
                    {
                        bw.ReportProgress(a, Terrain.Map.ToHeatmap(10, Outline));
                    }
                    return !bw.CancellationPending;
                });
            if (ShowBitmap.Checked)
            {
                res.ShowHeatmap(10, Outline);
            }
            */
            Terrain.Map = MapSeed;
            int sizeX = Terrain.Map.sizeX;
            int sizeY = Terrain.Map.sizeY;

            int radiusX = 15;
            int radiusY = 15;

            //Pick a point
            int x = sizeX / 2;
            int y = sizeY / 2;


            //And then gradually raise a hill at that point
            for (int i = x - radiusX; i < x + radiusX; i++)
            {
                for (int j = y - radiusY; j < y + radiusY; j++)
                {
                    int mag = radiusX * radiusY - ((i - x) * (i - x) + (j - y) * (j - y));
                    if (mag >= 0)
                    {
                        Terrain.Map[i, j] += (int)(mag);
                    }
                }
            }
            bw.ReportProgress(1, Terrain.Map.ToHeatmap(10, Outline));
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
            Terrain.Map.normalize(-500, 1000);
            //Flip the heightmap's Y coordinates, because N8 uses y+ to mean up-screen and 
            //everything else we've been doing so far uses Y+ to mean down-screen
            Terrain.Map.Map.ForEach((x) => x.Reverse());

            int SquareSize = 10;

            List<List<Heightmap>> maps = Terrain.ChopHeightmap(SquareSize, Terrain.Map);

            Random rand = new Random();
            for (int k = 0; k < maps.Count; k++)
            {
                for (int l = 0; l < maps[k].Count; l++)
                {
                    N8Level Level = new N8Level();
                    List<N8Block> lands = new List<N8Block>(100);
                    Vector3D LowestPoint = new Vector3D(0, 0, int.MaxValue);
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            N8Block land = Level.blocks.GenerateBlock("land", "Terrain");
                            land.position.X = ((i - 4) * 400) - 200;
                            land.position.Y = ((j - 4) * 400) - 200;
                            land.position.Z = maps[k][l][i, j];

                            //Find the lowest point in the cell, but restrict it to near the center
                            if (Math.Abs(land.position.X) < 1000 && Math.Abs(land.position.Y) < 1000)
                            {
                                if (land.position.Z < LowestPoint.Z)
                                {
                                    LowestPoint = land.position;
                                }
                            }
                            //Calculate the rotation - what we do is figure out what the 
                            //angle is between the south point and the  north point,
                            //then multiply that by the angle between the east point and west point.

                            //North/South is Y +- 1, East/West is X +- 1
                            //We pull height information directly from the original heightmap because on the edges we'll need to get data from a different square.
                            Vector3D North = new Vector3D(land.position.X, ((((j + 1) - 4) * 400)), Terrain.Map[k * SquareSize + i, l * SquareSize + (j + 1)]);
                            Vector3D South = new Vector3D(land.position.X, ((((j - 1) - 4) * 400)), Terrain.Map[k * SquareSize + i, l * SquareSize + (j - 1)]);
                            Vector3D East  = new Vector3D((((i + 1) - 4) * 400), land.position.Y, Terrain.Map[k * SquareSize + (i + 1), l * SquareSize + j]);
                            Vector3D West  = new Vector3D((((i - 1) - 4) * 400), land.position.Y, Terrain.Map[k * SquareSize + (i - 1), l * SquareSize + j]);;
                            Vector3D LandPos = land.position;


                            Quaternion NSRot = new Quaternion(0, 0, 0, 1);
                            Quaternion EWRot = new Quaternion(0, 0, 0, 1);
                            LandPos.Normalize();

                            //If I'm between two things that are both higher or both lower than me, I shouldn't have a tilt in that direction.
                            //E.G: if I'm like this : _-_ or -_-, I shouldn't have a tilt.
                            if ((North.Z < land.position.Z && land.position.Z < South.Z) || (North.Z > land.position.Z && land.position.Z > South.Z))
                            {
                                if ((North - South).Length > 400)
                                {
             //                       land.type = "landmega";
                                }
                                //Normalize them both and then take cos-1 of their yz dot product to get the degree of rotation
                                North.Normalize();
                                South.Normalize();

                                NSRot = new Quaternion(new Vector3D(0, 1, 0), Math.Acos((LandPos.Z * South.Z + LandPos.Y * South.Y)) * Utilities.RadToDeg);
                                
                            }

                            if ((East.Z < land.position.Z && land.position.Z < West.Z) || (West.Z < land.position.Z && land.position.Z < East.Z))
                            {
                                if ((East - West).Length > 400)
                                {
               //                     land.type = "landmega";
                                }
                                East.Normalize();
                                West.Normalize();
                                EWRot = new Quaternion(new Vector3D(0, 1, 0), Math.Acos((LandPos.Z * West.Z + LandPos.X * West.X)) * Utilities.RadToDeg);
                            }

                            land.rotation = EWRot;

                            lands.Add(land);
                        }
                    }
                    /*
                    #region Extra nice things
                    //Now add some nice things like a stack and random trees
                    
                    for (int i = -1000; i < LowestPoint.Z; i += 70)
                    {
                        N8Block stack = Level.blocks.GenerateBlock("floor", "Stack");
                        stack.position = LowestPoint;
                        stack.position.Z = i;
                    }
                    
                    for (int i = 0; i < 75; i++)
                    {
                        //Pick a random piece of land
                        N8Block currentLand = lands[rand.Next(lands.Count)];

                        //Put the tree's position in the middle of it
                        Vector3D TreePos = new Vector3D();
                        TreePos = rand.NextVector(new Vector3D(-200, -200, 60), new Vector3D(200, 200, 70));

                        //Give it a random spin
                        Quaternion rot = new Quaternion(new Vector3D(0, 0, 1), rand.Next(360));

                        //And finally materialize the tree
                        N8Block tree = Level.blocks.GenerateBlock(Forest.TreeTypes[rand.Next() % Forest.TreeTypes.Length], Forest.TreeNames[rand.Next() % Forest.TreeNames.Length]);
                        tree.position = TreePos;
                        tree.rotation = rot;

                        currentLand.AttachToMe(tree);
                    }
                    #endregion
                     */
                    Utilities.Save(Utilities.GetDefaultSaveFolder() + @"terrain\" + (k - BitmapOffset.X) + "," + (l - BitmapOffset.Y) + ".ncd", Level);
                }
            }
            //Unflip it, so we're back where we started
            Terrain.Map.Map.ForEach((x) => x.Reverse());
            Terrain.Map.ToHeatmap(10, null).Save(Utilities.GetDefaultSaveFolder() + @"terrain\heatmap.png", System.Drawing.Imaging.ImageFormat.Png);
            try
            {
                Terrain.Map.ToBitmap().Save(Utilities.GetDefaultSaveFolder() + @"terrain\heightmap.png", System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception)
            {
                MessageBox.Show("Warning, couldn't save heightmap");
            }
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
            TerrainImage.Refresh();

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
            if (N8Map == null)
            {
                N8Map = new Bitmap(@"C:\Projects\N8 Parser\Map.png");
            }
            Bitmap temp = Terrain.MinimizeMap(N8Map);
            temp = Terrain.ProcessMap(temp);
            temp = Terrain.OutlineMap(temp);
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
                DrawBitmap(TerrainDrawing, Terrain.Map.ToHeatmap(10, GetOutline()));
                KeepCurrentSeed.Enabled = true;
                Materialize.Enabled = true;
            }

            
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

        private void loadN8MapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            opener.Filter = "PNG Images|*.png";
            if (opener.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Bitmap temp = new Bitmap(opener.FileName);
                map.SetBitmap(temp);
                if (map.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    BitmapOffset = map.Offset;
                    N8Map = temp;
                    this.KeepCurrentSeed.Checked = false;
                    this.KeepCurrentSeed.Enabled = false;
                }
            }
        }
    }
}
