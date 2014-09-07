using ModestTree.Zenject;
using Silentor.TB.Client.Maps;
using Silentor.TB.Client.Tools;
using Silentor.TB.Common.Maps;
using Silentor.TB.Common.Maps.Geometry;
using UnityEngine;

namespace Assets.Code.Visualization
{
    /// <summary>
    /// Draws some Unity scene gizmos for map
    /// </summary>
    public class MapGizmos : MonoBehaviour
    {
        public bool ShowLoadRadius;
        public Color LoadRadiusColor = Color.yellow;

        [HideInInspector]
        [Inject]
        public IMap Map;

        void OnDrawGizmos()
        {
            if (Map == null) return;

            if (ShowLoadRadius)
            {
                Gizmos.color = LoadRadiusColor;
                var mapBounds = Map.Bounds;
                var minCoord = Chunk.ToWorldPosition(mapBounds.Min, 0, 0, 0);
                var maxCoord = Chunk.ToWorldPosition(mapBounds.Max, Chunk.SizeX - 1, Chunk.SizeY - 1, Chunk.SizeZ - 1);
                var unityBounds = new Bounds3i(minCoord, maxCoord).ToUnityBounds();
                Gizmos.DrawWireCube(unityBounds.center, unityBounds.size);
            }
        }


    }
}
