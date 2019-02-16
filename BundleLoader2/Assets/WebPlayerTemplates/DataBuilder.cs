using NG.TRIPSS.CORE;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NG.TRIPSS.DATA
{
    public class DataBuilder : MonoBehaviour
    {

        public List<AssetBundleItemList> dataList;
        public AssetBundleItemList mergedData;

        public bool save = true;

        public void Start()
        {
            if (save)
                Save();
            else
                Restore();
        }

        public void Save()
        {
            Debug.Log("Saving...");
            foreach (var list in dataList)
            {
                list.Save();
            }
        }

        public void Restore()
        {
            var fName = Application.dataPath + "/../" + "allModels.json";

            if (!File.Exists(fName))
            {
                Debug.Log("File does not exist: " + fName);
                return;
            }
            else
            {
                string json = File.ReadAllText(fName);

                Debug.Log(json);

                if (mergedData)
                    JsonUtility.FromJsonOverwrite(json, mergedData);
            }
        }

        //public void Save(string path = "")
        //{
        //    if (assets && !assets.IsEmpty)
        //    {
        //        var json = assets.ToJson();
        //        Debug.Log(json);
        //        if (string.IsNullOrEmpty(path))
        //        {
        //            path = Application.dataPath + "/../" + assets.name + ".json";
        //        }

        //        File.WriteAllText(path, json);
        //    }
        //}

#if UNITY_EDITOR_WIN
        /*
        private BundleAssetList CreateBundleAssetList()
        {
            BundleAssetList asset = ScriptableObject.CreateInstance<BundleAssetList>();
    
            AssetDatabase.CreateAsset(asset, "Assets/BundleAssetList.asset");
            AssetDatabase.SaveAssets();
            return asset;
        }
    
    
        public void Restore(string path = "")
        {
    
            if (string.IsNullOrEmpty(path))
            {
                path = Application.dataPath + "/../";
            }
    
            var fName = path + (assets ? assets.name : "BundleAssetList") + ".json";
    
            if (!File.Exists(fName))
            {
                Debug.Log("File does not exist: " + fName);
                return;
            }
            else
            {
                string json = File.ReadAllText(fName);
    
                Debug.Log(json);
    
                if (!assets)
                {
                    assets = CreateBundleAssetList();
                }
                if (assets)
                    JsonUtility.FromJsonOverwrite(json, assets);
            }
        }
        */
#endif

    }

    public static class DataBuilderHelper
    {
        public static void Save(this AssetBundleItemList list)
        {
            Debug.Log("DataBuilderHelper.Save");
            string json = JsonUtility.ToJson(list);

            Debug.Log(json);

            var path = Application.dataPath + "/../" + list.name + ".json";
            Debug.Log("DataBuilderHelper.Save to: " + path);


            File.WriteAllText(path, json);
        }
    }
}