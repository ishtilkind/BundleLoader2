using System;
using UnityEngine;


namespace NG.TRIPSS.CAMERA
{
    // DONT DELETE THIS
    public class ZoomOUT : MonoBehaviour
    {
        //button states color
        public Color normal = Color.white;
        public Color hover = Color.gray;
        public Color down = Color.green;

        [Serializable]
        public delegate void EventHandler(GameObject e);

        public event EventHandler MouseDown;
        private bool over = false;

        private Renderer rend = null;

        private Renderer[] rendArray = null;

        private void SetColor(Color col)
        {
            if (null != rend)
            {
                rend.material.color = col;
            }
            else if (null != rendArray)
            {
                foreach (Renderer r in rendArray)
                {
                    r.material.color = col;
                }
            }
            else
            {
                Debug.LogError("No Renderers found");
            }
        }

        // Use this for initialization
        void Start()
        {
            rend = GetComponent<Renderer>();
            if (null == rend)
            {
                rendArray = gameObject.GetComponentsInChildren<Renderer>();
                if (null == rendArray)
                {
                    Debug.LogError("No Renderers found");
                }
            }


            SetColor(normal);
        }

        //highlight with color when mouse enters (OnMouseEnter uses less CPU then OnMouseDown and leaves mouse down for event)
        void OnMouseEnter()
        {
            over = true;
            SetColor(hover);
        }

        void OnMouseExit()
        {
            over = false;
            SetColor(normal);
        }

        void OnMouseDown()
        {
            SetColor(down);
            //check to see if the mouse is hitting this objects collider, the event will be handled in the FreeObjectRotation script.
            if (MouseDown != null)
                MouseDown(this.gameObject);
        }

        void OnMouseUp()
        {
            SetColor(over ? hover : normal);
        }
    }
}
	
