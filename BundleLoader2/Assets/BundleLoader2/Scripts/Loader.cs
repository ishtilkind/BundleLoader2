using NG.TRIPSS.CONFIG;
using NG.TRIPSS.CORE.LOG;

namespace NG.TRIPSS.CORE
{
    public class Loader: IAssetBundleLoader //, IAssetBundleLoaderInternal, IAssetBundleLoaderInternalAsync
    {
        #region Variables

        private IDataSource _dataSource;
        private IBundleHandler _bundleHandler;
        
        private AssetBundleItemList.DB_Type currentDbType = AssetBundleItemList.DB_Type.Unset;
        private int currentID = -1;

        #endregion

        #region Life Cycle

        Loader(IDataSource dataSource, IBundleHandler bundleHandler)
        {
            this._dataSource = dataSource;
            this._bundleHandler = bundleHandler;
            AssignListeners();
        }
        
        ~Loader()
        {
            RemoveListeners();
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
        
                private void RemoveListeners()
        {
//            if (listenersInitialized)
//            {
//                loadAssetBundleItemEvent.RemoveAllListeners();
//                _assetLoadedEvent.RemoveAllListeners();
//
//
//                createAssetBundleItemEvent.RemoveAllListeners();
//                _createAssetEvent.RemoveAllListeners();
//
//
//                sameAssetBundleItemEvent.RemoveAllListeners();
//                _sameAssetEvent.RemoveAllListeners();
//                sameAssetIdEvent.RemoveAllListeners();
//
//                _invalidBundleEvent.RemoveAllListeners();
//                _invalidAssetEvent.RemoveAllListeners();
//
//                _notInDbAssetEvent.RemoveAllListeners();
//
//                listenersInitialized = false;
//                "Listeners removed."
//                    .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
//            }
        }

        private void AssignListeners()
        {
//            if (!listenersInitialized)
//            {
//                loadAssetBundleItemEvent.AddListener(LoadBundleAssetEventHandler);
//                _assetLoadedEvent.AddListener(AssetLoadedEventHandler);
//
//                createAssetBundleItemEvent.AddListener(CreateBundleAssetEventHandler);
//                _createAssetEvent.AddListener(CreateAssetEventHandler);
//
//                sameAssetBundleItemEvent.AddListener(SameBundleAssetEventHandler);
//                _sameAssetEvent.AddListener(SameAssetEventHandler);
//
//                sameAssetIdEvent.AddListener(SameAssetIdEventHandler);
//
//                _invalidBundleEvent.AddListener(InvalidBundleEventHandler);
//                _invalidAssetEvent.AddListener(InvalidAssetEventHandler);
//
//                _notInDbAssetEvent.AddListener(NotInDbAssetEventHandler);
//
//                listenersInitialized = true;
//                "Listeners assigned.".ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
//
//            }
//            else
//            {
//                "Listeners ALREADY assigned.".ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
//            }

        }
        
        
        
        
        
        
        
        
//        --------------------------------
        private bool SameAsset(int id, AssetBundleItemList.DB_Type type)
        {
            $"currentID: {currentID} currentDbType: {currentDbType} new id: {id} new type: {type}"
                .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

            return (id == currentID && type == currentDbType);
        }
    }
}