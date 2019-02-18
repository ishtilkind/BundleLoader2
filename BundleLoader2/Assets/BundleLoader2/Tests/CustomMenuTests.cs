using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class CustomMenuTests {

    [Test]
    public void CustomMenuTestsSimplePasses() {
        // CustomMenu.DoSomething();
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator CustomMenuTestsWithEnumeratorPasses() {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
        // CustomMenu.DoSomething();
    }
}
