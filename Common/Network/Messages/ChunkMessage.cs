using System;
using Lidgren.Network;
using Silentor.TB.Common.Exceptions;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Chunks;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Serialization;

namespace Silentor.TB.Common.Network.Messages
{
    public class ChunkMessage : Message
    {
        public Vector2i Position { get; private set; }

        public byte[] HeightMap { get; private set; }

        public BlockData[] Blocks { get; private set; }

        [Header(Headers.ChunkResponce)]
        public override Headers Header
        {
            get { return Headers.ChunkResponce; }
        }

        public override int Size
        {
            get { return 1 + 8 + 256 + 32768*2; }
        }

        public override DeliveryMethod Delivery { get { return Settings.Chunk; } }

        public ChunkMessage(Vector2i position, BlockData[] blocks, byte[] heightmap)
        {
            if (blocks == null) throw new ArgumentNullException("blocks");
            if (heightmap == null) throw new ArgumentNullException("heightmap");
            if (blocks.Length != ChunkContents.BlocksCount)
                throw new ArgumentOutOfRangeException("blocks", blocks.Length, "Blocks count incorrect");
            if (heightmap.Length != ChunkContents.SizeX * ChunkContents.SizeZ)
                throw new ArgumentOutOfRangeException("heightmap", heightmap.Length, "Heightmap count incorrect");

            Blocks = blocks;
            HeightMap = heightmap;
            Position = position;
        }

        public ChunkMessage(ChunkContents contents) : this(contents.Position, contents.Blocks, contents.HeightMap)
        {
        }

        /// <summary>
        /// Deserialization
        /// </summary>
        internal ChunkMessage(NetBuffer buffer)
        {
            //todo Decompress it firstly

            Position = buffer.ReadVector2i();
            HeightMap = buffer.ReadBytes(ChunkContents.SizeX*ChunkContents.SizeZ);

            var blockBuffer = buffer.ReadBytes(2*ChunkContents.BlocksCount);

            if(Blocks == null)
                Blocks = new BlockData[ChunkContents.BlocksCount];

            for (var i = 0; i < ChunkContents.BlocksCount; i++)
                Blocks[i] = new BlockData(blockBuffer[i*2], blockBuffer[i*2 + 1]);
        }

        public override void Serialize(NetBuffer buffer)
        {
            base.Serialize(buffer);                                 //1

            buffer.Write(Position);                                 //8
            buffer.Write(HeightMap);                                //256

            var blockBuffer = new byte[2* ChunkContents.BlocksCount];
            for (var i = 0; i < ChunkContents.BlocksCount; i++)
            {
                blockBuffer[i] = Blocks[i * 2].Id;
                blockBuffer[i + 1] = Blocks[i * 2 + 1].Data;
            }
            buffer.Write(blockBuffer);                              //32768 * 2

            //todo Compress it!
        }

        public override string ToString()
        {
            return string.Format("[ChunkContents: ({0}, {1})]", Position.X, Position.Z);
        }
    }
}
