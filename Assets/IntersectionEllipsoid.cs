using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionEllipsoid : MonoBehaviour
{
	public void SetInertia(Vector3 I)
	{
		transform.localScale = I * 2;
		Debug.Log(string.Format("Ellipsoid extents st to {0}", I));
	}

    // Start is called before the first frame update
    void Start()
    {
		Renderer rend = GetComponent<Renderer>();
		rend.material.SetVector("_WorldPos", transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
