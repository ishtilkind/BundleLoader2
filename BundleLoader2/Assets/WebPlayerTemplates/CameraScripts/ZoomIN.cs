using System;
using UnityEngine;


namespace NG.TRIPSS.CAMERA
{
    // DONT DELETE THIS
    public class ZoomIN : MonoBehaviour
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

        private void Awake()
        {
            Debug.Log("ZoomIN Awake");
            rend = GetComponent<Renderer>();
            if (null == rend)
            {
                rendArray = GetComponentsInChildren<Renderer>();
                if (null == rendArray || rendArray.Length == 0)
                {
                    Debug.LogError("No Renderers found on " + name);
                }
                else
                {
                    foreach (var rnd in rendArray)
                    {
                        Debug.LogWarning("Renderer found on " + rnd.name);
                    }
                }
            }
            else
            {
                Debug.LogWarning("Renderer found on " + name);
            }
        }

        void Start()
        {
            Debug.Log("ZoomIN Start");
            SetColor(normal);
        }

        //highlight with color when mouse enters (OnMouseEnter uses less CPU then OnMouseDown and leaves mouse down for event)
        void OnMouseEnter()
        {
            Debug.Log("ZoomIN OnMouseEnter");
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
            Debug.Log("ZoomIN OnMouseDown");
            SetColor(down);
            //Mark the object as hit for the FreeObjectRotation script.
            if (MouseDown != null)
                MouseDown(this.gameObject);
            //Debug.Log("I'm Hit!");		
        }

        void OnMouseUp()
        {
            SetColor(over ? hover : normal);
        }
    }
}




