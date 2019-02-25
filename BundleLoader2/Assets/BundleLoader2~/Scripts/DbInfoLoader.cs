using System;
using System.Collections;
using System.IO;
using NG.TRIPSS.CORE;
using UnityEngine;
using UnityEngine.Networking;

public class DbInfoLoader : MonoBehaviour
{
    [SerializeField] private String userName;
    [SerializeField] private String pwHash;
    [SerializeField] private String server;
    [SerializeField] private AssetBundleItemList assets;
    [SerializeField] private string path = "C:/Users/4B14990/Documents/GitHub/TRIPSS PROJECTS/tripss_3d_models/tripss_3d_models/_assetList.json";

    // Use this for initialization
    void Start () {
		
	}

    public void LoadDataFromFile()
    {
        //string jsonData = File.ReadAllText(path);
        //var serializedArray = JsonUtility.FromJson<AssetBundleItemList>(jsonData);

        using (StreamReader r = new StreamReader(path))
        {
            string json = r.ReadToEnd();
            assets = JsonUtility.FromJson<AssetBundleItemList>(json);
        }
    }

    public void Restore()
    {
        //var fName = Application.dataPath + "/../" + "allModels.json";

        if (!File.Exists(path))
        {
            Debug.Log("File does not exist: " + path);
            return;
        }
        else
        {
            string json = File.ReadAllText(path);

            Debug.Log(json);

            if (assets)
                JsonUtility.FromJsonOverwrite(json, assets);
        }
    }

    public void GetData()
    {
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        var uri = "http://" + server + "/TRIPSS_WebService.svc/rest/UserNaCl/" + userName;
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            //yield return www.Send();
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);

                // Or retrieve results as binary data
                byte[] results = www.downloadHandler.data;
                string s = System.Text.Encoding.UTF8.GetString(results, 0, results.Length);
                Debug.Log(s);

            }
        }
    }

    // Update is called once per frame
    void Login ()
	{
        //    // We always expect to use application/json on what we will receive
        //	    var headers = New - Object "System.Collections.Generic.Dictionary[[String],[String]]"
        //	        $headers.Add("Accept", 'application/json; charset=utf-8')
        //# We we need to get salt, this is how we would do it
        //#$uri = "http://" + $server + "/TRIPSS_WebService.svc/rest/UserNaCl/" + $employeeID
        //#$pwHash="5e71e8e0c480499773f9590dc33d8c0a1c373db7105f1dd59411596d3a285871"
        //#$response = Invoke-RestMethod  -Uri $uri  -Headers $headers
        //#$response
        //# Login the user...

        //	        $loginInfo = @{
        //	        strPWHash = '5e71e8e0c480499773f9590dc33d8c0a1c373db7105f1dd59411596d3a285871'
        //	        intAppID = 1
        //	    }

    }
}
