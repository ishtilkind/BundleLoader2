using System;
using UnityEngine;

namespace NG.TRIPSS.UI
{
    public class VersionInfo : MonoBehaviour
    {
        public string BuildInfo => DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()
                                   + "\r\nUnity: " + Application.unityVersion;
    }
}

