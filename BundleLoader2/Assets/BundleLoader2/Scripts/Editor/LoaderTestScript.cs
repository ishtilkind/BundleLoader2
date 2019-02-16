using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using NG.TRIPSS.CORE;

public class LoaderTestScript {

    [Test]
    public void Loader_LoadModel_ContainerId_1()
    {

        IDataSource source = null;
        // Use the Assert class to test conditions.
        Loader loader = new Loader("", source, null);
        var result = loader.LoadModel("1");
        Assert.True(result);
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator LoaderTestScriptWithEnumeratorPasses() {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }
}
