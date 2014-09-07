using System.IO;
using Assets.Scripts.Visualization;
using UnityEditor;
using UnityEngine;

namespace Silentor.Wob.Client.Tools
{
    public class AssetCreator
    {
        [MenuItem("Assets/Create/Atlas")]
        public static void CreateAtlasAsset()
        {
            CreateScriptableObject<BlocksAtlas>();
        }

        private static void CreateScriptableObject<T>() where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();

            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
                path = "Assets";
            else if (Path.GetExtension(path) != "")
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");

            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}