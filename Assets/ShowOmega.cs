using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOmega : MonoBehaviour
{
	public GameObject Target;
	Rigidbody TargetBody;
    // Start is called before the first frame update
    void Start()
    {
		TargetBody = Target.GetComponent<Rigidbody>();

	}

    // Update is called once per frame
    void Update()
    {
		transform.position = Target.transform.position;

		var w = TargetBody.angularVelocity;
		
		Quaternion q = new Quaternion();
		q.SetFromToRotation(Vector3.up, TargetBody.angularVelocity);

		transform.rotation = q;

	}
}
