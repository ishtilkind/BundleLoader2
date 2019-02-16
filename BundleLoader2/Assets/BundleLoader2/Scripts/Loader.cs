using System;
using NG.TRIPSS.CONFIG;
using NG.TRIPSS.CORE.LOG;
using UnityEngine;
using UnityEngine.Events;

namespace NG.TRIPSS.CORE
{
    public class Loader: IAssetBundleLoader //, IAssetBundleLoaderInternal, IAssetBundleLoaderInternalAsync
    {
        #region Variables

        private IDataSource _dataSource;
        private IBundleHandler _bundleHandler;
        
        private string bundlePath = "../AssetBundles/WebGL/";
        
        private AssetBundleItemList.DB_Type currentDbType = AssetBundleItemList.DB_Type.Unset;
        private int currentID = -1;
        
        [SerializeField] private AssetBundle currentBundle;
        [SerializeField] private GameObject currentAsset;

        #endregion



        #region Life Cycle
        
        public Loader(string bundlePath, IDataSource dataSource, IBundleHandler bundleHandler)
        {
            this.bundlePath = bundlePath;
            this._dataSource = dataSource;
            this._bundleHandler = bundleHandler;
        }

        #endregion
        
    
        
        #region IAssetBundleLoader
        
        /// <summary>
        /// Entry Point.
        /// </summary>
        /// <param name="containerId"></param>
        public void LoadModel(string containerId)
        {
            if (string.IsNullOrEmpty(containerId))
            {
                "Model ID is not set.".ConsoleLog(LOG.LogLevel.Error, AppSettings.Instance.staticSettings.logLevel);
                return;
            }
            
            var id = -1;

            if (!int.TryParse(containerId, out id))
            {
                $"Model ID is not numeric: {containerId}".ConsoleLog(LOG.LogLevel.Error, AppSettings.Instance.staticSettings.logLevel);
                return;
            }

            LoadTripssAsset(id, _dataSource);
        }

        public void LoadAsset(AssetBundleItem item)
        {
            this.currentDbType = AssetBundleItemList.DB_Type.Production;
            if (SameAsset(item.ContainerID, this.currentDbType))
            {
                "Asset is already loaded!"
                    .ConsoleLog(LOG.LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel);
                return;
            }

            // loadAssetBundleItemEvent.Invoke(item);
        }

        public void LoadTripssAsset(int id, IDataSource dataSource)
        {
            throw new System.NotImplementedException();
        }

        public int GetIdByName(string name)
        {
            throw new System.NotImplementedException();
        }

        public int GetContainerIdByAssetNameAndBundleName(string aname, string bname = null)
        {
            throw new System.NotImplementedException();
        }
        
        #endregion
        

        
        
        
        
        
        
        
        
//        --------------------------------
        public bool SameAsset(int id, AssetBundleItemList.DB_Type type)
        {
            $"currentID: {currentID} currentDbType: {currentDbType} new id: {id} new type: {type}"
                .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

            return (id == currentID && type == currentDbType);
        }
        
        private void UnloadCurrentBundle()
        {
            $"UnloadCurrentBundle: {(currentBundle ? currentBundle.name : "none")}"
                .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

            _bundleHandler.ResetCurrentAsset();
            if (currentBundle)
            {
                currentBundle.Unload(true);
                currentBundle = null;
            }
        }

    }

    public static class BundleLoaderUtilities
    {

    }
}