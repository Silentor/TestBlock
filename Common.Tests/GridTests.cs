using System;
using FluentAssertions;
using NUnit.Framework;
using Silentor.TB.Common.Maps.Geometry;

namespace Silentor.TB.Common.Tests
{
    [TestFixture]
    public class GridTests
    {
        [Test]
        public void TestCreate()
        {
            var grid = new Grid<Object>(new Bounds2i(new Vector2i(7, 7), 3));
            grid.MinX.Should().Be(4);
            grid.MinZ.Should().Be(4);
            grid.MaxX.Should().Be(10);
            grid.MaxZ.Should().Be(10);
            grid.SizeX.Should().Be(7);
            grid.SizeZ.Should().Be(7);
            grid.Bounds.As<Object>().Should().Be(new Bounds2i(new Vector2i(4, 4), new Vector2i(10, 10)));

            grid = new Grid<Object>(new Bounds2i(new Vector2i(-10, -10), new Vector2i(-4, -4)));
            grid.MinX.Should().Be(-10);
            grid.MinZ.Should().Be(-10);
            grid.MaxX.Should().Be(-4);
            grid.MaxZ.Should().Be(-4);
            grid.SizeX.Should().Be(7);
            grid.SizeZ.Should().Be(7);
            grid.Bounds.As<Object>().Should().Be(new Bounds2i(new Vector2i(-7, -7), 3));

            grid = new Grid<Object>(new Bounds2i(new Vector2i(5, 5), 3, 4));
            grid.MinX.Should().Be(5);
            grid.MinZ.Should().Be(5);
            grid.MaxX.Should().Be(7);
            grid.MaxZ.Should().Be(8);
            grid.SizeX.Should().Be(3);
            grid.SizeZ.Should().Be(4);
            grid.Bounds.As<Object>().Should().Be(new Bounds2i(new Vector2i(5, 5), new Vector2i(7, 8)));
        }


        [Test]
        public void TestPositiveIndexSlide()
        {
            var grid = new Grid<int>(new Bounds2i(new Vector2i(10, 10), 5));
            foreach (var i in grid.Bounds)
                grid[i] = i.GetHashCode();

            var oldBound = grid.Bounds;
            grid.Slide(new Vector2i(1, 2));
            var newBound = grid.Bounds;

            foreach (var xz in oldBound.Intersect(newBound))
                grid.Get(xz).Should().Be(xz.GetHashCode(), "element at {0}", xz);

            foreach (var xz in newBound.Substract(oldBound))
                grid.Get(xz).Should().Be(0, "element at {0}", xz);
        }

        [Test]
        public void TestNegativeIndex()
        {
            var grid = new Grid<Tuple<int, int>>(new Bounds2i(new Vector2i(-1, -1), 5));
            foreach (var i in grid.Bounds)
                grid[i] = new Tuple<int, int>(i.X, i.Z);

            var oldBound = grid.Bounds;
            grid.Slide(new Vector2i(-1, -2));
            var newBound = grid.Bounds;

            foreach (var i in oldBound.Intersect(newBound))
                grid.Get(i).Should().Be(new Tuple<int, int>(i.X, i.Z), "element at {0}", i);

            foreach (var i in newBound.Substract(oldBound))
                grid.Get(i).Should().Be(null, "element at {0}", i);
        }
    }
}
