using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderEllipsoid : MonoBehaviour
{
	Rigidbody rb;

	public Vector3 Omega;
	public Vector3 Extents;

	// Start is called before the first frame update
	void Start()
    {
		transform.localScale = Extents;
		rb = GetComponent<Rigidbody>();
		//rb.angularVelocity = Omega;

		// referencing http://scienceworld.wolfram.com/physics/MomentofInertiaEllipsoid.html

		rb.inertiaTensor = new Vector3((Extents.y * Extents.y + Extents.z * Extents.z) * 1 / 5,
										(Extents.x * Extents.x + Extents.z * Extents.z) * 1 / 5,
										(Extents.x * Extents.x + Extents.z * Extents.z) * 1 / 5
										);

		rb.AddTorque(Omega * 100);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
