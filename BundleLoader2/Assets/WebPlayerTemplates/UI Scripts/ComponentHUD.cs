using UnityEngine;

namespace NG.TRIPSS.UI
{
    //[RequireComponent(typeof(SphereCollider))]
    [DisallowMultipleComponent]
    public class ComponentHUD : MonoBehaviour
    {
        private HUD_Text hudText;

        private void Awake()
        {
            hudText = (HUD_Text) FindObjectOfType(typeof(HUD_Text));
            if (null == hudText)
            {
                Debug.LogWarning("Could not find HUD_Text object.");
            }
        }

        public void OnMouseEnter()
        {
            //Debug.Log("Mouse enters " + name);

            if (null == hudText) return;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            hudText.Show(screenPos.x, screenPos.y, name);

        }

        public void OnMouseExit()
        {
            //Debug.Log("Mouse exits " + name);

            if (null == hudText) return;

            hudText.Hide();
        }
    }
}