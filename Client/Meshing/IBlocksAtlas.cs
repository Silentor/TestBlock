using System.Collections.Generic;
using UnityEngine;

namespace Silentor.TB.Client.Meshing
{
    public interface IBlocksAtlas
    {
        Material Opaque { get; }
        Material Transparent { get; }
        Material[] Materials { get; }

        /// <summary>
        /// Textures "name" - "texture coordinates"
        /// </summary>
        Dictionary<string, Rect> Textures { get; }
    }
}