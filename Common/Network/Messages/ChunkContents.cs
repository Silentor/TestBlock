using System;
using ProtoBuf;
using Silentor.TB.Common.Exceptions;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Geometry;

namespace Silentor.TB.Common.Network.Messages
{
    [ProtoContract(SkipConstructor = true)]
    public class ChunkContents : Message, IChunkContent
    {
        public const int SizeXBits = 4;
        public const int SizeYBits = 7;
        public const int SizeZBits = 4;
        public const int SizeX = 1 << SizeXBits;
        public const int SizeY = 1 << SizeYBits;
        public const int SizeZ = 1 << SizeZBits;
        public const int BlocksCount = SizeX * SizeY * SizeZ;

        public override Headers Header
        {
            get { return Headers.ChunkResponce; }
        }

        public override DeliveryMethod Delivery { get { return Settings.Chunk; } }

        #region Protobuf
        [ProtoMember(1, IsRequired = true)]
        public Vector2i Position { get; private set; }

        [ProtoMember(2, OverwriteList = true, IsPacked = true, IsRequired = true)]
        private byte[] _blockId;

        [ProtoMember(3, OverwriteList = true, IsPacked = true, IsRequired = true)]
        private byte[] _blockData;

        [ProtoMember(5, OverwriteList = true, IsPacked = true, IsRequired = true)]
        public byte[] HeightMap { get; private set; }


        [ProtoBeforeSerialization]
        private void Serialize()
        {
            _blockId = new byte[BlocksCount];
            _blockData = new byte[BlocksCount];

            for (var i = 0; i < BlocksCount; i++)
            {
                _blockId[i] = Blocks[i].Id;
                _blockData[i] = Blocks[i].Data;
            }
        }

        [ProtoAfterSerialization]
        private void SerializeCleanup()
        {
            _blockId = _blockData = null;
        }

        [ProtoAfterDeserialization]
        private void Deserialize()
        {
            if (_blockId.Length != BlocksCount) throw new ChunkException("Invalid blocks id array lenght");
            if (_blockData.Length != BlocksCount) throw new ChunkException("Invalid blocks data array lenght");

            if (Blocks == null)
                Blocks = new BlockData[BlocksCount];

            for (var i = 0; i < BlocksCount; i++)
                Blocks[i] = new BlockData(_blockId[i], _blockData[i]);

            SerializeCleanup();
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

        private ChunkContents()
        {
            //Blocks = new BlockData[Chunk.BlocksCount];
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
