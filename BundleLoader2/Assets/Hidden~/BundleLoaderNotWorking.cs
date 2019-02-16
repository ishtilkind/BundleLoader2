using JetBrains.Annotations;
using NG.TRIPSS.CAMERA;
using NG.TRIPSS.CONFIG;
using NG.TRIPSS.CORE.LOG;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using static UnityEngine.Debug;

// using UnityEditorInternal;

namespace NG.TRIPSS.CORE
{
    public class BundleLoaderNotWorking : MonoBehaviour
    {
        #region Singleton

        //private static BundleLoader instance;

        public static BundleLoaderNotWorking Instance { get ; private set; }

        #endregion

        #region VARIABLES

        private IAssetInfo _assetInfo;

        private FreeObjectRotationCam cam;

        const int INVALID_BUNDLE_ID = -10;
        const int INVALID_ASSET_ID = -20;
        const int NOT_IN_DB_ID = -30;

        [Header("Spawn Point")] public Transform spawn;

        [Header("Bundle Path")] public string bundlePath = "../AssetBundles/WebGL/";

        [Header("Data")] public AssetBundleItemList tripssAssets;

        [Header("Loaded Asset")]
        [SerializeField]
        private int currentID = -1;

        [SerializeField] private AssetBundleItemList.DB_Type currentDbType = AssetBundleItemList.DB_Type.Unset;

        [SerializeField] private AssetBundle currentBundle;
        [SerializeField] private GameObject currentAsset;

        public string CurrentBundleName => currentBundle?.name;

        [Header("Default Model Settings")] public AssetBundleItemList missingAssets;

        [SerializeField] private bool loadDefaultBundle = true;

        [Header("Communication Manager")]
        [SerializeField]
        private CommunicationManager commsManager;

        [Header("Wait time for a new click")] public float fireRate = 0.5F;
        private float nextFire = 0.0F;

        private bool collectGarbage = false;

        private bool resetNeeded = true;

        #endregion

        #region Events and EventHandlers

        [Serializable]
        public class LoadAssetBundleItemEvent : UnityEvent<AssetBundleItem>
        {
        }

        [Serializable]
        public class AssetLoadedEvent : UnityEvent<AssetBundleItem>
        {
        }

        [Serializable]
        public class CreateAssetBundleItemEvent : UnityEvent<AssetBundleItem>
        {
        }


        [Serializable]
        public class CreateAssetEvent : UnityEvent<AssetBundleItem>
        {
        }

        [Serializable]
        public class SameAssetBundleItemEvent : UnityEvent<AssetBundleItem>
        {
        }

        [Serializable]
        public class SameAssetEvent : UnityEvent<AssetBundleItem>
        {
        }

        [Serializable]
        public class SameAssetIdEvent : UnityEvent<int>
        {
        }

        [Serializable]
        public class InvalidBundleEvent : UnityEvent<AssetBundleItem>
        {
        }

        [Serializable]
        public class InvalidAssetEvent : UnityEvent<AssetBundleItem>
        {
        }

        [Serializable]
        public class NotInDbAssetEvent : UnityEvent<int>
        {
        }


        private LoadAssetBundleItemEvent loadAssetBundleItemEvent = new LoadAssetBundleItemEvent();
        private CreateAssetBundleItemEvent createAssetBundleItemEvent = new CreateAssetBundleItemEvent();
        private AssetLoadedEvent _assetLoadedEvent = new AssetLoadedEvent();
        private CreateAssetEvent _createAssetEvent = new CreateAssetEvent();

        private SameAssetEvent _sameAssetEvent = new SameAssetEvent();
        private SameAssetBundleItemEvent sameAssetBundleItemEvent = new SameAssetBundleItemEvent();
        private SameAssetIdEvent sameAssetIdEvent = new SameAssetIdEvent();

        private InvalidBundleEvent _invalidBundleEvent = new InvalidBundleEvent();
        private InvalidAssetEvent _invalidAssetEvent = new InvalidAssetEvent();
        private NotInDbAssetEvent _notInDbAssetEvent = new NotInDbAssetEvent();

        public static event Action<GameObject> OnAssetLoaded;

        private bool listenersInitialized = false;

        #endregion

        #region MonoBehaviour

        

        public void QuitApplication()
        {
#if UNITY_WEBGL
            WebGLInput.captureAllKeyboardInput = false;
#endif
            Reset();
            Application.Quit();
        }

        [UsedImplicitly]
        private void Awake()
        {
            // Singleton init
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
            // Singleton End

            StartCoroutine(LoadInternal());

            if (AppSettings.Instance != null)
            {
                "BundleLoader is Initialized"
                    .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
            }
        }

        [UsedImplicitly]
        private void OnEnable()
        {
            StartCoroutine(Init());
        }
        void OnApplicationQuit()
        {
#if UNITY_WEBGL
            WebGLInput.captureAllKeyboardInput = false;
#endif
            "OnApplicationQuit".ConsoleLog(LOG.LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel);
            Reset();
        }

        //This is still called in the editor
        [UsedImplicitly]
        private void OnDestroy()
        {
            //if (!quitting)
            //{
            //    instance = null;
            //    Init();
            //}
#if UNITY_EDITOR
            OnApplicationQuit();
#endif
        }        
        void Update()
        {

            if (Input.anyKey && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;

                if (Input.GetKeyDown(KeyCode.F8))
                {
                    Log("F8");
                    ReloadDataDB();
                    loadDefaultBundle = true;
                    return;
                }

                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    loadDefaultBundle = true;
                    Log("loadDefaultBundle is ON");
                    return;
                }

                //if (Input.GetKeyDown(KeyCode.Z))
                //{
                //    Debug.Log("KeyCode.Z - Quit");
                //    Application.Quit();
                //}         
                //else if (Input.GetKeyDown(KeyCode.X))
                //{
                //    Debug.Log("KeyCode.X - QuitApplication");
                //    QuitApplication();
                //}
                //else if (Input.GetKeyDown(KeyCode.C))
                //{
                //    Debug.Log("KeyCode.C - UnloadSceneAsync");
                //    StartCoroutine(UnloadYourAsyncScene());
                //}
                //else if (Input.GetKeyDown(KeyCode.V))
                //{
                //    Debug.Log("KeyCode.V - UnloadSceneAsync + QuitApplication");
                //    SceneManager.UnloadSceneAsync(0);
                //    QuitApplication();
                //}
                //else if (Input.GetKeyDown(KeyCode.B))
                //{
                //    Scene scene = SceneManager.GetActiveScene();

                //    Debug.Log("Active scene is '" + scene.name + "'. Build index is : " + scene.buildIndex);
                //}
                //else if (Input.GetKeyDown(KeyCode.G))
                //{
                //    collectGarbage = !collectGarbage;
                //}

                if (null == missingAssets)
                {
                    "Default Asset Database is missing"
                         .ConsoleLog(LOG.LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel);
                }
                else if (null == missingAssets.assetList && 0 == missingAssets.assetList.Count)
                {
                    "Default Asset Database is not loaded."
                        .ConsoleLog(LOG.LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel);
                }

                if (null == tripssAssets)
                {
                    "Asset Database is missing"
                        .ConsoleLog(LOG.LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel);
                }
                else if (null == tripssAssets.assetList && 0 == tripssAssets.assetList.Count)
                {
                    "Asset Database is not loaded."
                        .ConsoleLog(LOG.LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel);

                }
            }
        }
        #endregion
        
        private IEnumerator LoadInternal()
        {

            cam = Camera.main.GetComponent<FreeObjectRotationCam>();

            if( null == _assetInfo)
                _assetInfo = new AssetInfoFromDB();
 
            AssignListeners();

            yield break;
        }


        IEnumerator Init()
        {
#if UNITY_WEBGL
            WebGLInput.captureAllKeyboardInput = true;
#endif
            while (null == CommunicationManager.Instance)
            {
                yield return new WaitForEndOfFrame();
            }
            commsManager = CommunicationManager.Instance;

            while (null == AppSettings.Instance)
            {
                yield return new WaitForEndOfFrame();
            }

            $"UNITY_CUSTOM_ASSET_BUNDLES_PATH is  {commsManager.AssetBundlesPath}"
                          .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

            $"Streaming Assets PATH is  { Application.streamingAssetsPath}"
                .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

            Debug.Log($"Unity log level is set to {AppSettings.Instance.staticSettings.logLevel}");

            resetNeeded = true;
        }



        private void RemoveListeners()
        {
            if (listenersInitialized)
            {
                //if (AppSettings.Instance.staticSettings.useLocalDB)
                loadAssetBundleItemEvent.RemoveAllListeners();
                _assetLoadedEvent.RemoveAllListeners();


                createAssetBundleItemEvent.RemoveAllListeners();
                _createAssetEvent.RemoveAllListeners();


                sameAssetBundleItemEvent.RemoveAllListeners();
                _sameAssetEvent.RemoveAllListeners();
                sameAssetIdEvent.RemoveAllListeners();

                _invalidBundleEvent.RemoveAllListeners();
                _invalidAssetEvent.RemoveAllListeners();

                _notInDbAssetEvent.RemoveAllListeners();

                listenersInitialized = false;
                "Listeners removed."
                    .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
            }
        }

        private void AssignListeners()
        {
            if (!listenersInitialized)
            {
                //if (AppSettings.Instance.staticSettings.useLocalDB)
                loadAssetBundleItemEvent.AddListener(LoadBundleAssetEventHandler);
                _assetLoadedEvent.AddListener(AssetLoadedEventHandler);

                createAssetBundleItemEvent.AddListener(CreateBundleAssetEventHandler);
                _createAssetEvent.AddListener(CreateAssetEventHandler);

                sameAssetBundleItemEvent.AddListener(SameBundleAssetEventHandler);
                _sameAssetEvent.AddListener(SameAssetEventHandler);

                sameAssetIdEvent.AddListener(SameAssetIdEventHandler);

                _invalidBundleEvent.AddListener(InvalidBundleEventHandler);
                _invalidAssetEvent.AddListener(InvalidAssetEventHandler);

                _notInDbAssetEvent.AddListener(NotInDbAssetEventHandler);

                listenersInitialized = true;
                "Listeners assigned.".ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

            }
            else
            {
                "Listeners ALREADY assigned.".ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
            }

        }


        private void Reset()
        {
            if (resetNeeded)
            {
                "Reset".ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

                resetNeeded = false;
                StopAllCoroutines();
                UnloadCurrentBundle();
                AssetBundle.UnloadAllAssetBundles(true);
                RemoveListeners();
                DeleteAllObjects();
            }
        }

        private string GetUri(string assetBundleName)
        {
            if (null == commsManager)
            {
                commsManager = CommunicationManager.Instance;
            }

            if (string.IsNullOrEmpty(assetBundleName))
            {
                "assetBundleName is not set in GetUri.".ConsoleLog(LOG.LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel); ;
                return null;
            }

            // Absent DB Types are for simple missing assets located in the StreamingAssets folder
            var uri = System.IO.Path.Combine(
                this.currentDbType == AssetBundleItemList.DB_Type.Absent
                    ? Application.streamingAssetsPath
                    : commsManager.AssetBundlesPath.ValidatePath(),
                assetBundleName);

            $"URI is set to: {uri}".ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

            return uri;
        }

        #region Interface

        public void LoadModel(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                "Model ID is not set.".ConsoleLog(LOG.LogLevel.Error, AppSettings.Instance.staticSettings.logLevel);
                return;
            }

            int id = -1;

            if (!Int32.TryParse(str, out id))
            {
                $"Model ID is not numeric: {str}".ConsoleLog(LOG.LogLevel.Error, AppSettings.Instance.staticSettings.logLevel);
                return;
            }

            // Check if listeners are set
            if (!listenersInitialized)
            {
                AssignListeners();
            }

            LoadTripssAsset(id);
        }

        public void LoadTripssAsset(int id)
        {
            AssetBundleItem item = null;
            if (AppSettings.Instance.staticSettings.useLocalDB)
            {
                item = _assetInfo.GetAssetInfo(id);
            }
            else
            {
                var val = commsManager.GetAssetInfo(id);
                if (val.IsEmptyObject())
                {
                    // Item not found in DB
                    $"Asset ID {id} NOT FOUND in DB."
                        .ConsoleLog(LOG.LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel);

                    // Unload Current Model!!!
                    // TODO: Need to check if ResetCurrentAsset() call is enough
                    if (this.currentDbType != AssetBundleItemList.DB_Type.Absent)
                    {
                        UnloadCurrentBundle();
                    }

                    // Instantiate CUBE!
                    _notInDbAssetEvent.Invoke(id);
                    return;
                }
                item = JsonUtility.FromJson<AssetBundleItem>(val);
            }

            if (item == null)
            {
                _notInDbAssetEvent.Invoke(id);
                return;
            }

            LoadAsset(item);


            /*
                        Debug.LogWarning("Trying to load TRIPSS ID: " + id);
                        if (id < 0)
                        {
                            Debug.LogError("Invalid Asset ID.");
                            return;
                        }

                        var val = commsManager.GetAssetInfo(id);

                        if ( val.IsEmptyObject() )
                        {
                            // Item not found in DB
                            Debug.LogWarning($"Asset ID {id} NOT FOUND in DB.");
                            // Instantiate CUBE!
                            _notInDbAssetEvent.Invoke(id);
                            return;
                        }

                        Debug.LogWarning($"LOOK=> Calling commsManager.GetAssetInfo with id: {id}. Return: {val}");

                        var item = JsonUtility.FromJson<AssetBundleItem>(val);
                        Debug.LogWarning(item.ToDetailedString());

                        if (AppSettings.Instance.staticSettings.useLocalDB)
                        {
                            Debug.Log("Using Local DB");
                            LoadAsset(id, tripssAssets);
                        }
                        else
                        {
                            Debug.Log("Using Remote DB");
                            LoadAsset(item);
                        }
             */

        }

        private void LoadAsset(AssetBundleItem item)
        {
            this.currentDbType = AssetBundleItemList.DB_Type.Production;
            if (SameAsset(item.ContainerID, this.currentDbType))
            {
                "Asset is already loaded!"
                    .ConsoleLog(LOG.LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel);
                return;
            }


            loadAssetBundleItemEvent.Invoke(item);
        }

        #endregion

        #region Implementation Async

        IEnumerator LoadBundleAsync(AssetBundleItem asset)
        {
            if (currentBundle && asset.BundleName == currentBundle.name)
            {
                $"Bundle is already loaded: {currentBundle.name} "
                    .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

                // return
                yield break;
            }

#if DEBUG
            var all = AssetBundle.GetAllLoadedAssetBundles();

            $"GetAllLoadedAssetBundles returned {all.Count()}  bundles"
                .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

            if (all.Any())
            {
                foreach (var current in all)
                {
                    $"Current Bundle is: {current.name}"
                         .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
                }
            }
#endif

            var url = GetUri(asset.BundleName);

            if (string.IsNullOrEmpty(url))
            {
                "Invalid URL".ConsoleLog(LOG.LogLevel.Error, AppSettings.Instance.staticSettings.logLevel);

                //  NEED TO HANDLE IT BETTER. INSTANTIATE ERROR CUBE.

                yield break;
            }

            $"URL: {url}"
                .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

            var request = UnityWebRequestAssetBundle.GetAssetBundle(url, 0);
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                request.error.ConsoleLog(LOG.LogLevel.Error, AppSettings.Instance.staticSettings.logLevel);
                _invalidBundleEvent.Invoke(asset);
                if (asset.ContainerID < -1)
                {
                    loadDefaultBundle = false;
                }

                // return
                yield break;
            }

            "request.SendWebRequest has no errors"
                .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);


            var loadedBundle = DownloadHandlerAssetBundle.GetContent(request);


            if (null == loadedBundle)
            {
                $"Invalid bundle {asset.BundleName}".ConsoleLog(LOG.LogLevel.Error, AppSettings.Instance.staticSettings.logLevel);

                if (asset.ContainerID < -1)
                {
                    $"Invalid currentBundle. Unable to load: {asset.BundleName}"
                        .ConsoleLog(LOG.LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel);

                    loadDefaultBundle = false;
                    "loadDefaultBundle set to FALSE."
                        .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
                }

                _invalidBundleEvent.Invoke(asset);
                yield break;
            }

            currentBundle = loadedBundle;
            _createAssetEvent.Invoke(asset);

        }

        //IEnumerator LoadBundleAsync(BundleAsset asset)
        //{
        //    Debug.Log("LoadBundleAsync for: " + asset.ToJson());
        //    if (currentBundle && asset.bundleName == currentBundle.name)
        //    {
        //        Debug.Log("Bundle is already loaded: " + currentBundle.name);
        //        // return
        //        yield break;
        //    }

        //    var all = AssetBundle.GetAllLoadedAssetBundles();

        //    Debug.LogWarning("GetAllLoadedAssetBundles returned " + all.Count().ToString() + " bundles");
        //    foreach (var current in all)
        //    {
        //        Debug.LogWarning("Current Bundle is: " + current.name);
        //    }

        //    var url = GetUri(asset.bundleName);
        //    Debug.LogWarning("URL: " + url);

        //    var request = UnityWebRequestAssetBundle.GetAssetBundle(url, 0);
        //    yield return request.SendWebRequest();

        //    if (request.isNetworkError || request.isHttpError)
        //    {
        //        Debug.LogError(request.error);
        //        invalidBundleEvent.Invoke(asset);
        //        if (asset.containerID < -1)
        //        {
        //            loadDefaultBundle = false;
        //        }

        //        // return
        //        yield break;
        //    }

        //    try
        //    {
        //        Debug.Log("Before DownloadHandlerAssetBundle");

        //        var loadedBundle = DownloadHandlerAssetBundle.GetContent(request);

        //        if (null == loadedBundle)
        //        {
        //            Debug.LogError("Invalid bundle " + asset.bundleName);

        //            if (asset.containerID < -1)
        //            {
        //                Debug.LogWarning("Invalid currentBundle. Unable to load: " + asset.bundleName);
        //                loadDefaultBundle = false;
        //                Debug.Log("loadDefaultBundle set to FALSE.");
        //            }

        //            invalidBundleEvent.Invoke(asset);
        //            yield break;
        //        }

        //        currentBundle = loadedBundle;
        //        createAssetEvent.Invoke(asset);
        //    }
        //    catch (Exception exception)
        //    {
        //        Debug.LogError($"DownloadHandlerAssetBundle.GetContent FAILED with error: {exception.Message}");
        //    }
        //}

        //IEnumerator LoadAssetAsync(BundleAsset asset)
        //{
        //    if (null == asset || string.IsNullOrEmpty(asset.assetName))
        //    {
        //        Debug.LogError("Invalid asset name");
        //        yield break;
        //    }

        //    if (null == currentBundle)
        //    {
        //        Debug.LogError("Invalid currentBundle. Unable to load: " + asset.assetName);
        //        yield break;
        //    }

        //    // Load Prefab by name if set.
        //    string prefabToLoad = string.IsNullOrEmpty(asset.prefabName) ? asset.assetName : asset.prefabName;

        //    GameObject[] gos = currentBundle.LoadAssetWithSubAssets<GameObject>(prefabToLoad);
        //    if (null == gos || gos.Length == 0)
        //    {
        //        Debug.LogWarning("Nothing to instantiate for: " + prefabToLoad);
        //        this.invalidAssetEvent.Invoke(asset);
        //        //invalidBundleEvent.Invoke(asset);
        //        yield break;
        //    }

        //    foreach (var go in gos)
        //    {
        //        var goInstance = Instantiate(go, spawn);
        //        goInstance.name = go.name;
        //        goInstance.transform.localPosition = new Vector3(0, 0, 0);
        //        goInstance.transform.localRotation = Quaternion.identity;
        //        assetLoadedEvent.Invoke(asset);

        //        //Debug.Log("Calling OnAssetLoaded for: " + goInstance + " in: " + name);
        //        OnAssetLoaded(goInstance);
        //        //Debug.Log("Object instantiated: " + goInstance.name);
        //    }
        //}

        IEnumerator LoadAssetAsync(AssetBundleItem asset)
        {
            if (null == asset || string.IsNullOrEmpty(asset.AssetName))
            {
                "Invalid asset name".ConsoleLog(LOG.LogLevel.Error, AppSettings.Instance.staticSettings.logLevel);
                yield break;
            }

            if (null == currentBundle)
            {
                $"Invalid currentBundle. Unable to load: {asset.AssetName}".ConsoleLog(LOG.LogLevel.Error, AppSettings.Instance.staticSettings.logLevel);
                yield break;
            }

            // Load Prefab by name if set.
            string prefabToLoad = string.IsNullOrEmpty(asset.PrefabName) ? asset.AssetName : asset.PrefabName;

            GameObject[] gos = currentBundle.LoadAssetWithSubAssets<GameObject>(prefabToLoad);
            if (null == gos || gos.Length == 0)
            {
                $"Nothing to instantiate for: {prefabToLoad}"
                    .ConsoleLog(LOG.LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel);

                _invalidBundleEvent.Invoke(asset);
                yield break;
            }

            foreach (var go in gos)
            {
                var goInstance = Instantiate(go, spawn);
                goInstance.name = go.name;
                goInstance.transform.localPosition = new Vector3(0, 0, 0);

                goInstance.transform.localRotation = Quaternion.Euler(new Vector3(asset.InitialRotationX, asset.InitialRotationY, asset.InitialRotationZ));
                goInstance.transform.localScale = Vector3.one * asset.InitialScale;

                if (cam)
                {
                    cam.loadedAsset = GetComponent<Transform>();
                    cam.ResetCamera();
                }

                _assetLoadedEvent.Invoke(asset);

                OnAssetLoaded(goInstance);
            }
        }

        #endregion

        #region Implementation Sync

        //private bool SameAsset(int id, BundleAssetList.DB_Type type)
        //{
        //    Debug.Log("currentID: " + currentID.ToString() + " currentDbType: " + currentDbType + " new id: " +
        //              id.ToString() + " new type: " + type);
        //    return (id == currentID && type == currentDbType);
        //}


        private bool SameAsset(int id, AssetBundleItemList.DB_Type type)
        {
            $"currentID: {currentID} currentDbType: {currentDbType} new id: {id} new type: {type}"
                .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

            return (id == currentID && type == currentDbType);
        }

        private void LoadAsset(int id, AssetBundleItemList assets)
        {
            $"LoadAsset with id: {id} from {assets.type}"
                .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

            bool bundleUnloaded = false;

            // Check if the same bundle type
            if (assets.type != this.currentDbType)
            {
                UnloadCurrentBundle();
                bundleUnloaded = true;
            }

            if (!assets || 0 == assets.assetList.Count)
            {
                "Asset DB is not initialized".ConsoleLog(LOG.LogLevel.Error, AppSettings.Instance.staticSettings.logLevel);
                return;
            }

            if (!bundleUnloaded && SameAsset(id, assets.type))
            {
                "Asset is already loaded!"
                     .ConsoleLog(LOG.LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel);

                return;
            }

            var assetBundleItems = assets.assetList.Where(item => id == item.ContainerID).ToList();
            if (!assetBundleItems.Any())
            {
                _notInDbAssetEvent.Invoke(id);
            }
            else
            {
                currentDbType = assets.type;
                loadAssetBundleItemEvent.Invoke(assetBundleItems.First());
                if (assetBundleItems.Count() > 1)
                {
                    $"Found {assetBundleItems.Count()} items with ContainerID {id}!"
                        .ConsoleLog(LOG.LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel);

                }
            }
        }

        //private void LoadAsset(int id, BundleAssetList assets)
        //{
        //    Debug.Log($"LoadAsset with id: {id} from {assets.type}");

        //    if (!assets || 0 == assets.assetList.Count)
        //    {
        //        Debug.LogError("Asset DB is not initialized");
        //        return;
        //    }

        //    if (SameAsset(id, assets.type))
        //    {
        //        Debug.LogWarning("Asset is already loaded!");
        //        return;
        //    }

        //    foreach (var item in assets.assetList)
        //    {
        //        if (id == item.containerID)
        //        {
        //            currentDbType = assets.type;
        //            loadBundleAssetEvent.Invoke(item);
        //            return;
        //        }
        //    }

        //    notInDbAssetEvent.Invoke(id, assets);
        //}

        private void LoadDefaultAsset(int id)
        {
            if (id >= 0)
            {
                "Invalid Default Asset ID.".ConsoleLog(LOG.LogLevel.Error, AppSettings.Instance.staticSettings.logLevel);
                return;
            }

            if (!loadDefaultBundle)
            {
                "Load Default Asset Flag is set to FALSE.".ConsoleLog(LOG.LogLevel.Error, AppSettings.Instance.staticSettings.logLevel);
                return;
            }

            if (missingAssets == null || missingAssets.assetList.Count == 0)
            {
                "Missing Assets DB is not initialized.".ConsoleLog(LOG.LogLevel.Error, AppSettings.Instance.staticSettings.logLevel);
                return;
            }

            if (this.currentDbType == AssetBundleItemList.DB_Type.Absent && id == currentID)
            {
                // Need to retest this!!!
                var asset = missingAssets.assetList.Where(item => item.ContainerID == id).ToList().FirstOrDefault();
                this._sameAssetEvent.Invoke(asset);
            }
            else
            {
                LoadAsset(id, missingAssets);
            }
        }

        private void UnloadCurrentBundle()
        {
            $"UnloadCurrentBundle: {(currentBundle ? currentBundle.name : "none")}"
                .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

            ResetCurrentAsset();
            if (currentBundle)
            {
                currentBundle.Unload(true);
                currentBundle = null;
            }

            if (collectGarbage)
            {
                CollectGarbage();
            }
        }

        private void CollectGarbage()
        {
            Console.WriteLine("Memory used before collection:       {0:N0}", GC.GetTotalMemory(false));
            // Collect all generations of memory.
            GC.Collect();
            Console.WriteLine("Memory used after full collection:   {0:N0}", GC.GetTotalMemory(true));

        }

        private void ResetCurrentAsset()
        {
            var ca = currentAsset ? currentAsset.name : "none";
            $"ResetCurrentAsset: {ca}"
                .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

            currentID = -1;
            if (spawn) spawn.DeleteChildren();
            if (currentAsset)
            {
                Destroy(currentAsset);
                currentAsset = null;
            }
        }

        #endregion

        #region HANDLERS

        //private void CreateAssetEventHandler(BundleAsset asset)
        //{
        //    Debug.Log("CreateAssetEventHandler for " + asset.assetName);

        //    if (currentAsset && currentAsset.name == asset.assetName)
        //    {
        //        sameAssetEvent.Invoke(asset);
        //        return;
        //    }

        //    // Make sure to destroy previous object
        //    ResetCurrentAsset();

        //    StartCoroutine(LoadAssetAsync(asset));
        //}

        //private void InvalidBundleEventHandler(BundleAsset asset)
        //{
        //    Debug.Log("InvalidBundleEventHandler for " + asset.ToJson());

        //    LoadDefaultAsset(INVALID_BUNDLE_ID);
        //}

        //private void InvalidAssetEventHandler(BundleAsset asset)
        //{
        //    Debug.Log("InvalidAssetEventHandler for " + asset.ToJson());
        //    LoadDefaultAsset(INVALID_ASSET_ID);
        //}

        //private void NotInDbAssetEventHandler(int id, BundleAssetList assets)
        //{
        //    Debug.Log("NotInDbAssetEventHandler for ID: " + id);
        //    if (BundleAssetList.DB_Type.Absent != assets.type)
        //    {
        //        LoadDefaultAsset(NOT_IN_DB_ID);
        //    }
        //    else
        //    {
        //        Debug.Log("Not IMPLEMENTED. Probably do nothing since no default asset is found.");
        //    }
        //}

        //private void SameAssetEventHandler(BundleAsset asset)
        //{
        //    Debug.Log("SameAssetEventHandler for " + asset.ToJson());
        //}

        private void SameAssetIdEventHandler(int id)
        {
            $"SameAssetIdEventHandler for {id}"
                .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
        }

        //private void LoadBundleAssetEventHandler(BundleAsset asset)
        //{
        //    Debug.Log("LoadBundleAssetEventHandler for " + asset.ToJson());

        //    if (null == currentBundle || currentBundle.name != asset.bundleName)
        //    {
        //        Debug.Log($"Need to Reload Bundle {currentBundle?.name} with {asset.bundleName}");
        //        UnloadCurrentBundle();
        //        StartCoroutine(LoadBundleAsync(asset));
        //    }
        //    else
        //    {
        //        Debug.Log($"Need to Create {asset.name} from {asset.bundleName} Bundle");
        //        createAssetEvent.Invoke(asset);
        //    }
        //}

        // --------------------------------------------------------------------------------------
        private void SameBundleAssetEventHandler(AssetBundleItem asset)
        {
            Log($"SameBundleAssetEventHandler for {asset?.ToJson()}");
        }

        private void CreateBundleAssetEventHandler(AssetBundleItem asset)
        {
            Log("CreateBundleAssetEventHandler for " + asset.AssetName);

            if (currentAsset && currentAsset.name == asset.AssetName)
            {
                sameAssetBundleItemEvent.Invoke(asset);
                return;
            }

            // Make sure to destroy previous object
            ResetCurrentAsset();

            StartCoroutine(LoadAssetAsync(asset));
        }


        private void LoadAssetBundleItemEventHandler(AssetBundleItem asset)
        {
            Log("LoadAssetBundleItemEventHandler for " + asset.ToJson());

            if (null == currentBundle || currentBundle.name != asset.BundleName)
            {
                UnloadCurrentBundle();
                StartCoroutine(LoadBundleAsync(asset));
            }
            else
            {
                // createAssetEvent.Invoke(asset);
                createAssetBundleItemEvent.Invoke(asset);
            }
        }


        private void InvalidBundleEventHandler(AssetBundleItem asset)
        {
            Log("InvalidBundleEventHandler for " + asset.ToJson());
            //currentDbType = BundleAssetList.DB_Type.Absent;
            LoadDefaultAsset(INVALID_BUNDLE_ID);
        }


        private void NotInDbAssetEventHandler(int id)
        {
            $"NotInDbAssetEventHandler for ID: {id}"
                .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

            //currentDbType = BundleAssetList.DB_Type.Absent;
            LoadDefaultAsset(NOT_IN_DB_ID);
        }

        private void InvalidAssetEventHandler(AssetBundleItem asset)
        {
            Log("InvalidAssetEventHandler for " + asset.ToJson());
            LoadDefaultAsset(INVALID_ASSET_ID);
        }


        private void AssetLoadedEventHandler(AssetBundleItem asset)
        {
            //$"Asset Loaded Successfully: { asset.ToJson()}"
            //    .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
        }

        private void LoadBundleAssetEventHandler(AssetBundleItem asset)
        {
            // Debug.Log("LoadBundleAssetEventHandler for " + asset.ToJson());

            if (null == currentBundle || currentBundle.name != asset.BundleName)
            {
                UnloadCurrentBundle();
                StartCoroutine(LoadBundleAsync(asset));
            }
            else
            {
                _createAssetEvent.Invoke(asset);
            }
        }

        private void CreateAssetEventHandler(AssetBundleItem asset)
        {
            $"CreateAssetEventHandler for {asset.AssetName}"
                .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

            if (currentAsset && currentAsset.name == asset.AssetName)
            {
                _sameAssetEvent.Invoke(asset);
                return;
            }

            // Make sure to destroy previous object
            ResetCurrentAsset();

            StartCoroutine(LoadAssetAsync(asset));
        }

        private void SameAssetEventHandler(AssetBundleItem asset)
        {
            Log("SameAssetEventHandler for " + asset.ToJson());
        }

        #endregion

        public int GetIdByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                "GetIdByName: NAME IS NOT SPECIFIED."
                    .ConsoleLog(LOG.LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel);

                return -1;
            }

            if (AppSettings.Instance.staticSettings.useLocalDB)
            {
                var val = tripssAssets.assetList
                    .FirstOrDefault(item => item.AssetName == name);

                return val?.ContainerID ?? -1;
            }
            else
            {

            }

            return -1;
        }

        public int GetContainerIdByAssetNameAndBundleName(string aname, string bname = null)
        {
            // Debug.Log("GetContainerIdByAssetNameAndBundleName: ASSET " + aname);

            if (string.IsNullOrEmpty(aname))
            {
                "GetContainerIdByAssetNameAndBundleName: ASSET NAME IS NOT SPECIFIED."
                    .ConsoleLog(LOG.LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel);
                return -1;
            }

            if (string.IsNullOrEmpty(bname))
            {
                if (null == currentBundle || string.IsNullOrEmpty(currentBundle.name))
                {
                    "GetContainerIdByAssetNameAndBundleName: BUNDLE NAME IS NOT SPECIFIED."
                        .ConsoleLog(LOG.LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel);

                    return -1;
                }
                bname = currentBundle.name;
            }

            // Debug.Log("GetContainerIdByAssetNameAndBundleName: BUNDLE " + bname);

            if (AppSettings.Instance.staticSettings.useLocalDB)
            {
                var val = tripssAssets.assetList
                    .FirstOrDefault(item => item.AssetName == aname && item.BundleName == bname);

                return val?.ContainerID ?? -1;
            }
            else
            {
                var val = this.commsManager.FindAssetInfoByAssetNameAndBundleName(aname, bname);
                if (val.IsEmptyObject())
                {
                    return -1;
                }

                var item = JsonUtility.FromJson<AssetBundleItem>(val);
                return item?.ContainerID ?? -1;
            }

            // return -1;
        }

        private void ReloadDataDB()
        {
            /*
            const string data_bundle = "tripss_model_data.assetbundle";
            const string data_main = "allModels_master";
            const string data_missing = "ErrorAssets_master";
             * */

            //BundleAsset asset = new BundleAsset();
            //asset.bundleName = data_bundle;
            //asset.assetName = data_main;
            //StartCoroutine ( LoadAssetAsync(asset) );
        }

        IEnumerator UnloadYourAsyncScene()
        {
            AsyncOperation asyncUnoad = SceneManager.UnloadSceneAsync(0);

            while (!asyncUnoad.isDone)
            {
                Log("Unloading");
                yield return null;
            }

            Log("asyncUnload.isDone");
        }

        private void DeleteAllObjects()
        {
            GameObject[] allObjects = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
            foreach (GameObject go in allObjects)
            {
                //Debug.Log("Deleting " + go.name);
                GameObject.Destroy(go);
            }

            Log("All Objects Deleted");
        }

    }
}
