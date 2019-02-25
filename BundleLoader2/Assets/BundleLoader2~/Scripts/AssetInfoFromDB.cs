using UnityEngine;

namespace NG.TRIPSS.CORE
{
    public class AssetInfoFromDB : IAssetInfo
    {
        public AssetBundleItem GetAssetInfo(int id)
        {
            // Debug.Log($"AssetInfoFromDB: GetAssetInfo  with TRIPSS ID: {id}");
            if (id < 0)
            {
                Debug.LogError("Invalid Asset ID.");
                return null;
            }

            var val = CommunicationManager.Instance.GetAssetInfo(id);

            if (val.IsEmptyObject())
            {
                // Item not found in DB
                Debug.LogWarning($"Asset ID {id} NOT FOUND in DB.");
                return null;
            }

            var item = JsonUtility.FromJson<AssetBundleItem>(val);
            Debug.LogWarning(item.ToDetailedString());

            return item;
        }

        public AssetBundleItem GetAssetByAssetNameAndBundleName(string aname, string bname)
        {
            // Debug.Log($"AssetInfoFromDB:GetAssetByAssetNameAndBundleName Asset {aname} Bundle {bname}.");

            try
            {
                var val = CommunicationManager.Instance.FindAssetInfoByAssetNameAndBundleName(aname, bname);

                if (val.IsEmptyObject()) return null;

                return JsonUtility.FromJson<AssetBundleItem>(val);
            }
            catch (ExitGUIException e)
            {
                Debug.LogError(e.Message);
                return null;
            }
        }

        public int GetContainerIdByAssetNameAndBundleName(string aname, string bname)
        {
            var item = GetAssetByAssetNameAndBundleName(aname, bname);
            return item?.ContainerID ?? -1;
        }
    }
}
