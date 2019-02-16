using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;


public class FpsDisplay : MonoBehaviour {

    private TextMeshProUGUI fpsText;
    public float deltaTime;

    void OnEnable()
    {
        fpsText = GetComponent<TextMeshProUGUI>();
    }

    void onDisable()
    {
        fpsText = null;
    }

    void Update()
    {
        if (null == fpsText) return;

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = $"FPS: {Mathf.Ceil(fps)}" ;
    }

}
