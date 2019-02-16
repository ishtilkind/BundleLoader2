using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using NG.TRIPSS.CORE;

public class LoaderTestScript
{

    private Loader loader = null;
    private readonly IDataSource dataSource = null;

    [SetUp]
    public void Setup()
    {
        loader = new Loader("", dataSource, null);
    }

    [TearDown]
    public void Teardown()
    {
        
    }

    [Test]
    public void Loader_LoadModel_ContainerId_1()
    {
        var result = loader.LoadModel("1");
        Assert.True(result);
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator LoaderTestScriptWithEnumeratorPasses() {
        LogAssert.Expect(LogType.Error, "Failed.");
        Debug.LogError("Failed.");
        // yield to skip a frame
        yield return null;
    }
}
