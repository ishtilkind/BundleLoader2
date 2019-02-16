using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NG.TRIPSS.UI
{
    public class HUD_Text : MonoBehaviour
    {
        private Transform trans;
        private TextMeshProUGUI hudText;
        private Image hudBackground;
        private float near;

        private void Awake()
        {
            hudText = GetComponentInChildren<TextMeshProUGUI>();
            hudBackground = GetComponentInChildren<Image>();
            trans = GetComponent<Transform>();
            hudText.enabled = false;
            hudBackground.enabled = false;
            near = Camera.main.nearClipPlane;
        }

        public void Show(float x, float y, string text)
        {
            trans.position = new Vector3(x, y, near);
            hudBackground.enabled = true;
            hudText.text = text;
            hudText.enabled = true;
        }

        public void Hide()
        {
            hudText.text = "";
            hudBackground.enabled = false;
            hudText.enabled = false;
        }
    }
}
