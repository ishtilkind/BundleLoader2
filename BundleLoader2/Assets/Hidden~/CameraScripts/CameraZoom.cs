using NG.TRIPSS.CONFIG;
using NG.TRIPSS.CORE;
using NG.TRIPSS.CORE.LOG;
using UnityEngine;


namespace NG.TRIPSS.CAMERA
{
    [RequireComponent(typeof(CameraInput))]
    public class CameraZoom : MonoBehaviour
    {

        private Transform myTransform;
        [SerializeField] private Transform pivotTransform;

        private float _CameraDistance = 10f;
        [SerializeField] private float _CameraDistanceMin = 4f;

        public float ScrollDampening = 6f;

        private float initialDistance = 10f;
        private const float initialCameraDistanceMin = 4f;

        // Use this for initialization
        void OnEnable()
        {
            this.myTransform = this.transform;
            //this.pivotTransform = this.transform.parent;
            CameraInput.OnScroll += Scroll;

            BundleLoaderNotWorking.OnAssetLoaded += AssetLoaded;

            initialDistance = Vector3.Distance(myTransform.position, pivotTransform ? pivotTransform.position : Vector3.zero);
        }

        void OnDisable()
        {
            CameraInput.OnScroll -= Scroll;
            BundleLoaderNotWorking.OnAssetLoaded -= AssetLoaded;
        }

        void LateUpdate()
        {
            if (this.myTransform.localPosition.z != this._CameraDistance * -1f)
            {
                this.myTransform.localPosition = new Vector3(0f, 0f,
                    Mathf.Lerp(this.myTransform.localPosition.z, this._CameraDistance * -1f,
                        Time.deltaTime * ScrollDampening));
            }
        }

        void Reset()
        {
            this._CameraDistance = initialDistance;
            _CameraDistanceMin = initialCameraDistanceMin;
        }

        private void Scroll(float value)
        {
            float ScrollAmount = value;

            ScrollAmount *= (this._CameraDistance * 0.3f);

            this._CameraDistance += ScrollAmount * -1f;

            this._CameraDistance = Mathf.Clamp(this._CameraDistance, _CameraDistanceMin, 60f);
            //Debug.Log("_CameraDistanceMin 3: " + _CameraDistanceMin + " _CameraDistance: " + _CameraDistance);
        }

        private void AssetLoaded(GameObject go)
        {
            //Debug.Log("Resetting CameraZoom on Asset Loaded: " + go.name);
            Reset();
        }

    }
}
