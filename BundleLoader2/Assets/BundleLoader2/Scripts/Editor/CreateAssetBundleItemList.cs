using UnityEditor;
using UnityEngine;

namespace NG.TRIPSS.CORE
{
    public class CreateAssetBundleItemList
    {
        [MenuItem("Assets/Create/Asset Bundle Item List")]
        public static AssetBundleItemList Create()
        {
            AssetBundleItemList asset = ScriptableObject.CreateInstance<AssetBundleItemList>();

            AssetDatabase.CreateAsset(asset, "Assets/AssetBundleItemList.asset");
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}
