using System.Collections.Generic;
using UnityEngine;

namespace NG.TRIPSS.CORE
{
    [System.Serializable]
    public class AssetBundleItemList : ScriptableObject
    {
        public enum DB_Type
        {
            Unset,
            Production,
            Testing,
            Absent
        };

        public DB_Type type = DB_Type.Production;
        public List<AssetBundleItem> assetList;
    }
}
