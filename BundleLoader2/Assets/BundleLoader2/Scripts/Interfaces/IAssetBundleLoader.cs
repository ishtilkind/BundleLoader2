using System;
using System.Collections;

namespace NG.TRIPSS.CORE
{
    interface IAssetBundleLoader
    {
        void LoadModel(string containerId);

        void LoadAsset(AssetBundleItem item);
        void LoadTripssAsset(int id, IDataSource dataSource);
        int GetIdByName(string name);
        int GetContainerIdByAssetNameAndBundleName(string aname, string bname = null);
    }

    interface IAssetBundleLoaderInternal
    {
        void Reset();
        void RemoveListeners();
        void AssignListeners();

        string GetUri(string assetBundleName);
        void LoadAsset(AssetBundleItem item);

        void ReloadDataDB();
    }

    interface IAssetBundleLoaderInternalAsync
    {
        // Async
        IEnumerator LoadInternal();
        IEnumerator Init();
        IEnumerator LoadBundleAsync(AssetBundleItem asset);
        IEnumerator LoadAssetAsync(AssetBundleItem asset);
        IEnumerator UnloadYourAsyncScene();
    }
}
