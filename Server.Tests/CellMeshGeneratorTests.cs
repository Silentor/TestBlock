using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using Silentor.TB.Server.Maps.Voronoi;

namespace Silentor.TB.Server.Tests
{
    [TestFixture]
    public class CellMeshGeneratorTests
    {
        [Test]
        public void TestClockWisePointComparer()
        {
            var center = new Vector2(2, 1);

            var point1 = new Vector2(1, 2);
            var point2 = new Vector2(3, 2);

            var point3 = new Vector2(1, 1);
            var point4 = new Vector2(1, 3);


            CellMeshGenerator.ClockWiseComparer(point1, point2, center).Should().BeTrue();
        }
    }
}
