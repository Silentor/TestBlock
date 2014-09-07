using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Silentor.Wob.Client.Tools
{
    public class BlockAtlasPostProcessor : AssetPostprocessor
    {
        /// <summary>
        /// Name of block atlas file without extension
        /// </summary>
        public string BlockAtlasName = "Blocks";

        public int KeepMipMapLevel = 4;

        //void OnPostprocessTexture(Texture2D blockAtlasTexture)
        //{
        //    Color[] requiredMipMap = null;

        //    if (!String.IsNullOrEmpty(BlockAtlasName) && BlockAtlasName == Path.GetFileNameWithoutExtension(assetPath))
        //    {
        //        for(var i = 0; i < blockAtlasTexture.mipmapCount; i++)
        //        {
        //            if (i == KeepMipMapLevel)
        //            {
        //                requiredMipMap = blockAtlasTexture.GetPixels(i);
        //            }
        //            else if (i > KeepMipMapLevel)
        //            {
        //                //var mip = blockAtlasTexture.GetPixels(i);
        //                //for (int j = 0; j < mip.Length; j++)
        //                //    mip[j] = new Color(0, 0, 0, 0);
        //                //blockAtlasTexture.SetPixels(mip, i);
        //                blockAtlasTexture.SetPixels(requiredMipMap, i);
        //                Debug.Log(string.Format("Set mipmap level {0} to level {1}", i, KeepMipMapLevel));
        //            }
        //        }

        //        //blockAtlasTexture.mipMapBias = -2f;

        //        blockAtlasTexture.Apply(false, true);
        //    }
            
        //}
        
    }
}
