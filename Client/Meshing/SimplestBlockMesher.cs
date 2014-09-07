using System;
using JetBrains.Annotations;
using Silentor.TB.Common.Maps.Blocks;
using UnityEngine;

namespace Silentor.TB.Client.Meshing
{
    /// <summary>
    /// Creates mesh for block
    /// </summary>
    public class SimplestBlockMesher
    {
        /// <summary>
        /// Block for process
        /// </summary>
        public Block Block { get; private set; }

        public SimplestBlockMesher(IBlocksAtlas atlas, Block block, string textureName)
        {
            if (block == null) throw new ArgumentNullException("block");
            if (textureName == null) throw new ArgumentNullException("textureName");

            Block = block;
            var material = block.IsTransparent ? atlas.Transparent : atlas.Opaque;
            _materialId = Array.IndexOf(atlas.Materials, material);
            if(_materialId < 0) throw new ArgumentException("Material not found");
            _front = atlas.Textures[textureName];
        }

        /// <summary>
        /// Create mesh for block
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="mesh"></param>
        /// <param name="visibleFaces"></param>
        public void Render(Vector3 offset, [NotNull] MeshData mesh, int visibleFaces)
        {
            if (mesh == null) throw new ArgumentNullException("mesh");
            if(visibleFaces == 0) return;

            for (var i = 0; i < 6; i++)
                if ((visibleFaces & (1 << i)) != 0) //If face visible
                    BuildFace(offset, mesh, SimpleCubeVertices[i], BaseFaceLight[i]);
        }

        protected virtual void BuildFace(Vector3 offset, MeshData mesh, Vector3[] vertices, byte light)
        {
            //Fill indices
            var indices = mesh.GetIndices(_materialId);
            var start = mesh.Vertices.Count;
            indices.Add(start + 0);
            indices.Add(start + 1);
            indices.Add(start + 2);
            indices.Add(start + 0);
            indices.Add(start + 2);
            indices.Add(start + 3);

            //Fill vertices
            for (var i = 0; i < vertices.Length; i++)
                mesh.Vertices.Add(vertices[i] + offset);

            //Fill UV
            mesh.Uv.Add(new Vector2(_front.xMax, _front.yMin));
            mesh.Uv.Add(new Vector2(_front.xMax, _front.yMax));
            mesh.Uv.Add(new Vector2(_front.xMin, _front.yMax));
            mesh.Uv.Add(new Vector2(_front.xMin, _front.yMin));

            //Fill colors
            mesh.Colors.Add(new Color32(light, light, light, 255));
            mesh.Colors.Add(new Color32(light, light, light, 255));
            mesh.Colors.Add(new Color32(light, light, light, 255));
            mesh.Colors.Add(new Color32(light, light, light, 255));
        }

        protected readonly Rect _front = new Rect(0, 0, 1, 1);
        private readonly int _materialId;

        #region Mesh stuff

        //Cube block vertices
        private static readonly Vector3[][] SimpleCubeVertices = new[]
        {
            //Front
            new[]
            {
                new Vector3(1f, 0f, 1f),
                new Vector3(1f, 1f, 1f),
                new Vector3(0f, 1f, 1f),
                new Vector3(0f, 0f, 1f)
            },
            //Back
            new[]
            {
                new Vector3(0f, 0f, 0f),
                new Vector3(0f, 1f, 0f),
                new Vector3(1f, 1f, 0f),
                new Vector3(1f, 0f, 0f)
            },
            //Right
            new[]
            {
                new Vector3(1f, 0f, 0f),
                new Vector3(1f, 1f, 0f),
                new Vector3(1f, 1f, 1f),
                new Vector3(1f, 0f, 1f)
            },
            //Left
            new[]
            {
                new Vector3(0f, 0f, 1f),
                new Vector3(0f, 1f, 1f),
                new Vector3(0f, 1f, 0f),
                new Vector3(0f, 0f, 0f)
            },
            //Top
            new[]
            {
                new Vector3(0f, 1f, 0f),
                new Vector3(0f, 1f, 1f),
                new Vector3(1f, 1f, 1f),
                new Vector3(1f, 1f, 0f)
            },
            //Bottom
            new[]
            {
                new Vector3(1f, 0f, 0f),
                new Vector3(1f, 0f, 1f),
                new Vector3(0f, 0f, 1f),
                new Vector3(0f, 0f, 0f)
            }
        };

        /// <summary>
        /// Lookup table for base face lights at day
        /// </summary>
        private static readonly byte[] BaseFaceLight =
        {
            210, //front
            210, //back
            180, //right
            180, //left
            255, //top
            120 //bottom
        };

        #endregion
    }
}
