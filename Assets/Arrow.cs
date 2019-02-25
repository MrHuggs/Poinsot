using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
	public Color Color = Color.red;
	public float Length = 2;
	public float ShaftWidth = .1f;
	public float HeadLength = .3f;
	public float HeadWidth  = .2f;

    // Start is called before the first frame update
    void Start()
    {
		var shaft = transform.Find("Shaft").gameObject;
		shaft.transform.localPosition = new Vector3(0, Length / 2, 0);
		shaft.transform.localScale = new Vector3(ShaftWidth, Length / 2, ShaftWidth);
		shaft.GetComponent<Renderer>().material.color = Color;
		
		var head = transform.Find("Head").gameObject;
		head.transform.localPosition = new Vector3(0, Length, 0);
		head.transform.localScale = new Vector3(HeadWidth, HeadLength, HeadWidth);
		head.GetComponent<Renderer>().material.color = Color;
	}

}
