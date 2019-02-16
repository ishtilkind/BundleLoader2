using System.Collections;
using System.Collections.Generic;
using NG.TRIPSS.CONFIG;
using UnityEngine;

public class LongClick : MonoBehaviour
{
    private float startTime, endTime;
    private float timeframe = 0.5f;// seconds

    private void Start ()
    {
        timeframe = AppSettings.Instance.staticSettings.longClickTimeLimit;

        Reset();
	}

    private void Update () {
	    if (Input.GetMouseButtonDown(0))
	        startTime = Time.time;

	    if (Input.GetMouseButtonUp(0))
	        endTime = Time.time;

	    if (endTime - startTime > timeframe)
	    {
	        Reset();
	    }
	}

    private void Reset()
    {
        Debug.Log("Long Click");
        startTime = 0f;
        endTime = 0f;
    }
}
