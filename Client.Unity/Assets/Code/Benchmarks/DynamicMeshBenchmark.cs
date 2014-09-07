using System.Linq;
using UnityEngine;

namespace Assets.Code.Benchmarks
{
    public class DynamicMeshBenchmark : MonoBehaviour
    {
        public Material Material;

        private GameObject _meshGo;
        private MeshFilter _filter;

        void Start ()
        {
            _meshGo = new GameObject("MeshGO", typeof (MeshRenderer), typeof (MeshFilter));
            _filter = _meshGo.GetComponent<MeshFilter>();
            _filter.sharedMesh = new Mesh();
            _filter.sharedMesh.Clear(false);
            _filter.sharedMesh.subMeshCount = 1;
            _meshGo.GetComponent<Renderer>().sharedMaterial = Material;
        }

        // Update is called once per frame
        void Update ()
        {
            var mesh = _filter.sharedMesh;

            var vertices = mesh.vertices.ToList();
            for (int i = 0; i < 3; i++)
                vertices.Add(Random.onUnitSphere);

            var indices = mesh.triangles.ToList();
            for (int i = 0; i < 3; i++)
                indices.Add(indices.Count);

            mesh.vertices = vertices.ToArray();
            mesh.triangles = indices.ToArray();

            mesh.RecalculateBounds();
        }
    }
}
