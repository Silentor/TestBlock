using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Server.Maps.Voronoi;
using Silentor.TB.Server.Tools;
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

        private int _seed;

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
            var borderBrush = new SolidBrush(Color.DarkGray);
            g.DrawRectangle(borderPen, 0, 0, zoneMax * ratio, zoneMax * ratio);

            var pointPen = new Pen(Color.Black, 3);
            var pointBrush = new SolidBrush(Color.Black);
            var pointFont = new Font("Arial", 8);

            var verticePen = new Pen(Color.Black, 1);

            foreach (var closedCell in cells)
            {
                FillCell(closedCell, g, _zonesColors[closedCell.Id % _zonesColors.Length], ratio);
            }

            //Draw open cells
            foreach (var openCell in cells.Where(c => !c.IsClosed))
            {
                foreach (var edge in openCell.Edges)
                    g.DrawLine(borderPen, new PointF(edge.Vertex1.X * ratio, edge.Vertex1.Y * ratio),
                        new PointF(edge.Vertex2.X * ratio, edge.Vertex2.Y * ratio));

                //Draw zones centers
                g.DrawEllipse(borderPen, openCell.Center.X * ratio, openCell.Center.Y * ratio, 1, 1);
                g.DrawString(openCell.Id.ToString(), pointFont, borderBrush, openCell.Center.X * ratio, openCell.Center.Y * ratio);
            }

            //Draw closed cells
            foreach (var cell in cells.Where(c => c.IsClosed))
            {
                //Draw polygon
                //var zoneColor = new  _zonesColors[cell.Id%_zonesColors.Length];
                for (var i = 0; i < cell.Edges.Length; i++)
                {
                    g.DrawPolygon(verticePen, cell.Vertices.Select(v => new PointF(v.X, v.Y)).ToArray());
                }

                //Draw zones centers
                g.DrawEllipse(pointPen, cell.Center.X * ratio, cell.Center.Y * ratio, 1, 1);
                g.DrawString(cell.Id.ToString(), pointFont, pointBrush, cell.Center.X * ratio, cell.Center.Y * ratio);
            }
        }

        private void FillCell(Cell cell, Graphics g, Brush b, float ratio)
        {
            if (!cell.IsClosed)
                return;

            //Get list of chunks contained in zone. todo consider flood fill from cell center
            var chunks = new List<Vector2i>();

            //Build chunk bounding box for cell
            Vector2i minChunkPos = new Vector2i(int.MaxValue, int.MaxValue), maxChunkPos = new Vector2i(int.MinValue, int.MinValue);
            foreach (var vert in cell.Vertices)
            {
                var chunkPos = new Vector2i((int)vert.X / 16, ((int) (vert.Y / 16)));
                if (chunkPos.X < minChunkPos.X)
                    minChunkPos = new Vector2i(chunkPos.X, minChunkPos.Z);
                if (chunkPos.Z < minChunkPos.Z)
                    minChunkPos = new Vector2i(minChunkPos.X, chunkPos.Z);
                if (chunkPos.X > maxChunkPos.X)
                    maxChunkPos = new Vector2i(chunkPos.X, maxChunkPos.Z);
                if (chunkPos.Z > maxChunkPos.Z)
                    maxChunkPos = new Vector2i(maxChunkPos.X, chunkPos.Z);
            }

            //Draw bounding box
            //g.DrawRectangle(new Pen(b), minChunkPos.X * 16, minChunkPos.Z * 16, (maxChunkPos.X - minChunkPos.X + 1) * 16, (maxChunkPos.Z - minChunkPos.Z + 1) * 16);

            //Get chunks which centers inside cell
            var chunkBound = new Bounds2i(new Vector2i(minChunkPos.X, minChunkPos.Z), new Vector2i(maxChunkPos.X, maxChunkPos.Z));
            foreach (var chunkPos in chunkBound)
            {
                //Check center of each chunk against cell borders
                var chunkCenterPos = new Vector2(chunkPos.X*16 + 7.5f, chunkPos.Z*16 + 7.5f);
                if(cell.IsContains(chunkCenterPos))
                    chunks.Add(chunkPos);
            }

            //Test cell chunks
            foreach (var cellChunks in chunks)
                g.FillRectangle(b, cellChunks.X * 16, cellChunks.Z * 16, 16, 16);
        }

        private void Create_Click(object sender, EventArgs e)
        {
            if (cbIsRandomSeed.Checked)
            {
                _seed = new Random().Next();
                tbSeed.Text = _seed.ToString();
            }

            var minDistance = (int)udMinDistance.Value;
            _zonesCoords = CellMeshGenerator.GeneratePoints((int)udZones.Value, (int)udGrid.Value, _seed, minDistance);
            _voronoi = CellMeshGenerator.GenerateVoronoi(_zonesCoords, (int)udGrid.Value);
            _cells = CellMeshGenerator.ProcessVoronoi(_zonesCoords, _voronoi);

            DrawDiagram(_cells);
        }

        private void cbZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawDiagram(_cells);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int result;
            if (int.TryParse(tbSeed.Text, out result))
                _seed = result;
            else
                tbSeed.Text = _seed.ToString();
        }
    }
}

