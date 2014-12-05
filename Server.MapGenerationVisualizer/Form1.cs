using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BenTools.Mathematics;
using Microsoft.Xna.Framework;
using Silentor.TB.Common.Maps.Geometry;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;

namespace Server.MapGenerationVisualizer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private IEnumerable<Vector2i> _zonesCoords;
        private VoronoiGraph _voronoi;

        private IEnumerable<Vector2i> GetZonesCoords()
        {
            //Prepare input data
            var rnd = new Random();
            var hangChecker = 0;
            var gridSize = (int) udGrid.Value;
            var zoneMax = gridSize * 16;
            var chunksGrid = new bool[gridSize, gridSize];

            //Generate zones center coords, check that only one zone occupies one chunk
            var zonesCount = (int) udZones.Value;
            var zonesCoords = new List<Vector2i>(zonesCount);
            for (int i = 0; i < zonesCount; i++)
            {
                var zoneCoord = new Vector2i(rnd.Next(0, zoneMax), rnd.Next(0, zoneMax));
                if (!chunksGrid[zoneCoord.X / 16, zoneCoord.Z / 16])
                {
                    chunksGrid[zoneCoord.X / 16, zoneCoord.Z / 16] = true;
                    zonesCoords.Add(zoneCoord);
                }
                else
                {
                    if (hangChecker++ < 100)
                        i--;
                    else
                        break;
                }
            }

            return zonesCoords;
        }

        private VoronoiGraph GetDiagram(IEnumerable<Vector2i> zonesCoords)
        {
            //Prepare input
            var input = new Vector[zonesCoords.Count()];
            var j = 0;
            foreach (var zoneCoord in zonesCoords)
            {
                input[j++] = new Vector(zoneCoord.X, zoneCoord.Z);
            }

            //Calc Voronoi
            var timer = Stopwatch.StartNew();
            var result = Fortune.ComputeVoronoiGraph(input);
            var time = timer.ElapsedMilliseconds;
            timer.Stop();

            Debug.Print("Voronoi calc time: {0} msec", time);

            return result;
        }

        private void DrawDiagram(IEnumerable<Vector2i> zonesCoord, VoronoiGraph graph)
        {
            var gridSize = (int)udGrid.Value;
            var zoneMax = gridSize * 16;
            var ratio = Convert.ToSingle(cbZoom.Text) / 100f;

            //Draw data
            var g = pictureBox1.CreateGraphics();
            g.Clear(Color.White);

            //Draw grid
            if (ratio >= 0.5)
            {
                var gridPen = new Pen(Color.LightGray, 1);
                for (int x = 0; x < zoneMax; x += 16)
                {
                    g.DrawLine(gridPen, new PointF(x * ratio, 0), new PointF(x * ratio, zoneMax * ratio));
                    g.DrawLine(gridPen, new PointF(0, x * ratio), new PointF(zoneMax * ratio, x * ratio));
                }
            }

            var pointPen = new Pen(Color.Blue, 3);
            var borderPen = new Pen(Color.Black, 1);

            //Draw zones centers
            foreach (var point in zonesCoord)
                g.DrawEllipse(pointPen, point.X * ratio, point.Z * ratio, 1, 1);

            //Draw border
            g.DrawRectangle(borderPen, 0, 0, zoneMax * ratio, zoneMax * ratio);

            var verticePen = new Pen(Color.Black, 1);
            var infiniteEdgePen = new Pen(Color.Red, 1);
            //foreach (var vertiz in result.Vertizes)
            //{
            //    g.DrawEllipse(verticePen, (float)vertiz[0], (float)vertiz[1], 1, 1);
            //}

            foreach (var voronoiEdge in graph.Edges)
            {
                if (voronoiEdge.IsInfinite) continue;

                if (!voronoiEdge.IsPartlyInfinite)
                {
                    var zoomedEdge1 = voronoiEdge.VVertexA * ratio;
                    var zoomedEdge2 = voronoiEdge.VVertexB * ratio;
                    g.DrawLine(verticePen, (float)zoomedEdge1[0], (float)zoomedEdge1[1], (float)zoomedEdge2[0], (float)zoomedEdge2[1]);
                }
                else
                {
                    var calculatedPoint = (voronoiEdge.FixedPoint + voronoiEdge.DirectionVector * 100) * ratio;
                    var fixedPoint = voronoiEdge.FixedPoint * ratio;
                    g.DrawLine(infiniteEdgePen, (float)fixedPoint[0], (float)fixedPoint[1],
                        (float)calculatedPoint[0], (float)calculatedPoint[1]);
                }
            }
        }

        private void Create_Click(object sender, EventArgs e)
        {
            _zonesCoords = GetZonesCoords();
            _voronoi = GetDiagram(_zonesCoords);
            DrawDiagram(_zonesCoords, _voronoi);
        }

        private void cbZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawDiagram(_zonesCoords, _voronoi);
        }
    }
}
