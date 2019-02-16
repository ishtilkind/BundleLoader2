using System.Collections;
using UnityEngine;


namespace NG.TRIPSS.CONFIG
{
    public class AppSettings : MonoBehaviour {

        public Settings staticSettings;

        private static AppSettings _instance;
        public static AppSettings Instance { get; private set; }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            if (null == _instance
                // Destroyed Object
                || _instance.Equals(null)
            )
            {
                // Debug.LogWarning("Creating and Eagerly Initializing App Settings");

                var gameObject = new GameObject("AppSettings");
                Instance = gameObject.AddComponent<AppSettings>();

                DontDestroyOnLoad(gameObject);
            }
        }

        private IEnumerator LoadInternals()
        {
            var resources = Resources.LoadAll("Static Settings", typeof(Settings));
            if (resources == null || resources.Length == 0)
            {
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                Debug.LogError("Error loading Static Settings from Resources!");
                yield break;
            }

            staticSettings = resources[0] as Settings;
            if (null == staticSettings)
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                Debug.LogError("Unable to load Static Settings");
            //else
            //    Debug.Log($"Log Level: {staticSettings.logLevel}");

            yield return null;
        }

        void Awake () {
            StartCoroutine( LoadInternals() );
        }

        //void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftControl))
        //    {
        //        if (null == staticSettings)
        //        {
        //            Debug.LogWarning("Static Setting are missing!");
        //            return;
        //        }

        //        staticSettings.useLocalDB = !staticSettings.useLocalDB;
        //        Debug.Log($"UseLocalDB is set to {staticSettings.useLocalDB}");
        //    }
        //}
    }
}

