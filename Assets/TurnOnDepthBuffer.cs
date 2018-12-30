using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnDepthBuffer : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{
		GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
	}
}