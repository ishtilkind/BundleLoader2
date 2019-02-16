using System;
using UnityEngine;
using NG.TRIPSS.CAMERA;
using NG.TRIPSS.CONFIG;


namespace NG.TRIPSS.CORE
{
    [DisallowMultipleComponent]
    public class Initializer : MonoBehaviour
    {

        public float scale = 10f;
        public Vector3 initRotation;

        // Use this for initialization
        void Start()
        {
            if (!AppSettings.Instance.staticSettings.useLocalDB)
            {
                // Debug.Log("Using DB Initialization data.");
                return;
            }

            if (!initRotation.AboutEquals(Vector3.zero))
            {
                transform.localRotation = Quaternion.Euler(initRotation);
                Debug.Log(name + " rotated " + initRotation);
            }

            if (!scale.AboutEquals(1.0f))
            {
                transform.localScale = Vector3.one * scale;
                Debug.Log("Current local scale is: " + transform.localScale.x);
            }

            Camera cam = Camera.main;

            if (cam)
            {
                var obj = cam.GetComponent<FreeObjectRotationCam>();
                if (obj)
                {
                    obj.loadedAsset = GetComponent<Transform>();
                    obj.ResetCamera();
                }
            }
        }
    }


    public static class InitializerUtils
    {
        public static bool AboutEquals(this float value1, float value2, float precalculatedContextualEpsilon = 0.001f)
        {
            return Math.Abs(value1 - value2) <= precalculatedContextualEpsilon;
        }

        public static bool AboutEquals(this Vector3 value1, Vector3 value2,
            double precalculatedContextualEpsilon = 0.001f)
        {
            return (
                Math.Abs(value1.x - value2.x) <= precalculatedContextualEpsilon &&
                Math.Abs(value1.y - value2.y) <= precalculatedContextualEpsilon &&
                Math.Abs(value1.z - value2.z) <= precalculatedContextualEpsilon
            );
        }
    }
}
