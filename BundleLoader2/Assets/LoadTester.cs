using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTester : MonoBehaviour
{
	private AssetBundleLoader loader = new AssetBundleLoader("");

	// Use this for initialization
	void Start ()
	{
		loader.LoadAsset("bundle1", "Cube");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
