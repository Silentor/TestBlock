using System;
using System.Linq;
using FluentAssertions;
using Lidgren.Network;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Chunks;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;
using Silentor.TB.Common.Network.Serialization;

namespace Silentor.TB.Common.Tests
{
    [TestFixture]
    public class SerializerTests
    {
        private MessageSerializer _serializer;
        private MessageFactory _messageFactory;

        [TestFixtureSetUp]
        public void Init()
        {
            _messageFactory = new MessageFactory();
            _messageFactory.AddCustomMessage<TestMessage>();

            _serializer = new MessageSerializer(_messageFactory);
        }

        /// <summary>
        /// Test serialization/deserialization of some good compressible data
        /// </summary>
        /// <param name="isCompressed"></param>
        [TestCase(true)]
        [TestCase(false)]
        public void TestCompressibleStream(bool isCompressed)
        {
            var data = GenerateCompressibleData(100);

            var cmd = new TestMessage(data);
            int size;
            var serialized = _serializer.Serialize(cmd, out size, isCompressed);
            Array.Resize(ref serialized, size);

            var cmd2 = (TestMessage)_serializer.Deserialize(serialized);

            Assert.That(cmd.Data, Is.EqualTo(data));
            Assert.That(cmd2.Data, Is.EqualTo(data));
        }

        /// <summary>
        /// Test serialization/deserialization of some very bad compressible data
        /// </summary>
        /// <param name="isCompressed"></param>
        [TestCase(true)]
        [TestCase(false)]
        public void TestUncompressibleStream(bool isCompressed)
        {
            var data = GenerateNoncompressibleData(100);

            var cmd = new TestMessage(data);
            int size;
            var serialized = _serializer.Serialize(cmd, out size);
            Array.Resize(ref serialized, size);

            var cmd2 = (TestMessage)_serializer.Deserialize(serialized);

            Assert.That(cmd.Data, Is.EqualTo(data));
            Assert.That(cmd2.Data, Is.EqualTo(data));
        }

        [Test]
        public void TestChunkContentSerialization()
        {
            //Arrange
            var blockTypes = new Block[] {new Block(1), new Block(2)};
            var blocks = new BlockData[ChunkContents.BlocksCount];
            for (int i = 0; i < blocks.Length; i++)
                blocks[i] = new BlockData(blockTypes[i%2]);
            var heightMap = GenerateCompressibleData(ChunkContents.SizeX*ChunkContents.SizeZ);
            var chunkContent = new ChunkContents(Vector2i.One, blocks, heightMap);
            var chunkMessage = new ChunkMessage(chunkContent);

            //Act
            int serializedSize;
            var serialized = _serializer.Serialize(chunkMessage, out serializedSize);

            var serialized2 = new Byte[serializedSize];
            Array.Copy(serialized, serialized2, serializedSize);
            var chunkMessage2 = (ChunkMessage)_serializer.Deserialize(serialized2);

            //Assert
            serializedSize.Should().BeLessThan(chunkMessage.Size);           //Because of suggested compression
            Assert.That(chunkMessage.Header == chunkMessage2.Header);
            Assert.That(chunkMessage.Position == Vector2i.One);
            Assert.That(chunkMessage.Position == chunkMessage2.Position);
            chunkMessage.HeightMap.Should().Equal(chunkMessage2.HeightMap);
            chunkMessage.Blocks.Should().Equal(chunkMessage2.Blocks);
        }

        [Test]
        public void TestNetBufferSerialization()
        {
            var data = GenerateNoncompressibleData(100);

            var cmd = new TestMessage(data);
            var buffer = new NetBuffer();
            var serialized = _serializer.Serialize(cmd, buffer);

            var cmd2 = (TestMessage)_serializer.Deserialize(serialized);

            Assert.That(cmd.Data, Is.EqualTo(data));
            Assert.That(cmd2.Data, Is.EqualTo(data));
        }

        private byte[] GenerateNoncompressibleData(int size)
        {
            var rnd = new Random();
            var result = new byte[size];
            rnd.NextBytes(result);

            return result;
        }

        private byte[] GenerateCompressibleData(int size)
        {
            var rnd = new Random();
            var result = new byte[size];
            var value = rnd.NextDouble() * 255d;

            for (var i = 0; i < size; i++)
            {
                value += 255 % rnd.NextDouble()/5;
                result[i] = (byte) value;
            }

            return result;
        }
    }

    public class TestMessage : TestMessageBase
    {
        public TestMessage(byte[] data)
        {
            if(data == null)
                Data = new byte[0];
            else
                Data = data;
        }

        internal TestMessage(NetBuffer buffer)
        {
            var length = buffer.ReadVariableInt32();
            Data = buffer.ReadBytes(length); 
        }

        public readonly byte[] Data = new byte[0];

        [Header(Headers.Test - 1)]
        public override Headers Header
        {
            get { return Headers.Test - 1; }
        }

        public override int Size
        {
            get { return 4 + Data.Length; }
        }

        protected override void SerializeTest(NetBuffer buffer)
        {
            buffer.WriteVariableInt32(Data.Length);         //4
            buffer.Write(Data);                             //?
        }
    }
}
