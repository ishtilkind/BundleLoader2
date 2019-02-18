using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Boo.Lang;
using NG.TRIPSS.CORE;
using UnityEditor.VersionControl;
using UnityEngine.TestTools.NUnitExtensions;

public class LoaderTestScript
{

    public class FakeDataSource : IDataSource
    {            
        System.Collections.Generic.List<AssetBundleItem> source = new System.Collections.Generic.List<AssetBundleItem>()
        {
            new AssetBundleItem()
            {
             ACAssignmentID=1,
             ContainerID=1,
             AssetID=1,
             BundleName="Z33.assetbundle",
             AssetName="Z33X03",
             PrefabName="Z33X03",
             Description="Zone 33 gun",
             BundleBaseName="Z33",
             BundleExtension=".assetbundle",
             InitialScale=1f,
             InitialRotationX=0f,
             InitialRotationY=0f,
             InitialRotationZ=0f                     
            },
            new AssetBundleItem()
            {
             ACAssignmentID=2,
             ContainerID=2,
             AssetID=2,
             BundleName="Z44.assetbundle",
             AssetName="Z44X03",
             PrefabName="Z44X03",
             Description="Zone 44 gun",
             BundleBaseName="Z44",
             BundleExtension=".assetbundle",
             InitialScale=1f,
             InitialRotationX=0f,
             InitialRotationY=0f,
             InitialRotationZ=0f                     
            },
        };

        public AssetBundleItem TryItemLookup(int containerId)
        {
            return source.Find( item => item.ContainerID == containerId);
        }
    };

    private Loader loader = null;
    private IDataSource dataSource = null;
    private IBundleHandler bundleHandler = null;

    [OneTimeSetUp]
    public void OnetimeSetup()
    {
        
    }

    [SetUp]
    public void Setup()
    {
        var go = new GameObject();
        bundleHandler = go.AddComponent<BundleHandler>();
        dataSource = new FakeDataSource();
        loader = new Loader("", dataSource, null);
    }

    [TearDown]
    public void Teardown()
    {
        
    }
    
    
//    [Test]
//    [Order(1)]
//    public void LogErrors()
//    {
//        LogAssert.ignoreFailingMessages = true;
//        Debug.LogError("Whoops!");
//        LogAssert.ignoreFailingMessages = false;
//    }

    [Test]
    public void BundleHandler_Created()
    {
        Assert.NotNull(bundleHandler);
//        IBundleHandler ibl = bl;
//        ibl.LoadAsset(new  AssetBundleItem());
        
//        var result = loader.LoadModel("1");
//        Assert.IsTrue(result);
    }

    [Test]
    public void Loader_LoadModel_ContainerId_1()
    {
//        var result = loader.LoadModel("1");
//        Assert.IsTrue(result);
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
//    [UnityTest]
//    public IEnumerator LoaderTestScriptWithEnumeratorPasses() {
//        LogAssert.Expect(LogType.Error, "Failed.");
//        Debug.LogError("Failed.");
//        // yield to skip a frame
//        yield return null;
//    }
}
