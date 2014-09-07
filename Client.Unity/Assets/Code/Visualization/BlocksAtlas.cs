using System;
using System.Collections;
using Assets.Scripts.Tools;
using Silentor.TB.Client.Meshing;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Visualization
{
    /// <summary>
    /// Contains textures and materials for blocks
    /// </summary>
    public class BlocksAtlas : ScriptableObject, IBlocksAtlas
    {
        //Unity references
        [SerializeField]
        private Material OpaqueLink;
        [SerializeField]
        private Material TransparentLink;
        [SerializeField]
        private TextAsset Description;

        public Material Opaque { get; private set; }
        public Material Transparent { get; private set; }
        public Material[] Materials { get; private set; }

        /// <summary>
        /// Textures "name" - "texture coordinates"
        /// </summary>
        public Dictionary<string, Rect> Textures { get { return _textures; } }

        private readonly Dictionary<string, Rect> _textures = new Dictionary<string, Rect>();

        private int _atlasWidth, _atlasHeight;

        void OnEnable() 
        {
            if (OpaqueLink == null) throw new ArgumentException("Atlas Opaque is null!");
            if (TransparentLink == null) throw new ArgumentException("Atlas Transparent is null!");
            if (Description == null) throw new ArgumentException("Atlas Description is null!");

            Materials = new[] { OpaqueLink, TransparentLink };
            Opaque = OpaqueLink;
            Transparent = TransparentLink;

            var atlasData = (Hashtable)MiniJSON.jsonDecode(Description.text);

            if (!MiniJSON.lastDecodeSuccessful())
                throw new InvalidOperationException(
                    string.Format("Error loading atlas description! Name {0}, position {1}, snippet: {2}",
                        Description.name, MiniJSON.getLastErrorIndex(), MiniJSON.getLastErrorSnippet()));

            //Parse TexturePacker atlas description
            //Parse metadata
            var meta = (Hashtable)atlasData["meta"];
            var size = (Hashtable)meta["size"];
            _atlasWidth = Convert.ToInt32((double)size["w"]);
            _atlasHeight = Convert.ToInt32((double)size["h"]);

            //Parse textures
            var frames = (Hashtable)atlasData["frames"];
            foreach (DictionaryEntry sprite in frames)
            {
                var name = sprite.Key.ToString();

                // Extract the info we need from the TexturePacker json file, mainly uvRect and size
                var spriteData = (Hashtable)sprite.Value;
                var frame = (Hashtable)spriteData["frame"];
                var rectPixels = new Rect((float)(double)frame["x"], (float)(double)frame["y"], (float)(double)frame["w"], (float)(double)frame["h"]);
                var rectTex = ConvertToTexCoords(rectPixels, _atlasWidth, _atlasHeight);

                _textures.Add(name, rectTex);
            }       
     
            Debug.Log("Loaded blocks textures: " + _textures.Count);
        }

        /// <summary>
        /// Convert from top-left based pixel coordinates to bottom-left based UV coordinates.
        /// TODO code borrowed from NGUI
        /// </summary>
        private static Rect ConvertToTexCoords(Rect rect, int width, int height)
        {
            Rect final = rect;

            if (width != 0f && height != 0f)
            {
                final.xMin = rect.xMin / width;
                final.xMax = rect.xMax / width;
                final.yMin = 1f - rect.yMax / height;
                final.yMax = 1f - rect.yMin / height;
            }

            return final;
        }
    
    }
}
