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

            _zonesBrush = _zones.Select(c => new SolidBrush(c)).ToArray();
        }

        private Vector2[] _zonesCoords;
        private static List<GraphEdge> _voronoi;
        private static Cell[] _cells;

        private static readonly Color[] _zones =
        {
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Orange,
            Color.Violet,
            Color.Yellow,
        };

        private static Brush[] _zonesBrush;

        private int _seed;

        private void DrawDiagram(Cell[] cells)
        {
            if (_cells == null) return;

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

            foreach (var closedCell in cells.Where(c => c.IsClosed)/*.Take(1)*/)
            {
                var style = (MapFillStyle)Enum.Parse(typeof (MapFillStyle), cbStyle.Text);
                switch (style)
                {
                    case MapFillStyle.Simple:
                    FillCellSimple(closedCell, g, _zonesBrush[closedCell.Id % _zonesBrush.Length], ratio);
                        break;

                    case MapFillStyle.ChunkLerpFill:
                        FillCellSimpleByChunkLerp(closedCell, g, _zonesBrush[closedCell.Id % _zonesBrush.Length], ratio);
                        break;

                    case MapFillStyle.Interpolation:
                        FillCellInterpolation(closedCell, g, ratio, 1);
                        break;

                    case MapFillStyle.Interpolation2:
                        FillCellInterpolation(closedCell, g, ratio, 2);
                        break;

                    case MapFillStyle.Interpolation3:
                        FillCellInterpolation(closedCell, g, ratio, 3);
                        break;

                }
            }

            //Draw open cells
            foreach (var openCell in cells.Where(c => !c.IsClosed))
            {
                foreach (var edge in openCell.Edges)
                    g.DrawLine(borderPen, new PointF(edge.Vertex1.X * ratio, edge.Vertex1.Y * ratio),
                        new PointF(edge.Vertex2.X * ratio, edge.Vertex2.Y * ratio));

                //Draw zones centers
                if (ratio >= 0.5)
                {
                    g.DrawEllipse(borderPen, openCell.Center.X*ratio, openCell.Center.Y*ratio, 1, 1);
                    g.DrawString(openCell.Id.ToString(), pointFont, borderBrush, openCell.Center.X*ratio,
                        openCell.Center.Y*ratio);
                }
            }

            //Draw closed cells
            foreach (var cell in cells.Where(c => c.IsClosed))
            {
                //Draw polygon
                //var zoneColor = new  _zonesColors[cell.Id%_zonesColors.Length];
                for (var i = 0; i < cell.Edges.Length; i++)
                    g.DrawPolygon(verticePen, cell.Vertices.Select(v => new PointF(v.X * ratio, v.Y*ratio)).ToArray());

                //Draw zones centers
                if (ratio >= 0.5)
                {
                    g.DrawEllipse(pointPen, cell.Center.X*ratio, cell.Center.Y*ratio, 1, 1);
                    g.DrawString(cell.Id.ToString(), pointFont, pointBrush, cell.Center.X*ratio, cell.Center.Y*ratio);
                }
            }
        }

        private static Vector2i[] GetCellChunks(Cell cell)
        {
            if (!cell.IsClosed)
                return new Vector2i[0];

            //Get list of chunks contained in zone. todo consider flood fill from cell center
            var chunks = new List<Vector2i>();

            //Build chunk bounding box for cell
            Vector2i minChunkPos = new Vector2i(int.MaxValue, int.MaxValue), maxChunkPos = new Vector2i(int.MinValue, int.MinValue);
            foreach (var vert in cell.Vertices)
            {
                var chunkPos = new Vector2i((int)vert.X / 16, ((int)(vert.Y / 16)));
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
                var chunkCenterPos = new Vector2(chunkPos.X * 16 + 8, chunkPos.Z * 16 + 8);
                if (cell.IsContains(chunkCenterPos))
                    chunks.Add(chunkPos);
            }

            return chunks.ToArray();
        }

        private void FillCellSimple(Cell cell, Graphics g, Brush b, float ratio)
        {
            var chunks = GetCellChunks(cell);
            if (!chunks.Any())
                return;

            //Simple fill cell chunks
            foreach (var cellChunks in chunks)
                g.FillRectangle(b, cellChunks.X * 16 * ratio, cellChunks.Z * 16 * ratio, 16 * ratio, 16 * ratio);
        }

        private void FillCellSimpleByChunkLerp(Cell cell, Graphics g, Brush b, float ratio)
        {
            var chunks = GetCellChunks(cell);
            if (!chunks.Any())
                return;

            var nearCells = new[] {cell}.Concat(cell.Neighbors).ToArray();
            var nearCellsCoords = nearCells.Select(c => c.Center).ToArray();
            var nearCellsColor = nearCells.Select(c => _zones[c.Id % _zones.Length]).ToArray();
            var chunksColor = new Color[chunks.Length];                                         //Result

            //Lerp influence between current cell and all neighbor cells
            for (int i = 0; i < chunks.Length; i++)
            {
                var chunkCenterPosition = new Vector2(chunks[i].X*16 + 8, chunks[i].Z*16 + 8);

                var chunkCellsDistance = nearCellsCoords.Select(c => Vector2.Distance(chunkCenterPosition, c)).ToArray();
                //Get normalized influence
                var distanceSum = chunkCellsDistance.Sum();
                var chunkCellsInfluence = chunkCellsDistance.Select(c => distanceSum - c).ToArray();
                chunkCellsInfluence[0] *= 2;                //Owner chunk gets more influence
                var influenceSum = chunkCellsInfluence.Sum();

                //Influence of all near cells (including owner cell) to chunk
                var influence = chunkCellsInfluence.Select(inf => inf / influenceSum).ToArray();

                //Calculate color
                double rComp = 0, gComp = 0, bComp = 0;
                for (var j = 0; j < nearCells.Length; j++)
                {
                    rComp += influence[j] * nearCellsColor[j].R;
                    gComp += influence[j] * nearCellsColor[j].G;
                    bComp += influence[j] * nearCellsColor[j].B;
                }

                chunksColor[i] = Color.FromArgb(
                    MathHelper.Clamp((int)rComp, 0, 255),
                    MathHelper.Clamp((int)gComp, 0, 255),
                    MathHelper.Clamp((int)bComp, 0, 255));
            }

            //Simple fill cell chunks
            for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
            {
                var cellChunks = chunks[chunkIndex];
                var currentChunkBrush = new SolidBrush(chunksColor[chunkIndex]);
                g.FillRectangle(currentChunkBrush, cellChunks.X * 16 * ratio, cellChunks.Z * 16 *ratio, 16 *ratio, 16*ratio);
            }
        }

        private static void FillCellInterpolation(Cell cell, Graphics g, float ratio, int radius)
        {
            var chunks = GetCellChunks(cell);
            if (!chunks.Any())
                return;

            var result = new Color[chunks.Length];
            for (int i = 0; i < chunks.Length; i++)
            {
                var chunk = chunks[i];
                var neighborChunksColor = NeighborsOf(chunk, radius).Select(pos => CellIdBy(pos, cell))
                    .Concat(new []{cell})
                    .Select(c => c.IsClosed ? _zones[c.Id % _zones.Length] : Color.Black).ToArray();

                var rComp = neighborChunksColor.Select(c => (double) c.R).Average();
                var gComp = neighborChunksColor.Select(c => (double) c.G).Average();
                var bComp = neighborChunksColor.Select(c => (double) c.B).Average();

                result[i] = Color.FromArgb(
                    MathHelper.Clamp((int) rComp, 0, 255),
                    MathHelper.Clamp((int) gComp, 0, 255),
                    MathHelper.Clamp((int) bComp, 0, 255));
            }

            //Simple fill cell chunks
            for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
            {
                var cellChunks = chunks[chunkIndex];
                var currentChunkBrush = new SolidBrush(result[chunkIndex]);
                g.FillRectangle(currentChunkBrush, cellChunks.X * 16 * ratio, cellChunks.Z * 16 * ratio, 16*ratio, 16*ratio);
            }
        }

        private static Cell CellIdBy(Vector2i chunkPos, Cell nearCell)
        {
            var chunkCenterPosition = new Vector2(chunkPos.X*16 + 8, chunkPos.Z*16 + 8);

            if (nearCell.IsContains(chunkCenterPosition)) return nearCell;
            foreach (var neighbor in nearCell.Neighbors)
                if (neighbor.IsContains(chunkCenterPosition))
                    return neighbor;

            foreach (var cell in _cells)
                if (cell.IsContains(chunkCenterPosition))
                    return cell;

            throw new Exception("Can not execute here");
        }

        private static IEnumerable<Vector2i> NeighborsOf(Vector2i position, int radius)
        {
            yield return position;

            if (radius == 0) yield break;

            yield return new Vector2i(position.X, position.Z - 1);
            yield return new Vector2i(position.X, position.Z + 1);
            yield return new Vector2i(position.X - 1, position.Z);
            yield return new Vector2i(position.X + 1, position.Z);

            if (radius == 1) yield break;

            yield return new Vector2i(position.X - 1, position.Z - 1);
            yield return new Vector2i(position.X + 1, position.Z - 1);

            yield return new Vector2i(position.X - 1, position.Z + 1);
            yield return new Vector2i(position.X + 1, position.Z + 1);

            if (radius == 2) yield break;

            yield return new Vector2i(position.X, position.Z - 2);
            yield return new Vector2i(position.X, position.Z + 2);
            yield return new Vector2i(position.X + 2, position.Z);
            yield return new Vector2i(position.X - 2, position.Z);
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int result;
            if (int.TryParse(tbSeed.Text, out result))
                _seed = result;
            else
                tbSeed.Text = _seed.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (var styleName in Enum.GetNames(typeof (MapFillStyle)).Select(s => s.ToString()))
                cbStyle.Items.Add(styleName);

            cbStyle.SelectedIndex = 0;
        }

        private void cbStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(_cells != null)
                DrawDiagram(_cells);
        }

        private enum MapFillStyle
        {
            Simple,
            ChunkLerpFill,
            Interpolation,
            Interpolation2,
            Interpolation3
        }
    }
}

