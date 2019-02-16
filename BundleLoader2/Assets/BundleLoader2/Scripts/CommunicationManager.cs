using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using NG.TRIPSS.CONFIG;
using NG.TRIPSS.CORE.LOG;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NG.TRIPSS.CORE
{
    public class CommunicationManager : MonoBehaviour, IDataSource
    //, ICommsToUnity, ICommsFromUnity
    {

        #region UNITY 2 WEB PAGE COMMUNICATION

        [DllImport("__Internal")]
        private static extern string GetAssetBundleDataAsString();

        [DllImport("__Internal")]
        private static extern string GetAssetInfoById(int id);

        [DllImport("__Internal")]
        private static extern string GetAssetInfoByAssetNameAndBundleName(string aname, string bname);

        [DllImport("__Internal")]
        private static extern string GetAssetsByAssetNameAndBundleName(string aname, string bname);

        [DllImport("__Internal")]
        private static extern string ProjectAssetBundlesPath();

        [DllImport("__Internal")]
        private static extern void UnityReceiverEnabled(string str);

        [DllImport("__Internal")]
        private static extern void UnityReceiverDisabled(string str);

        [DllImport("__Internal")]
        private static extern void ModelComponentClickedOnce(string jsonDetails);

        [DllImport("__Internal")]
        private static extern void ModelComponentClickedTwice(string jsonDetails);

        [DllImport("__Internal")]
        private static extern void MouseEnter(string componentID);

        [DllImport("__Internal")]
        private static extern void MouseLeave();

        [DllImport("__Internal")]
        private static extern string DataRequest(string jsonDetails);

        #endregion

        #region Singleton

        bool quitting = false;

        private static CommunicationManager instance;

        public static CommunicationManager Instance
        {
            get
            {
                Init();
                return instance;
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            if (instance == null || instance.Equals(null))
            {
                // Debug.LogWarning("Creating and Eager Initializing CommunicationManager");

                var gameObject = new GameObject("CommunicationManager");
                // gameObject.hideFlags = HideFlags.HideAndDontSave;

                instance = gameObject.AddComponent<CommunicationManager>();
            }
        }

        #endregion

        // [SerializeField]
        //private Loader loader;

        #region MonoBehaviour

        private void OnApplicationQuit()
        {
            quitting = true;
        }

        private void OnDestroy()
        {
            if (!quitting)
            {
                instance = null;
                Init();
            }
        }

        public void Awake()
        {
            StartCoroutine(LoadInternal());
            //// Singleton init
            //if (Instance == null)
            //{
            //    Instance = this;
            //}
            //else if (Instance != this)
            //{
            //    Destroy(gameObject);
            //}

            //DontDestroyOnLoad(gameObject);
            //// Singleton End
        }

        private IEnumerator LoadInternal()
        {
            //var path = Application.streamingAssetsPath + "/planets.json";
            //using (var www = new WWW(path))
            //{
            //    yield return new WaitForSeconds(5); // Pretend the network is slow
            //    yield return www;
            //    planets = JsonUtility.FromJson<Planets>(www.text).planets;
            //}
            // loader = BundleLoader.Instance;
            
            Debug.Log("Loading Internals...");

            yield break;
        }

        void OnDisable()
        {
            try
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
#if UNITY_WEBGL
                UnityReceiverDisabled(name);
#endif
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            Application.targetFrameRate = 1;
        }

        void OnEnable()
        {
            if (Application.targetFrameRate != -1)
            {
                var prev = Application.targetFrameRate;
                Application.targetFrameRate = -1;
                if (null != AppSettings.Instance)
                {
                    $"CommunicationManager::OnEnable Application.targetFrameRate reseted from: {prev}  to: {Application.targetFrameRate}"
                        .ConsoleLog(LOG.LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel);
                }
            }

            if (null != AppSettings.Instance)
            {
                $"UnityReceiverEnabled for {name}"
                    .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        #endregion

        #region ICommsToUnity

        public string AssetBundlesPath => ProjectAssetBundlesPath();

        public void LoadModel(string str)
        {
            $"LoadModel message received for containerId: {str}"
                .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

            //if (null == loader)
            //{
            //    "Loader is not created."
            //        .ConsoleLog(LOG.LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel);

            //    loader = BundleLoader.Instance;
            //}

            BundleHandler.Loader.LoadModel(str);
        }

        // Notify external app only after the level finished loading
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //loader = BundleLoader.Instance;

            try
            {

#if UNITY_WEBGL
                UnityReceiverEnabled(name);
                //Debug.LogWarning("Before GetAssetInfoById");
                //var result = GetAssetInfoById(651);
                //Debug.LogWarning(result);
                //var json = JsonUtility.FromJson<BundleAsset>(result);
                //Debug.LogWarning("Conversion back to JSON: " + json);
#endif
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void Quit()
        {
//            Loader.Instance.QuitApplication();
        }

        #endregion

        #region ICommsFromUnity


        public void ClickedOnce(string aname, string bname = null)
        {
            if (string.IsNullOrEmpty(bname))
            {
                bname = BundleHandler.Instance.CurrentBundleName;
            }
            $"ClickedOnce Bundle Name is {bname}"
                .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

            // name to id
            var asset = GetAssetByNameAsJson(aname, bname);
            if (!string.IsNullOrEmpty(asset))
            {
#if UNITY_WEBGL

                ModelComponentClickedOnce(asset);
#endif
            }
        }

        public void ClickedTwice(string aname, string bname = null)
        {
            if (string.IsNullOrEmpty(bname))
            {
                bname = BundleHandler.Instance.CurrentBundleName;
            }
            $"ClickedTwice Bundle Name is {bname}".ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

            var asset = GetAssetByNameAsJson(aname, bname);
            if (!string.IsNullOrEmpty(asset))
            {
#if UNITY_WEBGL
                ModelComponentClickedTwice(asset);
#endif
            }
        }

        public void CursorEntered(int componentID)
        {
            if (componentID > -1)
                MouseEnter(componentID.ToString());
        }

        public void CursorLeft()
        {
            MouseLeave();
        }

        #endregion

        //private string GetAssetByNameAsJson(string compName)
        //{
        //    Debug.Log("GetAssetByNameAsJson: " + compName);
        //    if (null == loader)
        //    {
        //        Debug.LogError("Loader is not set");
        //        return null;
        //    }

        //    if (null == loader.tripssAssets || null == loader.tripssAssets.assetList)
        //    {
        //        Debug.LogError("Loader Tripss Asset list is not set");
        //        return null;
        //    }

        //    var filterResults = loader.tripssAssets.assetList.Where(item => item.AssetName == compName);

        //    if ( !filterResults.Any() )
        //    {
        //        Debug.LogWarning("NO items with " + compName + " name found!");
        //        return null;
        //    }
        //    // Sanity check
        //    else if ( filterResults.Count() > 1)
        //    {
        //        Debug.LogWarning( filterResults.Count() + " items with " + compName + " name found!");
        //    }

        //    var asset = filterResults.ElementAt(0);

        //    var json =
        //        $"{{\"detail\":{{\"componentName\":\"{asset.AssetName}\",\"containerId\":\"{asset.ContainerID}\",\"commonName\":\"{asset.Description}\"}} }}";

        //    // Debug.LogWarning(json);

        //    return json;
        //}
        // IMPLEMENTATION IS NOT FINISHED
        private string GetAssetsByNameAsJson(string aname, string bname)
        {
            // Debug.Log("GetAssetByNameAsJson: " + bname + "|" + aname);
            var filterResults = GetAssetsByAssetNameAndBundleName(aname, bname);

            return filterResults;
        }

        private string GetAssetByNameAsJson(string aname, string bname)
        {
            // Debug.Log("GetAssetByNameAsJson: " + bname + "|" + aname);
            //if (null == loader)
            //{
            //    "Loader is not set"
            //         .ConsoleLog(LOG.LogLevel.Error, AppSettings.Instance.staticSettings.logLevel);

            //    return null;
            //}

            //if (null == loader.tripssAssets || null == loader.tripssAssets.assetList)
            //{
            //    Debug.LogError("Loader Tripss Asset list is not set");
            //    return null;
            //}


            // This probably need to be accessed through IAssetInfo interface
            var filterResults = GetAssetInfoByAssetNameAndBundleName(aname, bname);

            if (string.IsNullOrEmpty(filterResults))
            {
                return null;
            }

            var asset = JsonUtility.FromJson<AssetBundleItem>(filterResults);
            if (null == asset) return null;

            //// Sanity check
            //else if (filterResults.Count() > 1)
            //{
            //    Debug.LogWarning(filterResults.Count() + " items with " + compName + " name found!");
            //}

            //var asset = filterResults.ElementAt(0);

            var json =
                $"{{\"detail\":{{" +
                $"\"componentName\":\"{asset.AssetName}\"," +
                $"\"containerId\":\"{asset.ContainerID}\"," +
                $"\"commonName\":\"{asset.Description}\"}}" +
                $" }}";

            // Debug.LogWarning(json);

            return json;
        }

        public string GetAssetInfo(int id)
        {
            return GetAssetInfoById(id);
        }
        public string GetAssetsDB()
        {
            return GetAssetBundleDataAsString();
        }

        public string FindAssetInfoByAssetNameAndBundleName(string aname, string bname)
        {
            return GetAssetInfoByAssetNameAndBundleName(aname, bname);
        }

        public string RequestData(string json)
        {
            $"RequestData with\r {json}"
                .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

            return DataRequest(json);
        }

        /// <summary>
        /// {
        /// }
        /// </summary>
        /// <param name="json"></param>

        public void DataRequestReply(string json)
        {
            $"DataRequestReply JSON: {json}"
                .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

            // var res = JsonUtility.FromJson<Command>(json);
        }

        public AssetBundleItem TryItemLookup(int containerId)
        {
            var val =  GetAssetInfoById(containerId);
            if (val.IsEmptyObject()) {
                return null;
            }
            var item = JsonUtility.FromJson<AssetBundleItem>(val);
            return item;
        }
    }
}
