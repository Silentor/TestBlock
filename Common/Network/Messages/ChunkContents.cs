using System;
using Lidgren.Network;
using Silentor.TB.Common.Exceptions;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Serialization;

namespace Silentor.TB.Common.Network.Messages
{
    public class ChunkContents : Message, IChunkContent
    {
        public const int SizeXBits = 4;
        public const int SizeYBits = 7;
        public const int SizeZBits = 4;
        public const int SizeX = 1 << SizeXBits;
        public const int SizeY = 1 << SizeYBits;
        public const int SizeZ = 1 << SizeZBits;
        public const int BlocksCount = SizeX * SizeY * SizeZ;

        [Header(Headers.ChunkResponce)]
        public override Headers Header
        {
            get { return Headers.ChunkResponce; }
        }

        public override DeliveryMethod Delivery { get { return Settings.Chunk; } }

        #region Protobuf
        public Vector2i Position { get; private set; }

        public byte[] HeightMap { get; private set; }
      
        public override void Serialize(NetBuffer buffer)
        {
            base.Serialize(buffer);

            buffer.Write(Position);
            buffer.Write(HeightMap);
            for (int i = 0; i < BlocksCount; i++)
            {
                buffer.Write(Blocks[i].Id);
                buffer.Write(Blocks[i].Data);
            }
        }

        public override void Deserialize(NetBuffer buffer)
        {
            base.Deserialize(buffer);

            Position = buffer.ReadVector2i();
            HeightMap = buffer.ReadBytes(SizeX*SizeZ);

            if(Blocks == null)
                Blocks = new BlockData[BlocksCount];

            for (int i = 0; i < BlocksCount; i++)
                Blocks[i] = new BlockData(buffer.ReadByte(), buffer.ReadByte());
        }

        #endregion

        /// <summary>
        /// Serialized in a special way to byte array
        /// </summary>
        public BlockData[] Blocks { get; private set; }

        public ChunkContents(Vector2i position, BlockData[] blocks, byte[] heightmap)
        {
            if (blocks == null) throw new ArgumentNullException("blocks");
            if (heightmap == null) throw new ArgumentNullException("heightmap");
            if (blocks.Length != BlocksCount)
                throw new ArgumentOutOfRangeException("blocks", blocks.Length, "Blocks count incorrect");
            if (heightmap.Length != SizeX * SizeZ)
                throw new ArgumentOutOfRangeException("heightmap", heightmap.Length, "Heightmap count incorrect");

#if UNITY_EDITOR
                for (var i = 0; i < heightmap.Length; i++)
                    if (heightmap[i] > SizeY)
                        throw new ArgumentException(string.Format("Height in heightmap {0} more than Chunk height {1}",
                            heightmap[i], SizeY));
#endif

            Blocks = blocks;
            HeightMap = heightmap;
            Position = position;
        }

        /// <summary>
        /// Deserialization
        /// </summary>
        internal ChunkContents()
        {
            
        }

        public BlockData GetBlockData(int x, int y, int z)
        {
            return Blocks[y + SizeY * x + SizeY * SizeX * z];
        }

        //todo consider rewrite to Block[ConvertPosition(x, y, z)] = blockData
        public void SetBlockData(int x, int y, int z, BlockData block)
        {
            Blocks[y + SizeY * x + SizeY * SizeX * z] = block;
        }

        public override string ToString()
        {
            return string.Format("[ChunkContents: ({0}, {1})]", Position.X, Position.Z);
        }
    }

    public interface IChunkContent
    {
        Vector2i Position { get; }
        BlockData[] Blocks { get; }
        byte[] HeightMap { get; }

    }
}
