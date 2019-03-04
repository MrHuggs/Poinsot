using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
	[SerializeField] private Color Color = Color.red;

	[SerializeField] private float Length = 2;	// Include the shaft + head.
	[SerializeField] private float ShaftWidth = .1f;
	[SerializeField] private float HeadLength = .3f;
	[SerializeField] private float HeadWidth  = .2f;

	public void SetLength(float l)
	{
		Length = l;
		SetupChilden();
	}

	GameObject Shaft;
	GameObject Head;

	void SetupChilden()
	{
		float shaft_length = Length - HeadLength;
		Shaft.transform.localPosition = new Vector3(0, shaft_length / 2, 0);
		Shaft.transform.localScale = new Vector3(ShaftWidth, shaft_length / 2, ShaftWidth);
		Shaft.GetComponent<Renderer>().material.color = Color;

		Head.transform.localPosition = new Vector3(0, shaft_length, 0);
		Head.transform.localScale = new Vector3(HeadWidth, HeadLength, HeadWidth);
		Head.GetComponent<Renderer>().material.color = Color;
	}


	// Start is called before the first frame update
	void Start()
    {
		Shaft = transform.Find("Shaft").gameObject;
		Head = transform.Find("Head").gameObject;

		SetupChilden();
	}

	private void Update()
	{
		
	}

}
