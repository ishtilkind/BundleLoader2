using UnityEngine;


namespace NG.TRIPSS.CAMERA
{
    [AddComponentMenu("Camera-Control/BACN Camera Style")]
    public class FreeObjectRotationCam : MonoBehaviour
    {
        private Transform myTransform;
        [SerializeField] private Transform target;
        public Vector3 targetOffset;
        public float maxDistance = 20;
        public float minDistance = .6f;
        public float xSpeed = 200.0f;
        public float ySpeed = 200.0f;
        public int yMinLimit = -80;
        public int yMaxLimit = 80;
        public int zoomRate = 1;
        public float panSpeed = 0.3f;
        public float zoomDampening = 5.0f;

        private static float currentDistance;

        private Quaternion currentRotation;
        private Quaternion desiredRotation;
        private Quaternion rotation;
        private Vector3 position;

        private static float xDeg = 0.0f;
        private static float yDeg = 0.0f;

        private static float loadDistance = 5.0f;

        private static float desiredDistance;

        public Transform loadedAsset;

        private void Awake()
        {
            myTransform = GetComponent<Transform>();
        }

        //link the camera to the GUI objects on start up.
        private void Start()
        {
            Init();
        }

        void OnEnable()
        {
            Init();
        }

        public void ResetCamera()
        {
            xDeg = 0f;
            yDeg = 0f;
            desiredDistance = loadDistance;
        }

        public void Init()
        {
            // ASSIGN loaded asset position to a camera target position
            if (loadedAsset)
            {
                loadedAsset.position = target.position;
            }

            loadDistance = Vector3.Distance(myTransform.position, target.position);
            currentDistance = loadDistance;
            desiredDistance = loadDistance;

            //grab the current rotations as starting points.
            position = myTransform.position;
            rotation = myTransform.rotation;
            currentRotation = myTransform.rotation;
            desiredRotation = myTransform.rotation;

            xDeg = Vector3.Angle(Vector3.right, myTransform.right);
            yDeg = Vector3.Angle(Vector3.up, myTransform.up);
        }

        /*
         * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
         */

        private void LateUpdate()
        {
            if (loadedAsset)
            {
                float temp = Mathf.Sqrt((loadedAsset.localScale.x) * 2.5f);
                if (currentDistance < temp)
                {
                    desiredDistance = temp;
                    currentDistance = temp;
                    return;
                }
            }

            // If Middle button? ZOOM!
            if (Input.GetMouseButton(2))
            {
                desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate * 0.125f *
                                   Mathf.Abs(desiredDistance);
            }

            // If left left mouse button? ORBIT or fire GUI event
            else if (Input.GetMouseButton(0))
            {
                xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
//			}
                //add code to find the position x and y of mouse then find if the mouse is not moving for a 
                //set time and fire a marking menue event if no motion is found. This may need to be done in the ComponentPresets.CS
            }
            // Keyboard movement(Orbit and Zoom), WASD, Arrows, Ctrl, Alt, + and - (set in edit > progect setings > input)
            else if (target)
            {
                xDeg -= Input.GetAxis("Horizontal") * xSpeed * 0.02f;
                yDeg += Input.GetAxis("Vertical") * ySpeed * 0.02f;

                yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);

                desiredDistance += Input.GetAxis("Fire1") * Time.deltaTime * zoomRate * 0.125f *
                                   Mathf.Abs(desiredDistance);
                desiredDistance -= Input.GetAxis("Fire2") * Time.deltaTime * zoomRate * 0.125f *
                                   Mathf.Abs(desiredDistance);
                desiredDistance -= Input.GetAxis("Fire3") * Time.deltaTime * zoomRate * 0.125f *
                                   Mathf.Abs(desiredDistance);
                desiredDistance += Input.GetAxis("Fire4") * Time.deltaTime * zoomRate * 0.125f *
                                   Mathf.Abs(desiredDistance);

            }
            ////////OrbitAngle

            //Clamp the vertical axis for the orbit
            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
            // set camera rotation 
            desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
            currentRotation = transform.rotation;

            rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
            transform.rotation = rotation;


            ////////Orbit Position

            // affect the desired Zoom distance if we roll the scrollwheel
            desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate *
                               Mathf.Abs(desiredDistance);
            //clamp the zoom min/max
            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
            // For smoothing of the zoom, lerp distance
            currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

            // calculate position based on the new currentDistance 
            position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
            transform.position = position;
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }
    }
}
