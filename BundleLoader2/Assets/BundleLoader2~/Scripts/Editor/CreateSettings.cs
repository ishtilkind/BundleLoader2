using UnityEditor;
using UnityEngine;

namespace NG.TRIPSS.CONFIG
{
    public class CreateSettings
    {
        [MenuItem("Assets/Create/Settings")]
        public static Settings Create()
        {
            Settings asset = ScriptableObject.CreateInstance<Settings>();

            AssetDatabase.CreateAsset(asset, "Assets/Data/Settings.asset");
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}