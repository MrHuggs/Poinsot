using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BinetSetup : MonoBehaviour
{

	public GameObject TargetBody;

	GameObject EnergySphere;
	IntersectionEllipsoid IntersectionEllipsoid;
	FollowAngularMomentum FollowAngularMomentum;
	GameObject BinetCamera;

	// Start is called before the first frame update
	void Start()
    {
		PRigidBody body = TargetBody.GetComponent<PRigidBody>();

		EnergySphere = transform.Find("EnergySphere").gameObject;
		IntersectionEllipsoid = transform.Find("IntersectionEllipsoid").gameObject.GetComponent<IntersectionEllipsoid>();
		FollowAngularMomentum = transform.Find("FollowAngularMomentum").gameObject.GetComponent<FollowAngularMomentum>();

		float m2 = Vector3.Dot(body.AngularMomentum, body.AngularMomentum);
		float scale = 2 * body.Energy / m2;

		Debug.Log(string.Format("Energy: {0} Momentum {1} Scale {2} Inertia Tensor {3}", body.Energy, m2, scale, body.Inertia));

		IntersectionEllipsoid.SetInertia(body.Inertia * scale);

		FollowAngularMomentum.Target = body;
		FollowAngularMomentum.Scale = 1 / Mathf.Sqrt(m2);

		BinetCamera = transform.Find("BinetCamera").gameObject;
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
