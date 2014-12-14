using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace Silentor.TB.Common.Maps.Blocks
{
    public interface IBlockSet : IEnumerable<Block>
    {
        Block this[int id] { get; }

        Block this[string name] { get; }
    }

    public class BlockSet : IBlockSet
    {
#pragma warning disable 1591

        public const byte OpaqueStartIndex = 128;

        //Transparent
        public const byte AirID = 0;
        public const byte GlassID = 1;
        public const byte ReedsID = 2;

        //Opaque
        public const byte StoneID = OpaqueStartIndex + 0;
        public const byte DirtWithGrassID = OpaqueStartIndex + 1;
        public const byte DirtID = OpaqueStartIndex + 2;
        public const byte BedrockID = OpaqueStartIndex + 3;
        public const byte IceID = OpaqueStartIndex + 4;
        public const byte SandID = OpaqueStartIndex + 5;
        public const byte SnowID = OpaqueStartIndex + 6;
        public const byte LavaID = OpaqueStartIndex + 7;


        public const byte EmptyID = AirID;
        public const byte NullID = 255;

        private static Logger Log = LogManager.GetCurrentClassLogger();

#pragma warning restore 1591

        private void Init()
        {
            AddBlock(new Block(StoneID));
            AddBlock(new Block(DirtID));
            AddBlock(new Block(GlassID));
            //AddBlock(new Block(ReedsID));
            AddBlock(new Block(DirtWithGrassID));
            AddBlock(new Block(SnowID));
            AddBlock(new Block(IceID));
            AddBlock(new Block(SandID));
            AddBlock(new Block(LavaID));

            Log.Info("Added {0} blocks", _blocks.Count(b => b != null));
        }

        private Block AddBlock(Block block)
        {
            if (_blocks[block.Id] != null)
                throw new InvalidOperationException("There is already such block id in the set.");

            _blocks[block.Id] = block;

            return block;
        }

        public BlockSet()
        {
            Init();
        }

        private readonly Block[] _blocks = new Block[256];

        public Block this[int id]
        {
            get
            {
                if (id == AirID) return null;

                var result = _blocks[id];
                if (result == null)
                    throw new ArgumentOutOfRangeException("id", id, "Incorrect block id " + id);

                return result;
            }
        }

        public Block this[string name]
        {
            get
            {
                for (var index = 0; index < _blocks.Length; index++)
                {
                    var block = _blocks[index];
                    if (block.Name == name) return block;
                }

                throw new ArgumentOutOfRangeException("name", name, "Incorrect block id");
            }
        }

        public IEnumerator<Block> GetEnumerator()
        {
            return _blocks.Where(b => b != null).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}