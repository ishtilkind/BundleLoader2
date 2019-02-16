using UnityEngine;
using UnityEngine.UI;

namespace NG.TRIPSS.UI
{
    public class PathInfo : MonoBehaviour
    {
        public string BuildInfo => "Source: " + Application.dataPath + "\r\nTarget: ";

        void OnEnable()
        {
            Debug.LogWarning("Application.absoluteURL: " + Application.absoluteURL);
            GetComponent<Text>().text += Application.absoluteURL;
        }
    }
}
