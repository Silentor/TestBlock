using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Silentor.TB.Client.Meshing
{
    /// <summary>
    /// Contains renderer agnostic mesh data
    /// </summary>
    public class MeshData
    {
        public readonly List<Vector3> Vertices = new List<Vector3>();
        public readonly List<Vector2> Uv = new List<Vector2>();
        public readonly List<Color32> Colors = new List<Color32>();
        private readonly List<int>[] indices;

        public static readonly MeshData Empty = new MeshData(3);

        public MeshData(int subMeshCount)
        {
            indices = new List<int>[subMeshCount];
            for (var i = 0; i < subMeshCount; i++)
                indices[i] = new List<int>();
        }

        public List<int> GetIndices(int index)
        {
            return indices[index];
        }

        public void Clear()
        {
            Vertices.Clear();
            Uv.Clear();
            Colors.Clear();
            for (int index = 0; index < indices.Length; index++)
            {
                indices[index].Clear();
            }
        }

        public Mesh ToMesh(Mesh mesh)
        {
            if (mesh == null) throw new ArgumentNullException("mesh");

            if (Vertices.Count == 0)
            {
                if (Colors.Count > 0)
                    if (Colors.Count == mesh.vertexCount)
                        mesh.colors32 = Colors.ToArray();
                    else //Mesh vertexes changed, need mesh rebuild
                    {
                        Debug.LogWarning(String.Format("Mesh is invalid! meshData colors: {0}, mesh vertices: {1}",
                            Colors.Count, mesh.vertexCount));

                        //mesh = null;
                    }
                else
                    mesh.Clear();
                //{
                //    Logger.Log("Colors count " + colors.Count);
                //    Logger.Log("vertices count " + vertices.Count);
                //    Logger.Log("mesh vertices count " + mesh.vertices.Length);
                //    Logger.Log("mesh colors count " + mesh.colors32.Length);
                //    throw new Exception("mesh is invalid");
                //}
            }
            else
            {
                mesh.Clear();

                mesh.vertices = Vertices.ToArray();
                mesh.colors32 = Colors.ToArray();
                mesh.uv = Uv.ToArray();
                mesh.subMeshCount = indices.Length;
                for (var i = 0; i < indices.Length; i++)
                    mesh.SetTriangles(indices[i].ToArray(), i);
            }

            //todo optimize() mesh?

            return mesh;
        }

        //public void AddToMesh([NotNull] Mesh mesh)
        //{
        //    if (mesh == null) throw new ArgumentNullException("mesh");

        //    var newVertices = new List<Vector3>(mesh.vertices);
        //    newVertices.AddRange(Vertices);
        //    mesh.vertices = newVertices.ToArray();

        //    var newColors = new List<Color32>(mesh.colors32);
        //    newColors.AddRange(Colors);
        //    mesh.colors32 = newColors.ToArray();

        //    var newUv = new List<Vector2>(mesh.uv);
        //    newUv.AddRange(Uv);
        //    mesh.uv = newUv.ToArray();

        //    //if(mesh.subMeshCount != indices.Length) Debug.LogWarning("Submesh count is different while adding to mesh");
        //    mesh.subMeshCount = indices.Length;

        //    for (var i = 0; i < indices.Length; i++)
        //    {
        //        var newIndices = new List<int>(mesh.GetTriangles(i));
        //        newIndices.AddRange(indices[i]);
        //        mesh.SetTriangles(newIndices.ToArray(), i);
        //    }

        //    mesh.RecalculateBounds();
        //}

        //public void Add(MeshData meshData)
        //{
        //    Vertices.AddRange(meshData.Vertices);
        //    Colors.AddRange(meshData.Colors);
        //    Uv.AddRange(meshData.Uv);
        //    for (var i = 0; i < meshData.indices.Length; i++)
        //        indices[i].AddRange(meshData.indices[i]);
        //}

        //public void Rotate(Quaternion rotation)
        //{
        //    for (int i = 0; i < Vertices.Count; i++)
        //    {
        //        Vertices[i] = rotation * Vertices[i];
        //    }
        //}

        //public void Translate(Vector3 translate)
        //{
        //    for (int i = 0; i < Vertices.Count; i++)
        //    {
        //        var vertex = Vertices[i];
        //        vertex += translate;
        //        Vertices[i] = vertex;
        //    }
        //}
    }
}