using NG.TRIPSS.CAMERA;
using NG.TRIPSS.CONFIG;
using NG.TRIPSS.CORE;
using NG.TRIPSS.CORE.LOG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LogLevel = NG.TRIPSS.CORE.LOG.LogLevel;

namespace NG.TRIPSS.INPUT
{
    [RequireComponent(typeof(BoxCollider))]
    [DisallowMultipleComponent]
    public class MouseClickHandler : MonoBehaviour
    {
        #region ClickController

        [System.Serializable]
        private class OnSingleClick : UnityEvent<string, string> { };
        private OnSingleClick onSingleClickEvent = new OnSingleClick();

        [System.Serializable]
        private class OnDoubleClick : UnityEvent<string, string> { };
        private OnDoubleClick onDoubleClickEvent = new OnDoubleClick();

        [System.Serializable]
        private class OnLongClick : UnityEvent<string, string> { };
        private OnLongClick onLongClickEvent = new OnLongClick();

        //// Need to subscribe and track camera rotation 
        //// to avoid context menu popup after mouse button release
        //// after the model rotation
        //private CameraInput input;

        #region ClickTypeProcessor

        private ClickTypeProcessor clickTypeProcessor = null;

        class ClickTypeProcessor
        {
            public override string ToString()
            {
                return $"doubleClickTimeLimit:{doubleClickTimeLimit}|longClickTimeLimit:{longClickTimeLimit},clickCount:{clickCount},ClickUpColliderTime:{ClickUpColliderTime}," +
                       $"ClickUpAnywhereTime:{ClickUpAnywhereTime},ClickUpAnywhereTime:{ClickUpAnywhereTime}";
            }
            public string ToJson()
            {
                return $"{{\"doubleClickTimeLimit\": {doubleClickTimeLimit},\"longClickTimeLimit\": {longClickTimeLimit},\"clickCount\": {clickCount}," +
                       $"\"ClickUpColliderTime\": {ClickUpColliderTime},\"ClickUpAnywhereTime\": {ClickUpAnywhereTime},\"ClickUpAnywhereTime\": {ClickUpAnywhereTime}}}";
            }
            public ClickTypeProcessor(Settings settings) 
            {
                doubleClickTimeLimit = settings.doubleClickTimeLimit;
                longClickTimeLimit = settings.longClickTimeLimit;

                Init();
            }
            public void Init()
            {
                clickCount = 0;
                ClickUpColliderTime = 0f;
                ClickUpAnywhereTime = 0f; ;
                ClickDownColliderTime = 0f; ;
                Locked = false;
            }

            private float ClickUpColliderTime { get; set; }
            private float ClickUpAnywhereTime { get; set; }
            private float ClickDownColliderTime { get; set; }
            public bool Locked { get; set; }

            public void RecordClickUpColliderTime() => ClickUpColliderTime = Time.time;
            public void RecordClickUpAnywhereTime() => ClickUpAnywhereTime = Time.time;
            public void RecordClickDownColliderTime() => ClickDownColliderTime = Time.time;

            public void Increment() => clickCount++;
            public  bool FirstAllowedClick => !Locked && 1 == clickCount;
            public int Count => clickCount;
            public bool MultiClick => clickCount > 1;
            private float ClickDuration => ClickUpAnywhereTime - ClickDownColliderTime;
            private float ClickColliderDuration => ClickUpColliderTime - ClickDownColliderTime;

            public float DoubleClickTimeExpiration() => ClickUpColliderTime + doubleClickTimeLimit;
            public bool LongClickTimeReached() => ( ClickUpAnywhereTime - ClickDownColliderTime > longClickTimeLimit);

            public bool LongClickTimeExpired() => ClickDuration > longClickTimeLimit;

            // variables
            private volatile int clickCount;
            private readonly float doubleClickTimeLimit;
            private readonly float longClickTimeLimit;
        }

        #endregion

        private Settings staticSettings;

        [Header("Optional Asset Info Overwrites")]
        [SerializeField] private string assetNameOverwrite;
        [SerializeField] private string bundleNameOverwrite;

        [SerializeField] private GameObject rendererOverwrite;

        #endregion

        #region ClickReceiver

        private CommunicationManager commManager;
        private BundleLoaderNotWorking _loaderNotWorking;

        private const string COLOR = "_Color";

        private List<Tuple<MeshRenderer, Color>> colorInfo = new List<Tuple<MeshRenderer, Color>>();

        private MeshRenderer[] childRends;
        private Color[] initialColorChildren;

        #endregion

        #region MonoBehaviour

        void Awake()
        {
            StartCoroutine(Init());
        }

        IEnumerator Init()
        {
            while (null == CommunicationManager.Instance)
            {
                yield return new WaitForEndOfFrame();
            }
            commManager = CommunicationManager.Instance;

            while (null == BundleLoaderNotWorking.Instance)
            {
                yield return new WaitForEndOfFrame();
            }
            _loaderNotWorking = BundleLoaderNotWorking.Instance;
            while (null == AppSettings.Instance)
            {
                yield return new WaitForEndOfFrame();
            }
            if (!staticSettings)
                staticSettings = AppSettings.Instance.staticSettings;

            "MouseClickHandler Initialized"
                .ConsoleLog(LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
        }

        void OnEnable()
        {
            onSingleClickEvent.AddListener(onSingleClickReceived);
            onDoubleClickEvent.AddListener(onDoubleClickReceived);
            onLongClickEvent.AddListener(onLongClickReceived);

            if (null == rendererOverwrite)
            {
                $"Renderer Overwrite is null and will be set to {name}"
                    .ConsoleLog(LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
                rendererOverwrite = gameObject;
            }

            clickTypeProcessor = new ClickTypeProcessor(AppSettings.Instance.staticSettings);
        }

        void OnDisable()
        {
            onSingleClickEvent.RemoveAllListeners();
            clickTypeProcessor = null;
        }

        private int GetContainerId()
        {
            string aname = string.IsNullOrEmpty(assetNameOverwrite) ? name : assetNameOverwrite;
            string bname = string.IsNullOrEmpty(bundleNameOverwrite) ? _loaderNotWorking.CurrentBundleName : bundleNameOverwrite;

            return GetContainerId(aname, bname);
        }

        private int GetContainerId(string aname, string bname)
        {
            int id = -1;

            if (string.IsNullOrEmpty(assetNameOverwrite))
            {
                id = _loaderNotWorking.GetContainerIdByAssetNameAndBundleName(name);
            }
            else
            {
                id = _loaderNotWorking.GetContainerIdByAssetNameAndBundleName(assetNameOverwrite,
                    string.IsNullOrEmpty(bundleNameOverwrite) ? _loaderNotWorking.CurrentBundleName : bundleNameOverwrite);
            }

            return id;
        }

        #region 3D
        // TODO: Need to retest following 3D methods and replace 2D methods.

        void OnMouseEnter()
        {
            int id = GetContainerId();

            try { 
                if (id < 0)
                {
                    Highlight(false);
                    var tmp = (string.IsNullOrEmpty(assetNameOverwrite) ? name : assetNameOverwrite);
                    $"Mouse Over. ID NOT FOUND FOR: {tmp}"
                        .ConsoleLog(LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel);
                }
                else
                {
                    Highlight(true);
                    commManager.CursorEntered(id);
                }
            }
            catch (Exception e)
            {
                e.Message
                    .ConsoleLog(LogLevel.Error, AppSettings.Instance.staticSettings.logLevel);
            }

        }

        // OnMouseUpAsButton is only called when the mouse is released over the same GUIElement or Collider as it was pressed.
        void OnMouseUpAsButton()
        {
            if (null == clickTypeProcessor)
            {
                clickTypeProcessor = new ClickTypeProcessor(AppSettings.Instance.staticSettings);
            }

            int id = GetContainerId();
            if (id < 0)
            {
                clickTypeProcessor.Init();
                return;
            }

            clickTypeProcessor.Increment();

            if (clickTypeProcessor.FirstAllowedClick)
            {
                clickTypeProcessor.RecordClickUpColliderTime();
                StartCoroutine(ClickTypeDetection());
            }
            else
            {
                $"Unable start ClickTypeDetection for {name}: ContainerID is {id},  clickCount {clickTypeProcessor.Count} Locked {clickTypeProcessor.Locked}"
                    .ConsoleLog(LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
            }
        }

        void OnMouseDown()
        {
            clickTypeProcessor.RecordClickDownColliderTime();
        }

        // OnMouseUp is called even if the mouse is not over the same GUIElement or Collider as the has mouse has been pressed down on. 
        void OnMouseUp()
        {
            clickTypeProcessor.RecordClickUpAnywhereTime();
        }

        /// <summary>
        /// This method detects click type: Single, Double, or a Long Click
        /// </summary>
        /// <returns></returns>
        private IEnumerator ClickTypeDetection()
        {
            clickTypeProcessor.Locked = true;

            while (Time.time < clickTypeProcessor.DoubleClickTimeExpiration())
            {
                if (clickTypeProcessor.MultiClick)
                {
                    $"Multiclick on {name}"
                        .ConsoleLog(LogLevel.Debug, AppSettings.Instance.staticSettings.logLevel);

                    break;
                }
                yield return new WaitForEndOfFrame();
            }

            var aname = string.IsNullOrEmpty(assetNameOverwrite) ? name : assetNameOverwrite;
            var bname = string.IsNullOrEmpty(bundleNameOverwrite) ? _loaderNotWorking.CurrentBundleName : bundleNameOverwrite;

            //check if long click time is reached. This means the model was dragged.
            if ( clickTypeProcessor.LongClickTimeReached() )
            {
                $"LongClickTime Reached: {clickTypeProcessor.ToString()}"
                    .ConsoleLog(LogLevel.Debug, AppSettings.Instance.staticSettings.logLevel);
                onLongClickEvent.Invoke(aname, bname);
            }
            else
            {
                if (clickTypeProcessor.MultiClick)
                {
                    onDoubleClickEvent.Invoke(aname, bname);
                }
                else
                {
                    onSingleClickEvent.Invoke(aname, bname);
                }
            }

            clickTypeProcessor.Init();
        }

        void OnMouseExit() {
            try
            {
                commManager.CursorLeft();
                ResetColors();
            }
            catch (Exception e)
            {
                e.Message
                    .ConsoleLog(LogLevel.Error, AppSettings.Instance.staticSettings.logLevel);
            }

        }

        #endregion 3D

        #endregion

        private void onSingleClickReceived(string aname, string bname)
        {
            commManager.ClickedOnce(aname, string.IsNullOrEmpty(bname) ? _loaderNotWorking.CurrentBundleName : bname);
        }

        private void onDoubleClickReceived(string aname, string bname)
        {
            commManager.ClickedTwice(aname, string.IsNullOrEmpty(bname) ? _loaderNotWorking.CurrentBundleName : bname);
        }
        private void onLongClickReceived(string aname, string bname)
        {
            $"Long Click Event is not used at this time."
                .ConsoleLog(LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
        }

        private void Highlight(bool found)
        {
            if (null == colorInfo)
            {
                colorInfo = new List<Tuple<MeshRenderer, Color>>();
            }
            else if (colorInfo.Count > 0)
            {
                //colorInfo.Clear();
                ResetColors();
            }

            var renderers = rendererOverwrite?.GetComponentsInChildren<MeshRenderer>();

            if (null == renderers)
            {
                $"Error setting renderers for {name}"
                    .ConsoleLog(LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel);
            }

            if (null == renderers || renderers.Length == 0)
            {
                $"No MeshRenderer found for {rendererOverwrite?.name}"
                    .ConsoleLog(LogLevel.Warning, AppSettings.Instance.staticSettings.logLevel);

                return;
            }
            else
            {
                $"{renderers.Length} MeshRenderer found for {rendererOverwrite?.name}"
                    .ConsoleLog(LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
            }

            foreach (var rend in renderers)
            {
                if (rend.material.HasProperty("_Color"))
                {
                    colorInfo.Add(new Tuple<MeshRenderer, Color>(rend, rend.material.GetColor(COLOR)));
                    rend.material.SetColor(COLOR, found ? staticSettings.highlightColor : staticSettings.missingModelColor);
                }
                else
                {
                    $"{rend} does not have a Color property"
                        .ConsoleLog(LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
                }

            }
        }

        private void ResetColors()
        {
            if (null != colorInfo)
            {
                foreach (var node in colorInfo)
                {
                    node.Item1.material.SetColor(COLOR, node.Item2);
                }
                colorInfo.Clear();
            }
         }
    }

}

