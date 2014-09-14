using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Silentor.TB.Client.Maps;
using Silentor.TB.Client.Meshing;
using Silentor.TB.Client.Visualization;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Tools;
using UnityEngine;

namespace Assets.Code.Visualization
{
    /// <summary>
    /// Create views for Chunklets
    /// </summary>
    public interface IChunkletViewFactory
    {
        ChunkletView Create(Vector3i position);

        void Dispose([NotNull] ChunkletView chunkletView);
    }

    public class ChunkletViewFactory : IChunkletViewFactory
    {
        public ChunkletViewFactory(IBlocksAtlas atlas, MapGizmos map)
        {
            _atlas = atlas;
            _map = map.transform;
            _map.transform.parent = null;
        }

        public ChunkletView Create(Vector3i position)
        {
            if (_cache == null)
                _cache = new Stack<ChunkletView>();

            GameObject go;
            ChunkletView result;

            if (_cache.Count > 0)
            {
                result = _cache.Pop();
                go = result.gameObject;
                go.SetActive(true);
            }
            else
            {
                go = new GameObject();
                go.transform.parent = _map;
                go.transform.localRotation = Quaternion.identity;
                result = go.AddComponent<ChunkletView>();
                go.AddComponent<MeshRenderer>().sharedMaterials = _atlas.Materials;
                go.renderer.castShadows = false;
                go.renderer.receiveShadows = false;
                
            }

            go.transform.localPosition = new Vector3(position.X * Chunklet.SizeX, position.Y * Chunklet.SizeY, position.Z * Chunklet.SizeZ);
            go.name = String.Format("({0}, {1}, {2})", position.X, position.Y, position.Z);

            return result;
        }

        public void Dispose(ChunkletView chunkletView)
        {
            if (ReferenceEquals(chunkletView, null)) throw new ArgumentNullException("chunkletView");

            chunkletView.Destroy();
            _cache.Push(chunkletView);
        }

        private static Stack<ChunkletView> _cache;
        private readonly IBlocksAtlas _atlas;
        private readonly Transform _map;
    }
}
