using NSubstitute;
using NUnit.Framework;
using Silentor.TB.Common.Config;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Geometry;
using Wob.Server.Maps;
using Wob.Server.Maps.Generators;

namespace Server.Tests
{
    [TestFixture]
    public class GeneratorTests
    {
        [Test]
        public void TestFlatGenerator()
        {
            var blockSet = Substitute.For<IBlockSet>();
            blockSet[BlockSet.StoneID].Returns(new Block(BlockSet.StoneID));

            var wrld = Substitute.For<IGlobeConfig>();
            wrld.Seed.Returns(666);

            var generator = new Flat(wrld, Chunk.SizeY / 2 - 1, blockSet);
            var data = generator.Generate(Vector2i.Zero).Result;

            //Assert that topmost block is air
            Assert.That(data.Blocks[data.Blocks.Length - 1].IsEmpty);

            //Assert that block just above the heightmap is air
            Assert.That(data.Blocks[Chunk.SizeY / 2].IsEmpty);

            //Assert that block on heightmap level is not air
            Assert.That(!data.Blocks[Chunk.SizeY / 2 - 1].IsEmpty);
        }
    }
}
