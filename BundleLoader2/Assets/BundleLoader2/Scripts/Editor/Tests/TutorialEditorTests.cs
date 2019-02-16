using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.Assertions.Must;

public class TutorialEditorTests {

    [Test]
    public void Random([Random(0, 10, 5)] int x) 
    {
        Debug.LogWarningFormat("Random: {0}", x);
    }

    [Test]
    public void Range([NUnit.Framework.Range(1, 10, 1)] int x) 
    {    
        Debug.LogWarningFormat("Random: {0}", x);
    }
         
    [TestCase(10, 5, 2)]
    [TestCase(5, 5, 1)]
    [TestCase(1, 1, 1)]
     public void DivisionTest(int a, int b, int c) 
     {    
         Debug.LogWarningFormat("Testing whether {0} / {1} = {2}", a, b, c);
         Assert.AreEqual(a / b, c);
     }
     
     [TestCaseSource("DivisionTest2Source")]
     public void DivisionTest2(int a, int b, int c) 
     {    
         Debug.LogWarningFormat("Testing whether {0} / {1} = {2}", a, b, c);
         Assert.AreEqual(a / b, c);
     }

     private static readonly object[] DivisionTest2Source =
     {
         new object[] {10, 5, 2},
         new object[] {5, 5, 1},
         new object[] {1, 1, 1}
     };
     
     [DatapointSource]
     public float[] values = new float[] { -1f, 0f, 1f, 11f };

     [Theory]
     public void SquareRootDefinition(float value)
     {
         Assume.That(value >= 0f);
         
         Debug.LogWarningFormat("SquareRootDefinition: {0}", value);
         var root = Mathf.Sqrt(value);
         Assert.That(root >= 0);
         Assert.That(root * root, Is.EqualTo(value).Within(0.000001f));

     }
     
     public static float[] values2 = new float[] { 0f, 1f, 11f };

     [Test]
     public void SquareRootDefinition2([ValueSource("values2")] float value)
     {
         Debug.LogWarningFormat("SquareRootDefinition2: {0}", value);
         var root = Mathf.Sqrt(value);
         Assert.That(root >= 0);
         Assert.That(root * root, Is.EqualTo(value).Within(0.000001f));

     }
     
     [Test]
     [MaxTime(2000)]
     [Timeout(2000)] // Timeout only seems to work in play mode tests
     public void MaxTimeTest()
     {
         System.Threading.Thread.Sleep(1500);

     }
}

