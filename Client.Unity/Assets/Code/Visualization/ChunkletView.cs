using System;
using Silentor.TB.Client.Maps;
using Silentor.TB.Client.Meshing;
using UnityEngine;

namespace Silentor.TB.Client.Visualization
{
    /// <summary>
    /// GameObject for rendered chunklet in the scene
    /// </summary>
    public class ChunkletView : MonoBehaviour
    {
        /// <summary>
        /// Is mesh empty?
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                if (_isDestroyed) throw new InvalidOperationException("Access to destroyed chunklet " + name);
                return _isEmpty;
            }
        }

        public bool IsActive { get; private set; }

        //Chunklet debug info
        private bool IsChunkletEmpty { get; set; }
        private bool IsChunkletBuilding { get; set; }
        private bool IsChunkletSolid { get; set; }
        private bool IsChunkletHidden { get; set; }
        private bool IsChunkletNearPlayer { get; set; }


        /// <summary>
        /// Disable chunklet when out of look raduis
        /// </summary>
        public void Disable()
        {
            if (_isDestroyed) throw new InvalidOperationException("Access to destroyed chunklet " + name);

            _filter.sharedMesh.Clear(false);
            _isEmpty = true;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Destroy chunklet
        /// </summary>
        public void Destroy()
        {
            if (_isDestroyed) return;

            Disable();
            _isDestroyed = true;
            name = "Cached " + name;
        }

        public void Init(Chunklet chunklet)
        {
            _chunklet = chunklet;
        }

        /// <summary>
        /// Draw chunklet. Unity main thread
        /// </summary>
        /// <param name="meshData"></param>
        public void SetMesh(MeshData meshData)
        {
            if (meshData == null) throw new ArgumentNullException("meshData");

            var result = meshData.ToMesh(_filter.sharedMesh);
            _isEmpty = _filter.sharedMesh.vertexCount == 0;
        }

        private MeshFilter _filter;
        private bool _isDestroyed;
        private bool _isEmpty = true;
        private Chunklet _chunklet;

        //public static bool FixCoords(ref Vector3i chunk, ref Vector3i local)
        //{
        //    bool changed = false;
        //    if (local.x < 0)
        //    {
        //        chunk.x--;
        //        local.x += Chunklet.SIZE_X;
        //        changed = true;
        //    }
        //    if (local.y < 0)
        //    {
        //        chunk.y--;
        //        local.y += Chunklet.SIZE_Y;
        //        changed = true;
        //    }
        //    if (local.z < 0)
        //    {
        //        chunk.z--;
        //        local.z += Chunklet.SIZE_Z;
        //        changed = true;
        //    }

        //    if (local.x >= Chunklet.SIZE_X)
        //    {
        //        chunk.x++;
        //        local.x -= Chunklet.SIZE_X;
        //        changed = true;
        //    }
        //    if (local.y >= Chunklet.SIZE_Y)
        //    {
        //        chunk.y++;
        //        local.y -= Chunklet.SIZE_Y;
        //        changed = true;
        //    }
        //    if (local.z >= Chunklet.SIZE_Z)
        //    {
        //        chunk.z++;
        //        local.z -= Chunklet.SIZE_Z;
        //        changed = true;
        //    }
        //    return changed;
        //}

        #region Unity

        void Awake()
        {
            _filter = gameObject.AddComponent<MeshFilter>();
            _filter.sharedMesh = new Mesh();
        }

        void OnEnable()
        {
            IsActive = true;
#if UNITY_EDITOR
            //StartCoroutine(UpdateChunkletFlags());
#endif
        }

        void OnDisable()
        {
            IsActive = false;
        }

        void OnDestroy()
        {
            Destroy(_filter.sharedMesh);
            _filter.sharedMesh = null;
        }

        void OnDrawGizmosSelected()
        {
            //Draw chunklet bounds
            Gizmos.color = Color.white;

            var p1 = transform.position;
            Gizmos.DrawWireCube(p1 + Vector3.one * Chunklet.SizeX / 2f, Vector3.one * Chunklet.SizeX);
        }
        #endregion

        [Flags]
        public enum ChunkletMeshPart
        {
            None = 0,
            Center = 1, 
            North = 2, 
            South = 4, 
            East = 8, 
            West = 16, 
            NorthEast = 32, 
            NorthWest = 64, 
            SouthEast = 128, 
            SouthWest = 256
        }
    }
}

