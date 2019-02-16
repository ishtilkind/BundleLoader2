using System.Collections;
using System.Collections.Generic;
using NG.TRIPSS.CONFIG;
using NG.TRIPSS.CORE;
using NG.TRIPSS.CORE.LOG;
using UnityEngine;

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

		void Start()
		{

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
	}
}
