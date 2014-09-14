using System;

namespace Silentor.TB.Common.Maps.Blocks
{
    public enum CubeFace
    {
        Front  = 0,
        Back   = 1,
        Right  = 2,
        Left   = 3,
        Top    = 4,
        Bottom = 5,
    }

    /// <summary>
    /// For simplified named bit access
    /// </summary>
    [Flags]
    public enum CubeFaceMask
    {
        Front = 1 << CubeFace.Front,
        Back = 1 << CubeFace.Back,
        Right = 1 << CubeFace.Right,
        Left = 1 << CubeFace.Left,
        Top = 1 << CubeFace.Top,
        Bottom = 1 << CubeFace.Bottom,
    }
}