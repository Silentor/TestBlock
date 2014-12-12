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
        public Vector2[] Vertices;

        /// <summary>
        /// Clockwise oriented edges
        /// </summary>
        public Edge[] Edges;

        /// <summary>
        /// Clockwise oriented neighbor cells
        /// </summary>
        public Cell[] Neighbors;

        public BoundingBox Bounds;

        public bool IsClosed;

        /// <summary>
        /// Check if position contains in cell
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsContains(Vector2 position)
        {
            foreach (var edge in Edges)
            {
                if ((position.Y - edge.Vertex1.Y)*(edge.Vertex2.X - edge.Vertex1.X) -
                    (position.X - edge.Vertex1.X)*(edge.Vertex2.Y - edge.Vertex1.Y) > 0)
                    return false;
            }

            return true;
        }

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
