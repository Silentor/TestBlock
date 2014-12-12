using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Server.Maps.Voronoi;
using Color = System.Drawing.Color;
using Point = Microsoft.Xna.Framework.Point;

namespace Server.MapGenerationVisualizer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Vector2[] _zonesCoords;
        private List<GraphEdge> _voronoi;
        private Cell[] _cells;
        private static readonly Brush[] _zonesColors =
        {
            new SolidBrush(Color.Red), 
            new SolidBrush(Color.Green), 
            new SolidBrush(Color.Blue), 
            new SolidBrush(Color.Orange), 
            new SolidBrush(Color.Violet), 
            new SolidBrush(Color.Yellow), 
        };

        private void DrawDiagram(Cell[] cells)
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

            //Draw border
            var borderPen = new Pen(Color.DarkGray, 1);
            g.DrawRectangle(borderPen, 0, 0, zoneMax * ratio, zoneMax * ratio);

            var pointPen = new Pen(Color.Black, 3);
            var verticePen = new Pen(Color.Black, 1);
            
            foreach (var cell in cells)
            {
                //Draw polygon
                var zoneColor = _zonesColors[cell.Id%_zonesColors.Length];
                for (var i = 0; i < cell.Edges.Count; i++)
                {
                    if (cell.IsClosed)
                    {
                        foreach (var edge in cell.Edges)
                        {
                            g.DrawLine(verticePen, edge.Vertex1.X * ratio, edge.Vertex1.Y * ratio, edge.Vertex2.X * ratio, edge.Vertex2.Y * ratio);
                        }
                    }

                    //var vertices = cell.Vertices.Select(p => new PointF(p.X*ratio, p.Y*ratio)).ToArray();
                    //g.FillPolygon(zoneColor, vertices);

                    ////var zoomedEdge1 = new PointF(cell.Vertices[i].X * ratio, cell.Vertices[i].Y * ratio);
                    ////var nextVerticeIndex = i == cell.Vertices.Count - 1 ? 0 : i + 1;
                    ////var zoomedEdge2 = new PointF(cell.Vertices[nextVerticeIndex].X * ratio, cell.Vertices[nextVerticeIndex].Y * ratio);
                    ////g.DrawLine(verticePen, zoomedEdge1, zoomedEdge2);

                    //g.DrawPolygon(verticePen, vertices);
                }

                //Draw zones centers
                g.DrawEllipse(pointPen, cell.Center.X * ratio, cell.Center.Y * ratio, 1, 1);
            }

        }

        private void Create_Click(object sender, EventArgs e)
        {
            _zonesCoords = CellMeshGenerator.GeneratePoints((int)udZones.Value, (int)udGrid.Value);
            _voronoi = CellMeshGenerator.GenerateVoronoi(_zonesCoords, (int)udGrid.Value);
            _cells = CellMeshGenerator.ProcessVoronoi(_zonesCoords, _voronoi);
            DrawDiagram(_cells);
        }

        private void cbZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawDiagram(_cells);
        }
    }
}

