using Silentor.TB.Common.Maps.Geometry;

namespace Silentor.TB.Common.Maps.Blocks
{
    public struct BlockData
    {
        #region Block data

        public byte Id { get { return _blockId; } }
        public byte Data;

        #endregion

        /// <summary>
        /// Is block need rendering
        /// </summary>
        public bool IsBlockVisible
        {
            get { return !IsEmpty; /*(FacesMask & VisibleBlockMask) != 0; }*/ }
        }

        /// <summary>
        /// Is block rendererd as semitransparent
        /// </summary>
        public bool IsTransparent
        {
            get { return Id < BlockSet.OpaqueStartIndex; }
        }

        public bool IsEmpty { get { return _blockId == BlockSet.AirID; } }

        public bool IsNull { get { return _blockId == BlockSet.NullID; } }

        public BlockData(Block block, byte data = 0)
        {
            _blockId = block != null ? block.Id : BlockSet.AirID;
            Data = data;

            //Calculate cache bits
            if (_blockId != BlockSet.AirID)
            {
                if (block != null)
                {
                    //if (!block.Transparent)
                    //    FacesMask |= SolidBlockMask;
                    //FacesMask |= VisibleBlockMask;
                }
            }
        }

        internal BlockData(byte id, byte data)
        {
            _blockId = id;
            Data = data;
        }

        /// <summary>
        /// mask of ab______ bits: a - is alpha block, b - is block completely invisible (empty or surrounded by nontranparent blocks)
        /// </summary>
        //public byte FacesMask;

        //private const byte SolidBlockMask = 1 << 7;
        //public const byte VisibleBlockMask = 1 << 6;

        private readonly byte _blockId;

        /// <summary>
        /// Absence of any block (chunk not loaded etc)
        /// </summary>
        public static readonly BlockData Null = new BlockData(BlockSet.NullID, 0);

        #region Object and operators overrides

        public override string ToString()
        {
            return string.Format("[BlockData Id: {0}, Data: {1}]", Id, Data);
        }

        public override bool Equals(object obj)
        {
            if (obj is BlockData)
                return Equals((BlockData) obj);
            return false;
        }

        private bool Equals(BlockData block)
        {
            return _blockId == block._blockId;
        }

        /// <summary>
        /// Equality of block types
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(BlockData lhs, BlockData rhs)
        {
            return lhs._blockId == rhs._blockId;
        }

        /// <summary>
        /// Unequality of block types
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=(BlockData lhs, BlockData rhs)
        {
            return lhs._blockId != rhs._blockId;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        #endregion

    }

    /// <summary>
    /// BlockData and his world position
    /// </summary>
    public struct BlockDataPosition
    {
        public BlockData BlockData;
        public Vector3i Position;

        public BlockDataPosition(BlockData block, Vector3i position)
        {
            BlockData = block;
            Position = position;
        }
    }
}