using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace NG.TRIPSS.UI
{
    public class VersionInfoDisplay : MonoBehaviour
    {
        [SerializeField]
        private Text textControl;

        [SerializeField] private TextMeshProUGUI fpsControl;

        private Transform _transform;

        private bool initialTextVisibility;

        [SerializeField]
        private bool hideTextInitially;

        private Vector3 initialScale = Vector3.one;


        void Start()
        {
            _transform = GetComponent<Transform>();
            initialScale = _transform.localScale;

            if (null == textControl)
            {
                // GetComponentInChildren finds only active objects!!!
                textControl = _transform.parent.GetComponentInChildren<Text>();
            }

            if (textControl)
            {
                if (hideTextInitially)
                {
                    textControl.gameObject.SetActive(false);
                }

                initialTextVisibility = textControl.gameObject.activeSelf;
            }
            else
            {
                Debug.LogWarning("Unity Version Text Control is not found!");
            }

            InitFpsDisplay();
        }

        void InitFpsDisplay()
        {
            if (null == fpsControl)
            {
                // GetComponentInChildren finds only active objects!!!
                fpsControl = _transform.parent.GetComponentInChildren<TextMeshProUGUI>();
            }

            if (fpsControl)
            {
                if (hideTextInitially)
                {
                    fpsControl.gameObject.SetActive(false);
                }

                initialTextVisibility = fpsControl.gameObject.activeSelf;
            }
            else
            {
                Debug.LogWarning("Unity FPS Control is not found!");
            }
        }

        public void ToggleInitialTextVisibility()
        {
            initialTextVisibility = !initialTextVisibility;
        }

        public void ShowVersionInfo()
        {
            if (textControl)
            {
                textControl.gameObject.SetActive(true);
            }
        }

        public void HideVersionInfo()
        {
            if (textControl && !initialTextVisibility )
                textControl.gameObject.SetActive(false);
        }

        public void ToggleVersionInfo()
        {
            if (textControl)
            {
                textControl.gameObject.SetActive(!textControl.gameObject.activeSelf);
            }
        }

        public void ShowFPS()
        {
            if (fpsControl)
            {
                fpsControl.gameObject.SetActive(true);
            }
        }

        public void HideFPS()
        {
            if (fpsControl && !initialTextVisibility)
                fpsControl.gameObject.SetActive(false);
        }

        public void ToggleFPS()
        {
            if (fpsControl)
            {
                fpsControl.gameObject.SetActive(!fpsControl.gameObject.activeSelf);
            }
        }

        public void ScaleUp()
        {
            _transform.localScale = initialScale * 1.1f;
        }

        public void ScaleDown()
        {
            _transform.localScale = initialScale;
        }
    }
}
