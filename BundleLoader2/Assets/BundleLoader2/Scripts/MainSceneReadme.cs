#pragma warning disable CS0414 // The private field is assigned but its value is never used

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneReadme : MonoBehaviour
{

    [SerializeField] private string description = "Using RuntimeInitializeOnLoadMethod attribute is allowing for the following self instantiating Game Objects: 1.AppSettings with Component AppSettings 2.CommunicationManager with Component CommunicationManager";
    void ReadMe () {
#if (UNITY_EDITOR)

	    //  Using RuntimeInitializeOnLoadMethod attribute is allowing for the following self instantiating Game Objects:
	    //  1.AppSettings with Component AppSettings
	    //  2.CommunicationManager with Component CommunicationManager

#endif
    }

}
