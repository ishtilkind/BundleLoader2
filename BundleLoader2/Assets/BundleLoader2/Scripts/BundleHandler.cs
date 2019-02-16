using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NG.TRIPSS.CAMERA;
using NG.TRIPSS.CONFIG;
using NG.TRIPSS.CORE;
using NG.TRIPSS.CORE.LOG;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace NG.TRIPSS.CORE
{
	/// <summary>
	/// Class responsible for Asset Bundle manipulations
	/// </summary>
	public class BundleHandler : MonoBehaviour, IBundleHandler
	{
		/// <summary>
		/// Parent transform for an instantiated asset
		/// </summary>
		[Header("Spawn Point")] 
		[Tooltip("Parent transform for an instantiated asset")]
		public Transform spawn;
		
		[Header("Loaded Asset")] [SerializeField]
		private int currentID = -1;

		[SerializeField] private AssetBundleItemList.DB_Type currentDbType = AssetBundleItemList.DB_Type.Unset;

		[SerializeField] private AssetBundle currentBundle;
		[SerializeField] private GameObject currentAsset;

		public string CurrentBundleName => currentBundle?.name;
		
		[Header("Communication Manager")]
		[SerializeField]
		private CommunicationManager commsManager;
		
		[SerializeField] private bool loadDefaultBundle = true;

        #region Constants

        
        const int INVALID_BUNDLE_ID = -10;
        const int INVALID_ASSET_ID = -20;
        const int NOT_IN_DB_ID = -30;
        
        
        [Header("Default Model Settings")] public AssetBundleItemList missingAssets;

        
        private FreeObjectRotationCam cam;

        private IAssetBundleLoader loader;
        

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
            
            #region HANDLERS
    
            private void SameAssetIdEventHandler(int id)
            {
                $"SameAssetIdEventHandler for {id}"
                    .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
            }
    
            // --------------------------------------------------------------------------------------
            private void SameBundleAssetEventHandler(AssetBundleItem asset)
            {
                Debug.Log($"SameBundleAssetEventHandler for {asset?.ToJson()}");
            }
    
            private void CreateBundleAssetEventHandler(AssetBundleItem asset)
            {
                Debug.Log("CreateBundleAssetEventHandler for " + asset.AssetName);
    
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
                Debug.Log("LoadAssetBundleItemEventHandler for " + asset.ToJson());
    
                if (null == currentBundle || currentBundle.name != asset.BundleName)
                {
                    UnloadCurrentBundle();
                    StartCoroutine(LoadBundleAsync(asset));
                }
                else
                {
                    createAssetBundleItemEvent.Invoke(asset);
                }
            }
    
    
            private void InvalidBundleEventHandler(AssetBundleItem asset)
            {
                Debug.Log("InvalidBundleEventHandler for " + asset.ToJson());
                LoadDefaultAsset(INVALID_BUNDLE_ID);
            }
    
    
            private void NotInDbAssetEventHandler(int id)
            {
                $"NotInDbAssetEventHandler for ID: {id}"
                    .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
    
                LoadDefaultAsset(NOT_IN_DB_ID);
            }
    
            private void InvalidAssetEventHandler(AssetBundleItem asset)
            {
                Debug.Log("InvalidAssetEventHandler for " + asset.ToJson());
                LoadDefaultAsset(INVALID_ASSET_ID);
            }
    
    
            private void AssetLoadedEventHandler(AssetBundleItem asset)
            {
                //$"Asset Loaded Successfully: { asset.ToJson()}"
                //    .ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
            }
    
            private void LoadBundleAssetEventHandler(AssetBundleItem asset)
            {
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
                Debug.Log("SameAssetEventHandler for " + asset.ToJson());
            }
    
            #endregion
    
       	
		
	    private void RemoveListeners()
        {
            if (listenersInitialized)
            {
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


        private void OnEnable()
        {
	        AssignListeners();
        }

        private void OnDisable()
        {
	        RemoveListeners();
        }


        void Start()
		{
			loader = new Loader("../AssetBundles/WebGL/", commsManager, this);
		}

		void Update()
		{

		}
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		

		public void UnloadCurrentBundle()
		{
			$"UnloadCurrentBundle: {(currentBundle ? currentBundle.name : "none")}"
				.ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);

			ResetCurrentAsset();
			if (currentBundle)
			{
				currentBundle.Unload(true);
				currentBundle = null;
			}
		}
		
		public void ResetCurrentAsset()
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


		public IEnumerator LoadAssetAsync(AssetBundleItem asset)
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

		public void LoadAsset(AssetBundleItem asset)
		{
			StartCoroutine(LoadAssetAsync(asset));
		}


		/// <summary>
		/// Use this method for cleanup only in case of Unity memory leaks
		/// </summary>
		public void DeleteAllGameObjects()
		{
			var allObjects = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];

			if (allObjects == null || allObjects.Length == 0 ) return;

			foreach (var go in allObjects)
			{
				GameObject.Destroy(go);
			}

			"All Objects Deleted"
				.ConsoleLog(LOG.LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
		}
		
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
		
		private string GetUri(string assetBundleName)
		{
			if (null == commsManager)
			{
				commsManager = CommunicationManager.Instance;
			}

			if (string.IsNullOrEmpty(assetBundleName))
			{
				"assetBundleName is not set in GetUri.".ConsoleLog(LOG.LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel);
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

			if (!bundleUnloaded && loader.SameAsset(id, assets.type))
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

		
	}
}
