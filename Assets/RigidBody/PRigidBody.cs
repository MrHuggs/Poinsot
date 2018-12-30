using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PRigidBody : MonoBehaviour
{
	// Size of the intertia eillipsoid:
	public Vector3 Extents;
	
	// Angular velocity (world) coordinates:
	public Vector3 Omega;

	// Diagonal values of the interia tensor:
	[HideInInspector]
	public Vector3 Inertia;

	// Conserved values:
	[HideInInspector]
	public Vector3 AngularMomentum;
	[HideInInspector]
	public float Energy;

	// Body --> World
	[HideInInspector]
	public Quaternion Orientation;
	
	void UpdateOrientation(float dt)
	{
		Vector3 axis = Omega.normalized;
		// Omega is in radians/sec, but AngleAxis need degrees.
		float w = Omega.magnitude * Mathf.Rad2Deg;

		var inc = Quaternion.AngleAxis(w * dt, axis);

		Orientation = inc * Orientation;
	}

	void UpdateOmega(float dt)
	{
		var ir = Quaternion.Inverse(Orientation);

		var body_omega = ir * Omega;

		Vector3 body_omega_dt = new Vector3();

		// Euler's equations for torque free motion.
		// Even the the Unity coordinate system is left-handed, this still works.
		body_omega_dt.x = (Inertia.y - Inertia.z) * body_omega.y * body_omega.z / Inertia.x;
		body_omega_dt.y = (Inertia.z - Inertia.x) * body_omega.z * body_omega.x / Inertia.y;
		body_omega_dt.z = (Inertia.x - Inertia.y) * body_omega.x * body_omega.y / Inertia.z;

		body_omega += body_omega_dt * dt;

		Omega = Orientation * body_omega;
	}

	private void Awake()
	{
		transform.localScale = Extents;

		Orientation = Quaternion.identity;

		// referencing http://scienceworld.wolfram.com/physics/MomentofInertiaEllipsoid.html
		Inertia = new Vector3((Extents.y * Extents.y + Extents.z * Extents.z) * 1 / 5,
							  (Extents.x * Extents.x + Extents.z * Extents.z) * 1 / 5,
							  (Extents.x * Extents.x + Extents.y * Extents.y) * 1 / 5
							  );


		// Intially the body and world coordinates match:
		AngularMomentum.x = Inertia.x * Omega.x;
		AngularMomentum.y = Inertia.y * Omega.y;
		AngularMomentum.z = Inertia.z * Omega.z;

		Energy = Vector3.Dot(AngularMomentum, Omega) * .5f;

		Debug.Log(string.Format("Initial L {0} E {1}", AngularMomentum, Energy));
	}

	// Start is called before the first frame update
	void Start()
    {
		transform.localScale = Extents;

		Orientation = Quaternion.identity;

		// Intially the body and world coordinates match:
		AngularMomentum.x = Inertia.x * Omega.x;
		AngularMomentum.y = Inertia.y * Omega.y;
		AngularMomentum.z = Inertia.z * Omega.z;

		Energy = Vector3.Dot(AngularMomentum, Omega) * .5f;

		Debug.Log(string.Format("Initial L {0} E {1}", AngularMomentum, Energy));
		ShowParms();
	}

	private void ShowParms()
	{
		var ir = Quaternion.Inverse(Orientation);
		var body_omega = ir * Omega;
		Vector3 body_l = new Vector3();
		body_l.x = body_omega.x * Inertia.x;
		body_l.y = body_omega.y * Inertia.y;
		body_l.z = body_omega.z * Inertia.z;

		var e = Vector3.Dot(body_l, body_omega) * .5f;

		Vector3 l = Orientation * body_l;
		float l2 = Vector3.Dot(body_l, body_l);


		Debug.Log(string.Format("L {0} L^2 {1} E {2}", l, l2, e));
	}

	private void Normalize()
	{
		// Renormalize the orientation, and make sure angular momentum
		// and energy are being conserved.
		
		Orientation.Normalize();

		var ir = Quaternion.Inverse(Orientation);
		var body_l = ir * AngularMomentum;

		Vector3 body_omega = new Vector3();
		body_omega.x = body_l.x / Inertia.x;
		body_omega.y = body_l.y / Inertia.y;
		body_omega.z = body_l.z / Inertia.z;

		// Scale angular velocity to preserve kinetic energy:
		var cur_energy = Vector3.Dot(body_l, body_omega) * .5f;

		var sf = Mathf.Sqrt(Energy / cur_energy);
		body_omega *= sf;

		var corrected_omega = Orientation * body_omega;
		Omega = corrected_omega;
	}

	private void FixedUpdate()
	{
		const float target_dt = .001f;
		for (float elapsed = 0; elapsed < Time.fixedDeltaTime; elapsed += target_dt)
		{
			UpdateOmega(target_dt);
			UpdateOrientation(target_dt);
		}
		Normalize();
		//ShowParms();
	}

	// Update is called once per frame
	void Update()
    {
		transform.rotation = Orientation;
        
    }
}
