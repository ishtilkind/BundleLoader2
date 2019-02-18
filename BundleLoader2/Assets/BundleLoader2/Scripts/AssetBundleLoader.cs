using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AssetBundleLoader
{
    public AssetBundleLoader(string path)
    {
        bundlePath = path;
        
    }

    private string bundlePath;
    
    public GameObject LoadAsset(string bundleName, string assetName)
    {
        GameObject go = null;
        if (string.IsNullOrEmpty(bundlePath))
        {
            bundlePath = Application.streamingAssetsPath;
        }
        
        var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(bundlePath, bundleName));
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
        }
        else
        {
            var prefab = myLoadedAssetBundle.LoadAsset<GameObject>(assetName);
            go = Object.Instantiate(prefab);            
        }

        return go;
    }
}
