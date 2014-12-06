using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Server.Maps.Voronoi;
using Color = System.Drawing.Color;

namespace Server.MapGenerationVisualizer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private IEnumerable<Vector2i> _zonesCoords;
        private List<GraphEdge> _voronoi;

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

        private List<GraphEdge> BuildVoronoiBurhan(IEnumerable<Vector2i> zonesCoords)
        {
            var voronoi = new Voronoi(0.1);

            //Prepare data
            var xValues = new double[zonesCoords.Count()];
            var yValues = new double[zonesCoords.Count()];

            for (int i = 0; i < xValues.Length; i++)
            {
                xValues[i] = zonesCoords.ElementAt(i).X;
                yValues[i] = zonesCoords.ElementAt(i).Z;
            }

            var gridSize = (int)udGrid.Value;
            var zoneMax = gridSize * 16;

            //Calc Voronoi
            var timer = Stopwatch.StartNew();
            var result = voronoi.generateVoronoi(xValues, yValues, 0, zoneMax, 0, zoneMax);
            var time = timer.ElapsedMilliseconds;
            timer.Stop();

            Debug.Print("Voronoi by Burhan calc time: {0} msec", time);

            return result;
        }

        private void DrawDiagram(IEnumerable<Vector2i> zonesCoord, List<GraphEdge> graph)
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

            foreach (var voronoiEdge in graph)
            {
                var zoomedEdge1 = new PointF((float)voronoiEdge.x1*ratio, (float)voronoiEdge.y1*ratio);
                var zoomedEdge2 = new PointF((float)voronoiEdge.x2 * ratio, (float)voronoiEdge.y2 * ratio);
                g.DrawLine(verticePen, zoomedEdge1, zoomedEdge2);
            }
        }

        private void Create_Click(object sender, EventArgs e)
        {
            _zonesCoords = GetZonesCoords();
            //_voronoi = BuildVoronoiBen(_zonesCoords);
            _voronoi = BuildVoronoiBurhan(_zonesCoords);
            DrawDiagram(_zonesCoords, _voronoi);
        }

        private void cbZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawDiagram(_zonesCoords, _voronoi);
        }
    }
}

