using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace N8Parser.Level_Modifiers
{
    public class Forest
    {
        public static string[] TreeTypes = new string[] { "tree", "ghost.tree", "dark.tree", "tree2fall", "tree2winter", "tree3", "tree3pink", "tree2", "bush", "bush2", "mushroom", "flowers" };
        public static string[] TreeNames = new string[] { "FYI I am a tree", "Tree", "Treebeard", "Tree for all", "Slim Shady", "FYI I am a spy", 
                                                          "Hibernating Treenosaur", "Menage a tree", "Treewise the Brave", "The Forgotten Tree", 
                                                          "The real Slim Shady", "OMGWTTREE", "A LOUD TREE", "Creaky McCreakerson", "The fake Slim Shady",
                                                          "Petrified Treesaurus Rex", "Tree with a mission", "Tree with a gun", "Running tree",
                                                          "I move when you're not looking", "Don't blink!", "BRB asleep", "Wolf in tree's clothing", 
                                                          "THIS IS TREEEEEEEEEEEE", "THIS IS SPARTAAAAAAAAA", "THIS IS STUPIIIIIIIIID", "Tree for the asking",
                                                          "Tree! Please take one!", "Treeway", "Snooze button", "Not a tree", "Definitely not a tree", 
                                                          "Not a tree stop asking", "Still not a tree", "Tree party", "Greenbeard the Wise", "Barky", 
                                                          "Snarky tree", "Snake tree", "Treeangle", "OH MY GOD WE'RE ALL GONNA DIE", "THE SKY IS FALLING!",
                                                          "Oaky", "Okay wait I mean Oaky", "Anagram", "Professor Oak", "Treeagonal", "I'll have tree please",
                                                          "Treeway or the highway!", "Treesexual", "I give up", "Treepenny", "I'm a tree when you're not looking"};
        public static N8Level GetRandomForest()
        {
            N8Level forest = Utilities.GetDefault();

            
            Random rand = new Random();

            for (int i = 0; i < 300; i++)
            {
                N8Block b = forest.blocks.GenerateBlock(TreeTypes[i % TreeTypes.Length], TreeNames[i % TreeNames.Length]);
                b.position = rand.NextVector(new Vector3D(-2000, -2000, 75), new Vector3D(2000, 2000, 65));
                b.rotation = new Quaternion(new Vector3D(0, 0, 1), rand.Next(360));
            }

            MinorModifiers.OrderLoading(forest, new Vector3D(1, 0, 0));

            return forest;
        }

        public static void GenerateRandomForest()
        {
            Utilities.Save(Utilities.GetDefaultSaveFolder() + "rforest", GetRandomForest());
        }
    }
}
