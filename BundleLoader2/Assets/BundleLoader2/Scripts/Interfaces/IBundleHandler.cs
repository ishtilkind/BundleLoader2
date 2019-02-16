using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NG.TRIPSS.CORE
{
    public interface IBundleHandler
    {
        void UnloadCurrentBundle();
        void DeleteAllGameObjects();
        void ResetCurrentAsset();

        IEnumerator LoadAssetAsync(AssetBundleItem asset);
        void LoadAsset(AssetBundleItem asset);

        void LoadAssetBundleItem(AssetBundleItem asset);
    }
}