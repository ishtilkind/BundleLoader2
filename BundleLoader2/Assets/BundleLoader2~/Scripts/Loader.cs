using System;
using System.Runtime.CompilerServices;
using NG.TRIPSS.CONFIG;
using NG.TRIPSS.CORE.LOG;
using UnityEngine;
using UnityEngine.Events;

namespace NG.TRIPSS.CORE
{
    public sealed class Loader: IAssetBundleLoader //, IAssetBundleLoaderInternal, IAssetBundleLoaderInternalAsync
    {
//        #region Singleton
//
//        private static IAssetBundleLoader instance = null;
//        private static readonly object padlock = new object();
//
//        public static IAssetBundleLoader Instance
//        {
//            get
//            {
//                lock (padlock)
//                {
//                    if (instance == null)
//                    {
//                        instance = new Loader();
//                    }
//                    return instance;
//                }
//            }
//        }
//
//        #endregion

        
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
        public bool LoadModel(string containerId)
        {
            if (string.IsNullOrEmpty(containerId))
            {
                "Model ID is not set.".ConsoleLog(LOG.LogLevel.Error, AppSettings.Instance.staticSettings.logLevel);
                return false;
            }
            
            var id = -1;

            if (!int.TryParse(containerId, out id))
            {
                $"Model ID is not numeric: {containerId}".ConsoleLog(LOG.LogLevel.Error, AppSettings.Instance.staticSettings.logLevel);
                return false;
            }

            LoadTripssAsset(id, _dataSource);
            return true;
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
            
            _bundleHandler.LoadAssetBundleItem(item);
        }

        public bool LoadTripssAsset(int id, IDataSource dataSource)
        {
            var item = dataSource.TryItemLookup(id);
            if (null == item) return false;
            
            LoadAsset(item);

            return true;
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
        public static string ValidatePath(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                "Empty Path"
                    .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

                return "/";
            }

            if (path.EndsWith("/"))
                return path;
            else
                return (path + "/");
        }

        public static string ToJson<T>(this T val)
        {
            return JsonUtility.ToJson(val);
        }

        public static bool IsEmptyObject(this string val)
        {
            var result = string.IsNullOrEmpty(val) || val == "{}";

            $"IsEmptyObject called with value: '{val}' is {result}".ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

            return result;
        }
    }
}