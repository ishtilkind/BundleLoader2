using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NG.TRIPSS.CORE
{
    public interface IDataSource
    {
        AssetBundleItem TryItemLookup(int containerId);
    }
}
