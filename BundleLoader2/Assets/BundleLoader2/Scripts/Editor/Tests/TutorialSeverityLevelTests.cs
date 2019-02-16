using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TutorialSeverityLevelTests {


    public enum TestSeverityLevel
    {
        Minor, 
        Major
    }

    public class TestSeverityAttribute : NUnit.Framework.PropertyAttribute
    {
        public TestSeverityAttribute(TestSeverityLevel level) : base()
        {
            Debug.Log(($"Test severity: {level}"));
        }
    }

    [Test]
    [TestSeverity((TestSeverityLevel.Minor))]
    public void CustomPropertyTest() {

    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator TutorialSeverityLevelTestsWithEnumeratorPasses() {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }
}
