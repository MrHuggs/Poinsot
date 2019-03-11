using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BinetSetup : MonoBehaviour
{


	public PRigidBody Body;

	GameObject MomentumSphere;
	GameObject EnergyEllipsoid;
	BinetFollowL BinetFollowL;
	TrailRenderer BinetFollowTrail;
	BinetCamera BinetCamera;

	private void Awake()
	{
		MomentumSphere = transform.Find("MomentumSphere").gameObject;
		EnergyEllipsoid = transform.Find("EnergyEllipsoid").gameObject;
		BinetFollowL = transform.Find("BinetFollowL").gameObject.GetComponent<BinetFollowL>();
		BinetFollowTrail = BinetFollowL.transform.Find("Trail").gameObject.GetComponent<TrailRenderer>();

		BinetCamera = transform.Find("BinetCamera").gameObject.GetComponent<BinetCamera>();
		BinetCamera.SetTargets(MomentumSphere, Body);

		Body.BodyParmsChanged += SetParameters;
	}

	// Start is called before the first frame update
	void SetParameters()
    {

		// The overall scale of the display is set so that |L| = 1.
		float m2 = Vector3.Dot(Body.L, Body.L);
		float E = Body.Energy;

		Debug.Log(string.Format("Energy: {0} Momentum {1}", E, m2));

		// For given Lx, Ly, Lz, the energy is:
		//        Lx^2   Ly^2   Lz^2
		//  2E =  ---- + ---- +	----
		//        Ix	 Iy		Iz
		// Applying our scale, and turning this into an ellipsoid equation gives the following ellipse extents:
		Vector3 ellipsoid_scale = new Vector3(Mathf.Sqrt(E * Body.I.x * 2 / m2),
											  Mathf.Sqrt(E * Body.I.y * 2 / m2),
											  Mathf.Sqrt(E * Body.I.z * 2 / m2));

		Debug.Log(string.Format("Ellipsoid scale: {0},{1},{2}", ellipsoid_scale.x, ellipsoid_scale.y , ellipsoid_scale.z));

		// Need another factor of 2 in the final scale because a standard Unity sphere is radius .5:
		EnergyEllipsoid.transform.localScale = ellipsoid_scale * 2;

		BinetFollowL.Target = Body;
		BinetFollowL.Scale = 1 / Mathf.Sqrt(m2);


		BinetFollowTrail.enabled = false;
		BinetFollowTrail.Clear();
	}

	// Update is called once per frame
	void Update()
    {
		BinetFollowTrail.enabled = true;

	}
}
