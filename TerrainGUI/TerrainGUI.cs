using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using N8Parser.Terrain;
using N8Parser;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Media.Media3D;
using N8Parser.Level_Modifiers;

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
        static Bitmap CurrentHeatmap;

        public TerrainGUI()
        {
            InitializeComponent();
            this.Show();
            this.FormClosing += new FormClosingEventHandler((a, b) => { if (bw != null && bw.IsBusy) bw.CancelAsync(); });
            this.TerrainImage.MouseMove += new MouseEventHandler(TerrainImage_MouseMove);
            //What the fuck. When I did the one above ^ all of the event handler bindings everything else disappeared from the designer.
            this.HighOrder.Click += new EventHandler(HighOrder_Click);
            this.LowOrder.Click += new EventHandler(LowOrder_Click);
            this.loadHeightmapToolStripMenuItem.Click += new EventHandler(loadHeightmapToolStripMenuItem_Click);
            this.Materialize.Click += new EventHandler(Materialize_Click);
            this.Stop.Click += new EventHandler(Stop_Click);
            this.loadN8MapToolStripMenuItem.Click += new EventHandler(loadN8MapToolStripMenuItem_Click);
            this.saveHeightmapToolStripMenuItem.Click += new EventHandler(saveHeightmapToolStripMenuItem_Click);

            

            TerrainDrawing = TerrainImage.CreateGraphics();
            //Generate a new graphics if we're resized, because they don't get updated with the new size
            this.ResizeEnd += new EventHandler((a, b) => TerrainDrawing = TerrainImage.CreateGraphics() );
            //And refresh the bitmap, if it exists
            TerrainImage.Paint += new PaintEventHandler((a, b) => RefreshHeatmap(TerrainDrawing));
            
        }

        void TerrainImage_MouseMove(object sender, MouseEventArgs e)
        {
            int scale = (int)LandsPerCell.Value;
            
            int CurrentX = (e.X - 5) / scale;
            int CurrentY = (e.Y - 10) / scale;

            X_Current.Text = (CurrentX - BitmapOffset.X) + "";
            //N8's y axis is flipped, so the current Y is negative.
            Y_Current.Text = -(CurrentY - BitmapOffset.Y) + "";
        }

        private void DoTerrainTest(Graphics g, Bitmap Outline, int seed = 0)
        {

            Terrain.Map = new Heightmap(Outline.Width, Outline.Height);
            Terrain.rand = new Random(seed);
            Point center = new Point(Outline.Width / 2, Outline.Height / 2);

            Terrain.Map.Holds = Terrain.BitmapToHolds(Outline);

            Terrain.UniformNoise(-500, 500);
            bw.ReportProgress(1, Terrain.Map.ToHeatmap((int)LandsPerCell.Value, Outline));

            for (int i = 0; i < 20; i++)
            {
                Terrain.FluidSimulation(20);
                bw.ReportProgress(i, Terrain.Map.ToHeatmap((int)LandsPerCell.Value, Outline));
                if (bw.CancellationPending)
                    break;
            }
        }

        private void DoHighOrderTerrain(Graphics g, Bitmap Outline, int seed = 0)
        {
            Random r = new Random(seed);
            Terrain.rand = r;
            
            if (!KeepCurrentSeed.Checked || Terrain.Map == null)
            {
                Heightmap MapSeed;
                MapSeed = new Heightmap(Outline.Width, Outline.Height);
                //MapSeed.Holds = Terrain.BitmapToHolds(Outline);
                Terrain.Map = MapSeed;
            }
            
            SortedList<double, Action> actions = new SortedList<double, Action>();
            actions.Add(0.001, () => Terrain.Map.normalize(-5000, 5000));
            actions.Add(0.5, () => Terrain.Mountains(20, 2, 30, 50));
            actions.Add(0.49, () => Terrain.Valleys(20, 2, 30, 50));
            actions.Add(0.04, () => Terrain.ValleyRaiser(2, 50, true));
            actions.Add(0.05, () => Terrain.MountainSmasher(2, 50, true));
            actions.Add(0.20, () => Terrain.RiverFormation(0.009, 10));
            
            Heightmap res = Terrain.Arbitrary(actions, 100000, (a) =>
                {
                    if (a % 10 == 0)
                    {
                        bw.ReportProgress(a, Terrain.Map.ToHeatmap((int)LandsPerCell.Value, Outline));
                    }
                    return !bw.CancellationPending;
                });
            if (ShowBitmap.Checked)
            {
                res.ShowHeatmap((int)LandsPerCell.Value, Outline);
            }

            FinishMap();
        }

        private void DoLowOrderTerrain(Graphics g, Bitmap Outline, int seed = 0)
        {
            Random r = new Random(seed);
            Terrain.rand = r;

            if (!KeepCurrentSeed.Checked || Terrain.Map == null)
            {
                Heightmap MapSeed;
                MapSeed = new Heightmap(Outline.Width, Outline.Height);
                MapSeed.Holds = Terrain.BitmapToHolds(Outline);
                Terrain.Map = MapSeed;
                Terrain.Map.MaxValue = 5000;
                Terrain.Map.MinValue = -5000;
            }



            SortedList<double, Action> actions = new SortedList<double, Action>();
            actions.Add(0.75, () => Terrain.Hill(r.NextPoint(), 0.0009, 0.5, 5, 10, true));
            actions.Add(0.25, () => Terrain.Hill(r.NextPoint(), 0.0009, 0.5, 10, 25, true));
            actions.Add(0.07, () => Terrain.MountainSmasher(5, 25, true));
            actions.Add(0.04, () => Terrain.ValleyRaiser(5, 25, true));
            actions.Add(0.05, () => Terrain.RiverFormation(0.009, 10));
            actions.Add(0.01, () => Terrain.FluidSimulation(50));

            Heightmap res = Terrain.Arbitrary(actions, 100000, (a) =>
            {
                if (a % 100 == 0)
                {
                    bw.ReportProgress(a, Terrain.Map.ToHeatmap((int)LandsPerCell.Value, Outline));
                }
                return !bw.CancellationPending;
            });
            if (ShowBitmap.Checked)
            {
                res.ShowHeatmap((int)LandsPerCell.Value, Outline);
            }

            FinishMap();
        }

        private static void FinishMap()
        {
            //And smooth it out one last time
            Terrain.FluidSimulation(100);
            //Normalize it to good N8 values
            Terrain.Map.normalize(-500, 1920);
            //Give it some noise, to cut down on z-fighting
            Terrain.UniformNoise(-5, 5);
        }

        public void RefreshHeatmap(Graphics g)
        {
            if (CurrentHeatmap != null)
            {
                g.DrawImage(CurrentHeatmap, 5, 10);
            }
        }

        public void DrawBitmap(Graphics g, Bitmap bits)
        {
            try
            {
                CurrentHeatmap = bits;
                RefreshHeatmap(g);
            }
            catch (ExternalException)
            {
                //Well we're screwed, not much we can do besides return.
            }
        }

        private void Materialize_Click(object sender, EventArgs e)
        {
            Materialize.Enabled = false;
            //Flip the heightmap's Y coordinates, because N8 uses y+ to mean up-screen and 
            //everything else we've been doing so far uses Y+ to mean down-screen
            Terrain.Map.Map.ForEach((x) => x.Reverse());

            //Assume that the cell will be tiled with CellSquareSize x CellSquareSize blocks
            int MaxCellSquareSize = (int)LandsPerCell.Value;

            //And then the number of cells in the X and Y direction...
            int NumCellsX = Terrain.Map.sizeX / MaxCellSquareSize;
            int NumCellsY = Terrain.Map.sizeY / MaxCellSquareSize;

            //And then reduce this map to whatever size we really want
            //Generate every map from 0 to what we have, so we can pick them out later.
            //That didn't work out :( I need to generate like a polygon surface or something and then tile it, not interpolate the points...

            List<Tuple<Heightmap, int>> maps = new List<Tuple<Heightmap, int>>();
            Heightmap current = Terrain.Map;

            maps.Add(Tuple.Create(current, MaxCellSquareSize));


            /*
            for (int i = MaxCellSquareSize; i >= 5; i--)
            {
                int CellSquareSize = i;
                //Reduce the map so each cell is CellSquareSize x CellSquareSize
                int newXSize = NumCellsX * CellSquareSize;
                int newYSize = NumCellsY * CellSquareSize;

                Heightmap fullmap = current.reduceTo(newXSize, newYSize);
                List<List<Heightmap>> map = Terrain.ChopHeightmap(CellSquareSize, fullmap);
                fullmap.ToHeatmap(CellSquareSize, null).Save(Utilities.GetDefaultSaveFolder() + @"terrain\heatmap," + CellSquareSize + ".png", System.Drawing.Imaging.ImageFormat.Png);
                maps.Add(Tuple.Create(fullmap, CellSquareSize));
                current = fullmap;
            }
            */

            Random rand = new Random();

                    //Pick which map to use; ideally, cells that need more detail will use a more detailed map.
                    //Right now I'm just gonna generate them all and see how they vary by hand
            for (int CellSize = 0; CellSize < maps.Count; CellSize++)
            {

                Heightmap fullmap = maps[CellSize].Item1;
                int CellSquareSize = maps[CellSize].Item2;
                List<List<Heightmap>> CurrMap = Terrain.ChopHeightmap(CellSquareSize, fullmap);
                for (int k = 0; k < NumCellsX; k++)
                {
                    for (int l = 0; l < NumCellsY; l++)
                    {
                        #region land placing setup
                        //Given that square size, figure out how to (flat) tile a cell.
                        //Each block will be cell size / square size in (effective) width and breadth.
                        int BlockSize = 4000 / CellSquareSize;

                        //If we have an even square size, we'll want to offset the block's position.
                        int Offset = 0;
                        if (CellSquareSize % 2 == 0)
                        {
                            Offset = BlockSize / 2;
                        }

                        //And we'll also need to calculate the offset to the edges of blocks, for rotation generation
                        //But I'm not sure how to do that nicely, so I'll skip it for now.
                        int BlockEdgeOffset = 0;
                        if (CellSquareSize % 2 == 1)
                        {
                            BlockEdgeOffset = BlockSize / 2;
                        }
                        //Just set it to 0 in order to skip 
                        BlockEdgeOffset = 0;

                        //And calculate the iterator offset, so we have everything in the middle
                        int IterOffset = (CellSquareSize - 1) / 2;

                        N8Level Level = new N8Level();
                        List<N8Block> lands = new List<N8Block>(CellSquareSize * CellSquareSize);
                        Vector3D LowestPoint = new Vector3D(0, 0, int.MaxValue);
                        string LandType = "land";
                        if (CellSquareSize < 10)
                        {
                            LandType = "landmega";
                        }
                        #endregion

                        #region placing lands
                        for (int i = 0; i < CellSquareSize; i++)
                        {
                            for (int j = 0; j < CellSquareSize; j++)
                            {

                                N8Block land = Level.blocks.GenerateBlock(LandType, "Terrain");
                                land.position.X = ((i - IterOffset) * BlockSize) - Offset;
                                land.position.Y = ((j - IterOffset) * BlockSize) - Offset;
                                land.position.Z = CurrMap[k][l][i, j];

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
                                Vector3D North = new Vector3D(land.position.X, (((j + 1) - IterOffset) * BlockSize + BlockEdgeOffset), fullmap[k * CellSquareSize + i, l * CellSquareSize + (j + 1)]);
                                Vector3D South = new Vector3D(land.position.X, (((j - 1) - IterOffset) * BlockSize + BlockEdgeOffset), fullmap[k * CellSquareSize + i, l * CellSquareSize + (j - 1)]);
                                Vector3D East = new Vector3D((((i + 1) - IterOffset) * BlockSize + BlockEdgeOffset), land.position.Y, fullmap[k * CellSquareSize + (i + 1), l * CellSquareSize + j]);
                                Vector3D West = new Vector3D((((i - 1) - IterOffset) * BlockSize + BlockEdgeOffset), land.position.Y, fullmap[k * CellSquareSize + (i - 1), l * CellSquareSize + j]); ;
                                Vector3D LandPos = land.position;


                                Quaternion NSRot = new Quaternion(0, 0, 0, 1);
                                Quaternion EWRot = new Quaternion(0, 0, 0, 1);

                                //If I'm between two things that are both higher or both lower than me, I shouldn't have a tilt in that direction.
                                //E.G: if I'm like this : _-_ or -_-, I shouldn't have a tilt.
                                if ((North.Z < LandPos.Z && LandPos.Z < South.Z) || (North.Z > LandPos.Z && LandPos.Z > South.Z))
                                {
                                    if ((North - South).Length > 400)
                                    {
                                        //                       land.type = "landmega";
                                    }
                                    Vector3D higher = North;
                                    Vector3D lower = South;
                                    int dirMul = -1;
                                    if (South.Z > North.Z)
                                    {
                                        higher = South;
                                        lower = North;
                                        dirMul = 1;
                                    }
                                    Vector3D A = lower - new Vector3D(higher.X, higher.Y, lower.Z);
                                    Vector3D B = lower - new Vector3D(higher.X, higher.Y, higher.Z);
                                    A.Normalize();
                                    B.Normalize();

                                    NSRot = new Quaternion(new Vector3D(1, 0, 0), dirMul * Math.Acos(A.Z * B.Z + A.Y * B.Y) * Utilities.RadToDeg);

                                }

                                if ((East.Z < LandPos.Z && LandPos.Z < West.Z) || (West.Z < LandPos.Z && LandPos.Z < East.Z))
                                {
                                    if ((East - West).Length > 400)
                                    {
                                        //                     land.type = "landmega";
                                    }
                                    Vector3D higher = West;
                                    Vector3D lower = East;
                                    int dirMul = -1;
                                    if (East.Z > West.Z)
                                    {
                                        higher = East;
                                        lower = West;
                                        dirMul = 1;
                                    }
                                    Vector3D A = lower - new Vector3D(higher.X, higher.Y, lower.Z);
                                    Vector3D B = lower - new Vector3D(higher.X, higher.Y, higher.Z);
                                    A.Normalize();
                                    B.Normalize();

                                    EWRot = new Quaternion(new Vector3D(0, 1, 0), dirMul * Math.Acos(A.Z * B.Z + A.X * B.X) * Utilities.RadToDeg);
                                }

                                land.rotation = EWRot * NSRot;

                                lands.Add(land);
                            }
                        }
#endregion
                        
                        //Utilities.Save(Utilities.GetDefaultSaveFolder() + @"terrain\plain\" + (k - BitmapOffset.X) + "," + (l - BitmapOffset.Y) + ".ncd", Level);
                        Utilities.Save(Utilities.GetDefaultSaveFolder() + @"terrain\testing\" + (k - BitmapOffset.X) + "," + (l - BitmapOffset.Y) + ".ncd", Level);
                        #region Extra nice things
                        /*
                        //Now add some nice things like a stack and random trees

                        for (int b = -1000; b < LowestPoint.Z; b += 70)
                        {
                            N8Block stack = Level.blocks.GenerateBlock("floor", "Stack");
                            stack.position = LowestPoint;
                            stack.position.Z = b;
                        }

                        Utilities.Save(Utilities.GetDefaultSaveFolder() + @"terrain\stack\" + (k - BitmapOffset.X) + "," + (l - BitmapOffset.Y) + ".ncd", Level);

                        for (int b = 0; b < 75; b++)
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
                        
                        Utilities.Save(Utilities.GetDefaultSaveFolder() + @"terrain\tree\" + (k - BitmapOffset.X) + "," + (l - BitmapOffset.Y) + ".ncd", Level);
                        
                        // */
                        #endregion

                    }
                }
            }
            //Unflip it, so we're back where we started
            Terrain.Map.Map.ForEach((x) => x.Reverse());
            Terrain.Map.ToHeatmap(MaxCellSquareSize, null).Save(Utilities.GetDefaultSaveFolder() + @"terrain\heatmap.png", System.Drawing.Imaging.ImageFormat.Png);
            try
            {
                Terrain.Map.ToBitmap().Save(Utilities.GetDefaultSaveFolder() + @"terrain\heightmap.png", System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception)
            {
                MessageBox.Show("Warning, couldn't save heightmap");
            }
            MessageBox.Show("Done!");
            Materialize.Enabled = true;
        }


        private void LowOrder_Click(object sender, EventArgs e)
        {
            Bitmap outline = SetupRun();
            bw.DoWork += new DoWorkEventHandler((a, b) => DoLowOrderTerrain(TerrainDrawing, outline, (int)SeedEntry.Value));
            //bw.DoWork += new DoWorkEventHandler((a, b) => DoTerrainTest(TerrainDrawing, outline, (int)SeedEntry.Value));
            bw.RunWorkerAsync();
        }

        private void HighOrder_Click(object sender, EventArgs e)
        {
            Bitmap outline = SetupRun();
            bw.DoWork += new DoWorkEventHandler((a, b) => DoHighOrderTerrain(TerrainDrawing, outline, (int)SeedEntry.Value));
            bw.RunWorkerAsync();
        }

        private Bitmap SetupRun()
        {
            HighOrder.Enabled = false;
            LowOrder.Enabled = false;
            Materialize.Enabled = false;
            Stop.Enabled = true;
            SeedEntry.Enabled = false;
            LandsPerCell.Enabled = false;
            FileMenu.Enabled = false;
            

            Bitmap outline = GetOutline();
            TerrainImage.Refresh();

            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.ProgressChanged += new ProgressChangedEventHandler((a, b) => { DrawBitmap(TerrainDrawing, (Bitmap)b.UserState); IterCounter.Text = b.ProgressPercentage + ""; });
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            return outline;
        }

        private Bitmap GetOutline()
        {
            if (N8Map == null)
            {
                N8Map = new Bitmap(@"C:\Projects\N8 Parser\Map.png");
            }
            Bitmap temp = Terrain.MinimizeMap(N8Map);
            temp = Terrain.ProcessMap(temp);
            temp = Terrain.OutlineMap(temp);
            temp = Terrain.TrimBounds(temp);
            temp = Terrain.ExpandMap(temp, (int)LandsPerCell.Value);
            return temp;
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            saveHeightmapToolStripMenuItem.Enabled = true;
            Materialize.Enabled = true; 
            HighOrder.Enabled = true;
            LowOrder.Enabled = true;
            Stop.Enabled = false;
            SeedEntry.Enabled = true;
            LandsPerCell.Enabled = true;
            KeepCurrentSeed.Enabled = true;
            FileMenu.Enabled = true;

            DrawBitmap(TerrainDrawing, Terrain.Map.ToHeatmap((int)LandsPerCell.Value, GetOutline()));
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
                DrawBitmap(TerrainDrawing, Terrain.Map.ToHeatmap((int)LandsPerCell.Value, GetOutline()));
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
