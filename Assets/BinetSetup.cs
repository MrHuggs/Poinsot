using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BinetSetup : MonoBehaviour
{

	public GameObject TargetBody;

	GameObject MomentumSphere;
	GameObject EnergyEllipsoid;
	FollowAngularMomentum FollowAngularMomentum;
	BinetCamera BinetCamera;

	// Start is called before the first frame update
	void Start()
    {
		PRigidBody body = TargetBody.GetComponent<PRigidBody>();

		MomentumSphere = transform.Find("MomentumSphere").gameObject;
		EnergyEllipsoid = transform.Find("EnergyEllipsoid").gameObject;
		FollowAngularMomentum = transform.Find("FollowAngularMomentum").gameObject.GetComponent<FollowAngularMomentum>();

		// The overall scale of the display is set so that |L| = 1.
		float m2 = Vector3.Dot(body.L, body.L);
		float E = body.Energy;

		Debug.Log(string.Format("Energy: {0} Momentum {1}", E, m2));

		// For given Lx, Ly, Lz, the energy is:
		//        Lx^2   Ly^2   Lz^2
		//  2E =  ---- + ---- +	----
		//        Ix	 Iy		Iz
		// Applying our scale, and turning this into an ellipsoid equation gives the following ellipse extents:
		Vector3 ellipsoid_scale = new Vector3(Mathf.Sqrt(E * body.I.x * 2 / m2),
											  Mathf.Sqrt(E * body.I.y * 2 / m2),
											  Mathf.Sqrt(E * body.I.z * 2 / m2));

		Debug.Log(string.Format("Ellipsoid scale: {0},{1},{2}", ellipsoid_scale.x, ellipsoid_scale.y , ellipsoid_scale.z));

		// Need another factor of 2 in the final scale because a standard Unity sphere is radius .5:
		EnergyEllipsoid.transform.localScale = ellipsoid_scale * 2;

		FollowAngularMomentum.Target = body;
		FollowAngularMomentum.Scale = 1 / Mathf.Sqrt(m2);

		BinetCamera = transform.Find("BinetCamera").gameObject.GetComponent<BinetCamera>();
		BinetCamera.SetTargets(MomentumSphere, body);
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
