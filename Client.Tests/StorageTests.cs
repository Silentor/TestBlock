using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Silentor.TB.Client.Maps;
using Silentor.TB.Client.Storage;
using Silentor.TB.Client.Tests.Support;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Chunks;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Client.Tests
{
    [TestFixture]
    public class StorageTests
    {
        [Test]
        public void StorageAddTest()
        {
            var appEvents = new TestApplicationEvents();
            var sut = new ChunkStorage(appEvents, new TestGameConfig {ChunkCacheSize = 3});
            
            var c1 = GetChunkContents(Vector2i.Zero);
            var c2 = GetChunkContents(new Vector2i(0, 1));
            var c3 = GetChunkContents(new Vector2i(1, 1));
            var c4 = GetChunkContents(new Vector2i(2, 1));

            //Should store any amount of chunks
            sut.Store(c1);
            sut.Store(c2);
            sut.Store(c3);
            sut.Store(c4);

            appEvents.DoFrameTick(4);
        }

        [Test]
        public void StorageSimpleRetrieveTest()
        {
            var appEvents = new TestApplicationEvents();
            var sut = new ChunkStorage(appEvents, new TestGameConfig { ChunkCacheSize = 3 });

            var c1 = GetChunkContents(Vector2i.Zero);
            var c2 = GetChunkContents(new Vector2i(0, 1));
            var c3 = GetChunkContents(new Vector2i(1, 1));
            var c4 = GetChunkContents(new Vector2i(2, 1));

            sut.Store(c1);
            sut.Store(c2);
            sut.Store(c3);
            sut.Store(c4);

            appEvents.DoFrameTick(4);

            //Assert
            sut.Retrieve(c1.Position).Should().BeFalse("first chunk must be already dropped from storage");
            sut.Retrieve(c2.Position).Should().BeTrue("this chunk must be still stored in storage");
            sut.Retrieve(c3.Position).Should().BeTrue("this chunk must be still stored in storage");
            sut.Retrieve(c4.Position).Should().BeTrue("this chunk must be still stored in storage");
        }

        [Test]
        public void StorageAddAfterRetrieveTest()
        {
            var appEvents = new TestApplicationEvents();
            var sut = new ChunkStorage(appEvents, new TestGameConfig { ChunkCacheSize = 3 });

            var c1 = GetChunkContents(Vector2i.Zero);
            var c2 = GetChunkContents(new Vector2i(0, 1));
            var c3 = GetChunkContents(new Vector2i(1, 1));
            var c4 = GetChunkContents(new Vector2i(2, 1));
            var c5 = GetChunkContents(new Vector2i(2, 2));

            sut.Store(c1);
            sut.Store(c2);
            sut.Store(c3);
            sut.Store(c4);

            appEvents.DoFrameTick(4);

            //Renew one of old chunks
            sut.Retrieve(c2.Position);
            //Store another chunk
            sut.Store(c5);

            appEvents.DoFrameTick();
            
            //Assert than c3 chunk is dropped
            sut.Retrieve(c3.Position).Should().BeFalse("chunk must be dropped due to old access time");
        }

        [Test]
        public void AsyncRetrieveTest()
        {
            var appEvents = new TestApplicationEvents();
            var sut = new ChunkStorage(appEvents, new TestGameConfig { ChunkCacheSize = 3 });

            var c1 = GetChunkContents(Vector2i.Zero);

            sut.Store(c1);

            appEvents.DoFrameTick();

            bool chunkRetrieved = false;
            sut.Retrieved += contents =>
            {
                if (contents.Position == c1.Position && contents.Blocks.SequenceEqual(c1.Blocks))
                    chunkRetrieved = true;
            };

            sut.Retrieve(c1.Position).Should().BeTrue("chunk was asynchronuosly retrieved");

            //Tick to async retrieving
            appEvents.DoFrameTick();

            //Assert that chunk was async retrieved
            chunkRetrieved.Should().BeTrue();
        }

        private ChunkContents GetChunkContents(Vector2i position)
        {
            return new ChunkContents(position, new BlockData[Chunk.BlocksCount], new byte[Chunk.SizeX * Chunk.SizeZ]);
        }
    }
}
