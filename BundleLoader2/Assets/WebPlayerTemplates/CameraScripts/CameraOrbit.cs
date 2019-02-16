using NG.TRIPSS.CONFIG;
using NG.TRIPSS.CORE;
using UnityEngine;
using NG.TRIPSS.CORE.LOG;
using LogLevel = NG.TRIPSS.CORE.LogLevel;

namespace NG.TRIPSS.CAMERA
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CameraInput))]
    public class CameraOrbit : MonoBehaviour
    {

        //private Transform myTransform;
        [SerializeField] private Transform pivotTransform;
        private Transform cameraTransform;

        protected Vector3 _LocalRotation;

        public float MouseSensitivity = 4f;
        public float OrbitDampening = 10f;

        void Awake()
        {
            cameraTransform = GetComponent<Transform>();
        }

        void OnEnable()
        {
            //this.myTransform = this.transform;
            //this.pivotTransform = this.transform.parent;
            CameraInput.OnRotate += Rotate;
            BundleLoaderNotWorking.OnAssetLoaded += AssetLoaded;
        }

        void OnDisable()
        {
            CameraInput.OnRotate -= Rotate;
            BundleLoaderNotWorking.OnAssetLoaded -= AssetLoaded;
        }

        void LateUpdate()
        {
            //Actual Camera Rig Transformations
            Quaternion QT = Quaternion.Euler(_LocalRotation.y, _LocalRotation.x, 0);
            this.pivotTransform.rotation =
                Quaternion.Lerp(this.pivotTransform.rotation, QT, Time.deltaTime * OrbitDampening);
        }

        private void Rotate(float x, float y)
        {
            _LocalRotation.x += x;
            _LocalRotation.y += y * AdjustmentY();
            //Clamp the y Rotation to horizon and not flipping over at the top
            _LocalRotation.y = Mathf.Clamp(_LocalRotation.y, -90f, 90f);
        }

        private void Reset()
        {
            _LocalRotation.x = 0f;
            _LocalRotation.y = 0f;
        }

        private void AssetLoaded(GameObject go)
        {
            Reset();
        }

        // This is not a final solution but a good improvement. Rotation still off sometimes around 0 and 180 degrees.
        private float AdjustmentY()
        {
            // Get signed Angle between vector from the target to the camera and forward vector of the target.
            var angle = Vector3.SignedAngle(cameraTransform.position - pivotTransform.position, pivotTransform.forward,
                Vector3.up);
            // determine the mouse position on the screen (left or right from the target, i.e. the center of the screen)
            var left = Input.mousePosition.x < Screen.width * .5f;
            // Calculate adjustment
            var adjustmentY = angle <= Mathf.Epsilon ? 
                                        left ? 1f : -1f :
                                        left ? -1f : 1f;

            return adjustmentY;

        }
    }
}
