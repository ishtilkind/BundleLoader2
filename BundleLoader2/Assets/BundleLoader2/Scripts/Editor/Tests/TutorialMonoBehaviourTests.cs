using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using NG.TRIPSS.CORE;

public class TutorialMonoBehaviourTests {


    [UnityTest]
    public IEnumerator TestCharacterInventoryMonoBehaviour() {

        yield return null;
       // CommunicationManager.Instance.Awake();
       var go = new GameObject();
       var comms = go.AddComponent<CommunicationManager>();
       comms.Awake();
       
       LogAssert.Expect(LogType.Log, "Loading Internals...");
    }
}
