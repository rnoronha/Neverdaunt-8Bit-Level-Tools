using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N8Parser.Level_Modifiers
{
    class MazeGenerator
    {
        private class Node
        {
            public bool InMaze = false;
            public List<Edge> edges;

            public Node()
            {
                edges = new List<Edge>(8);
            }
        }

        private class Edge
        {
            public Node A;
            public Node B;

            public Edge(Node A, Node B)
            {
                this.A = A;
                this.B = B;
            }

        }
    }
}
