using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Silentor.TB.Server.Players;

namespace Server.Tests
{
    [TestFixture]
    public class SnapshotCompareTests
    {
        private IPlayer[] _players;

        [TestFixtureSetUp]
        public void Init()
        {
            //Create snapshots
            _players = new IPlayer[10];
            for (var i = 0; i < _players.Length; i++)
            {
                var player = Substitute.For<IPlayer>();
                player.Id.Returns(i);
                _players[i] = player;
            }
        }

        [Test]
        public void TestComparison()
        {
            var oldList = new List<IPlayer>()
            {
                _players[0],
                _players[1],
                _players[4],
                _players[6],
                _players[7],
                _players[9]
            };

            var newList = new List<IPlayer>()
            {
                _players[2],
                _players[3],
                _players[5],
                _players[6],
                _players[7],
                _players[8]
            };

            var result = new Sensor.SnapshotCompare(oldList, newList);

            //Assert
            result.Same.Select(p => p.Id).Should().Equal(new[]{6, 7});
            result.Removed.Select(p => p.Id).Should().Equal(new[] { 0, 1, 4, 9 });
            result.Added.Select(p => p.Id).Should().Equal(new[] { 2, 3, 5, 8 });
        }

        [Test]
        public void TestComparison2()
        {
            var oldList = new List<IPlayer>()
            {
                _players[0],
                _players[1],
                _players[2],
            };

            var newList = new List<IPlayer>()
            {
            };

            var result = new Sensor.SnapshotCompare(oldList, newList);

            //Assert
            result.Same.Select(p => p.Id).Should().Equal(new int[0]);
            result.Removed.Select(p => p.Id).Should().Equal(new[] { 0, 1, 2 });
            result.Added.Select(p => p.Id).Should().Equal(new int[0]);
        }

        [Test]
        public void TestComparison3()
        {
            var oldList = new List<IPlayer>()
            {
            };

            var newList = new List<IPlayer>()
            {
                _players[0],
                _players[1],
                _players[2],

            };

            var result = new Sensor.SnapshotCompare(oldList, newList);

            //Assert
            result.Same.Select(p => p.Id).Should().Equal(new int[0]);
            result.Removed.Select(p => p.Id).Should().Equal(new int[0]);
            result.Added.Select(p => p.Id).Should().Equal(new[] { 0, 1, 2 });
        }

        [Test]
        public void TestComparison4()
        {
            var oldList = new List<IPlayer>()
            {
                _players[0],
                _players[1],
                _players[2],
            };

            var newList = new List<IPlayer>()
            {
                _players[0],
                _players[1],
                _players[2],

            };

            var result = new Sensor.SnapshotCompare(oldList, newList);

            //Assert
            result.Same.Select(p => p.Id).Should().Equal(new[] { 0, 1, 2 });
            result.Removed.Select(p => p.Id).Should().Equal(new int[0]);
            result.Added.Select(p => p.Id).Should().Equal(new int[0]);
        }

        [Test]
        public void TestComparison5()
        {
            var oldList = new List<IPlayer>()
            {
            };

            var newList = new List<IPlayer>()
            {
            };

            var result = new Sensor.SnapshotCompare(oldList, newList);

            //Assert
            result.Same.Select(p => p.Id).Should().Equal(new int[0]);
            result.Removed.Select(p => p.Id).Should().Equal(new int[0]);
            result.Added.Select(p => p.Id).Should().Equal(new int[0]);
        }

        [Test]
        public void TestComparison6()
        {
            var oldList = new List<IPlayer>()
            {
                _players[0],
                _players[1],
                _players[2],
            };

            var newList = new List<IPlayer>()
            {
                _players[5],
                _players[6],
                _players[7],
            };

            var result = new Sensor.SnapshotCompare(oldList, newList);

            //Assert
            result.Same.Select(p => p.Id).Should().Equal(new int[0]);
            result.Removed.Select(p => p.Id).Should().Equal(new int[]{0, 1, 2});
            result.Added.Select(p => p.Id).Should().Equal(new int[]{5, 6, 7});
        }
    }
}
