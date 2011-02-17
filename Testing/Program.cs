using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N8Parser.Level_Modifiers;
using N8Parser;
using System.Windows.Media.Media3D;
using N8Parser.Geometry;
using N8Parser.Tronics;
using System.Drawing;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {

            N8Level ShrineHut = MaxProtectTest.GetHutProxies();

            Utilities.MergeWithDefault(ShrineHut);

            Utilities.Save(Utilities.GetDefaultSaveFolder() + "shrine_hut_land.ncd", ShrineHut);
        }
        
    }
}
