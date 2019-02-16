namespace NG.TRIPSS.CORE
{
    public interface IAssetInfo
    {
        AssetBundleItem GetAssetInfo(int id);

        AssetBundleItem GetAssetByAssetNameAndBundleName(string aname, string bname);

        int GetContainerIdByAssetNameAndBundleName(string aname, string bname);
    }
}
