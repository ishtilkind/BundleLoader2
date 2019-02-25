using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TutorialTestScript {

    [Test]
     [Combinatorial]
     public void CombinatorialTest(
         [Values(1,2)] int a,
         [Values("A", "B")] string b,
         [Values(true, false)] bool c
         ) 
     {
         Debug.LogWarningFormat("Combinatorial Test {0}, {1}, {2}", a, b, c);
     }

     [Test]
     [Pairwise]
     public void PairwiseTest(
         [Values(1,2)] int a,
         [Values("A", "B")] string b,
         [Values(true, false)] bool c
     ) 
     {
         Debug.LogWarningFormat("Pairwise Test {0}, {1}, {2}", a, b, c);
     }
     
     [Test]
     [Sequential]
     public void SequentialTest(
         [Values(1,2)] int a,
         [Values("A", "B")] string b,
         [Values(true, false)] bool c
     ) 
     {
         Debug.LogWarningFormat("Sequential Test {0}, {1}, {2}", a, b, c);
     }

     [Test]
     [Ignore("This test is ignored")]
     public void EditorTestSimplePasses()
     {
         Debug.LogError("Should be ignored.");
     }
     
     
     [Test]
     [UnityPlatform(
         exclude = new RuntimePlatform[] {},
         include = new RuntimePlatform[] { RuntimePlatform.WindowsEditor, RuntimePlatform.WindowsPlayer}
         )]
     public void IncludeWindowsPlatformTest()
     {
//         Debug.LogError("Should be ignored.");
     }
     
          
     [Test]
     [UnityPlatform(
         include = new RuntimePlatform[] { RuntimePlatform.WebGLPlayer},
         exclude = new RuntimePlatform[] { RuntimePlatform.WindowsEditor, RuntimePlatform.WindowsPlayer}
     )]
     public void ExcludeWindowsPlatformTest()
     {
//         Debug.LogError("Should be ignored.");
     }
     
#if UNITY_WEBGL    
     [Test]
     public void WebGlTestSimplePasses()
     {
         Debug.Log("Should be available for WebGL only.");
     }
#endif
     
    [Test]
    [Repeat(3)]
    public void RepeatedTestSimplePasses()
    {
        Debug.LogWarning("Repeated Test");
    }

    private static int retryIneration = 0;
     
    [Test]
    [Retry(3)]
    public void RetryTestSimplePasses()
    {
        Debug.LogWarning("Retry Test");
        retryIneration++;

        if (retryIneration < 3)
        {
            Assert.That(false);            
        }
        else
        {
            Assert.That(true);
        }
    }
     
     
    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    [Description("This is my description.")]
    [Author("Igor Shtilkind", "igor.shtilkind@ngc.com")]
    [Category("TRIPSS")]
    public IEnumerator TutorialTestScriptWithEnumeratorPasses() {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }
}
