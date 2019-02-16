using System.Collections;
using System.Collections.Generic;
using NG.TRIPSS.CONFIG;
using UnityEngine;

public class DoubleClick : MonoBehaviour
{
    private float TimeBetweenClicks = 0.2f;
    private float firstClickTime;
    private bool coroutineAllowed;
    private int clickCounter;

	void Start () {
	    TimeBetweenClicks = AppSettings.Instance.staticSettings.doubleClickTimeLimit;
        Reset();
	}
	
	void Update ()
	{
	    if (Input.GetMouseButtonUp(0))
	        clickCounter++;

	    if (1 == clickCounter && coroutineAllowed)
	    {
	        firstClickTime = Time.time;
	        StartCoroutine(DoubleClickDetection());
	    }
	}

    private IEnumerator DoubleClickDetection()
    {
        coroutineAllowed = false;
        while (Time.time < firstClickTime + TimeBetweenClicks)
        {
            if (2 == clickCounter)
            {
                Debug.Log("Double Click");
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        Reset();
    }

    private void Reset()
    {
        firstClickTime = 0f;
        coroutineAllowed = true;
        clickCounter = 0;
    }
}
