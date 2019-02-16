using UnityEditor;
using UnityEngine;

public class CustomMenu : MonoBehaviour {

    [MenuItem("Custom Menu/Do Something")]
    public static void DoSomething()
    {
        Debug.Log("Do Something...");
    }
}
