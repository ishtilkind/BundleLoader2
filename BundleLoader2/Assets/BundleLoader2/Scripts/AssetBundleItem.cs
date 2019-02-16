using NG.TRIPSS.CORE;

[System.Serializable]
public class AssetBundleItem
{
    public int ACAssignmentID;
    public int ContainerID;
    public int AssetID;
    public string BundleName;
    public string AssetName;
    public string PrefabName;
    public string Description;
    public string BundleBaseName;
    public string BundleExtension;
    public float InitialScale;
    public float InitialRotationX;
    public float InitialRotationY;
    public float InitialRotationZ;
}

public static class AssetBundleItemExtensions
{
    public static string ToDetailedString(this AssetBundleItem item)
    {
        return $"ACAssignmentID={item.ACAssignmentID}|ContainerID={item.ContainerID}|AssetID={item.AssetID}|BundleName={item.BundleName}|AssetName={item.AssetName}|PrefabName={item.PrefabName}|" +
               $"Description={item.Description}|BundleBaseName={item.BundleBaseName}|BundleExtension={item.BundleExtension}|InitialScale={item.InitialScale}" +
               $"|InitialRotationX={item.InitialRotationX}|InitialRotationY={item.InitialRotationY}|InitialRotationZ={item.InitialRotationZ}";
    }

}