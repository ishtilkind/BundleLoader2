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
    [Ignore("This is an Example only")]
//    [TestSeverity((TestSeverityLevel.Minor))]
    public void CustomPropertyTest() {

    }
}
