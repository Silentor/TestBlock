using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Silentor.TB.Server.Maps.Voronoi
{
    public class Cell
    {
        /// <summary>
        /// Unique id of cell in mesh
        /// </summary>
        public int Id;

        /// <summary>
        /// Position of cell center
        /// </summary>
        public Vector2 Center;

        /// <summary>
        /// Clockwise sorted vertices
        /// </summary>
        public List<Vector2> Vertices;

        /// <summary>
        /// Clockwise oriented edges
        /// </summary>
        public List<Edge> Edges;

        /// <summary>
        /// Clockwise oriented neighbor cells
        /// </summary>
        public Cell[] Neighbors;

        public bool IsClosed;

        public struct Edge
        {
            public readonly Vector2 Vertex1;
            public readonly Vector2 Vertex2;

            public Edge(Vector2 vertex1, Vector2 vertex2)
            {
                Vertex1 = vertex1;
                Vertex2 = vertex2;
            }

            public Edge Reverse()
            {
                return new Edge(Vertex2, Vertex1);
            }
        }
    }
}
