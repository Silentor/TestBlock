namespace Silentor.TB.Common.Maps.Blocks
{
    /// <summary>
    /// Simple cubic symmetric static block
    /// </summary>
    public class Block
    {
        public byte Id { get; private set; }

        public string Name { get; private set; }

        public bool IsTransparent
        {
            get { return Id < BlockSet.OpaqueStartIndex; }
        }

        public Block(byte id)
        {
            Id = id;
        }

        public static readonly CubeFace[] Faces =
        {
            CubeFace.Front,
            CubeFace.Back,
            CubeFace.Right,
            CubeFace.Left,
            CubeFace.Top,
            CubeFace.Bottom
        };
    }
}

