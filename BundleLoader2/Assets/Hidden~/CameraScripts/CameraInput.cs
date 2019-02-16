using System;
using NG.TRIPSS.CONFIG;
using NG.TRIPSS.CORE.LOG;
using UnityEngine;

namespace NG.TRIPSS.CAMERA
{
    public class CameraInput : MonoBehaviour
    {
        public float MouseX { get; private set; }
        public float MouseY { get; private set; }

        public float Horizontal { get; private set; }
        public float Vertical { get; private set; }

        public float MouseScrollWheel { get; private set; }

        public bool Rotate { get; private set; }

        public static event Action<float, float> OnRotate;
        public static event Action<float> OnScroll;
        //public static event Action OnReset;

        public float ScrollSensitvity = 2f;
        public float MouseSensitivity = 4f;

        /// <summary>
        /// Check for focus return used to skip the very first click in the frame.
        /// This prevent from the sudden model rotation when foris is return.
        /// </summary>
        private bool focusReturned = false;

        // Update is called once per frame
        void Update()
        {
            //// Enable to set checkForfocusReturned at a run tume
            //if ( Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha1))
            //{
            //    checkForfocusReturned = !checkForfocusReturned;
            //    $"CheckForfocusReturned is set to {checkForfocusReturned}"
            //        .ConsoleLog(LogLevel.Debug, AppSettings.Instance.staticSettings.logLevel);
            //}

            // Skip the first mouse click when the app focus returns.
            if (focusReturned)
            {
                focusReturned = false;
                "App Focus is back!".ConsoleLog(LogLevel.Info, AppSettings.Instance.staticSettings.logLevel);
            }
            else
            {
                Horizontal = Input.GetAxis("Horizontal");
                Vertical = Input.GetAxis("Vertical");

                MouseX = Input.GetAxis("Mouse X");
                MouseY = Input.GetAxis("Mouse Y");

                MouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");

                if (MouseScrollWheel != 0)
                    OnScroll(MouseScrollWheel * ScrollSensitvity);

                Rotate = Input.GetButton("Fire1");
                if (Rotate && (MouseX != 0 || MouseY != 0))
                    OnRotate(MouseX * MouseSensitivity * -1f, MouseY * MouseSensitivity);
            }
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
                focusReturned = true;
        }
    }
}
