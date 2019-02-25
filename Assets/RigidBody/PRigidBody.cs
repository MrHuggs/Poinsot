using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PRigidBody : MonoBehaviour
{
	// Size of the intertia eillipsoid:
	public Vector3 Extents;
	
	// Angular velocity (world) coordinates:
	public Vector3 Omega;

	// Controlls apply the ELSolver during normalize:
	public bool ApplyAdjustment;

	// Diagonal values of the interia tensor:
	[HideInInspector]
	public Vector3 I;

	// Conserved values:
	[HideInInspector]
	public Vector3 L;
	[HideInInspector]
	public float Energy;

	ELSolver ELSolver;

	// Body --> World
	[HideInInspector]
	public Quaternion Orientation;

	public Vector3 BodyOmega()
	{
		var ir = Quaternion.Inverse(Orientation);
		var body_omega = ir * Omega;
		return body_omega;
	}

	public Vector3 BodyL()
	{
		var body_omega = BodyOmega();
		Vector3 body_l = new Vector3();
		body_l.x = body_omega.x * I.x;
		body_l.y = body_omega.y * I.y;
		body_l.z = body_omega.z * I.z;

		return body_l;
	}
	
	void UpdateOrientation(float dt)
	{
		Vector3 axis = Omega.normalized;
		// Omega is in radians/sec, but AngleAxis need degrees.
		float w = Omega.magnitude * Mathf.Rad2Deg;

		// There is a sign problem here...
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
		body_omega_dt.x = (I.y - I.z) * body_omega.y * body_omega.z / I.x;
		body_omega_dt.y = (I.z - I.x) * body_omega.z * body_omega.x / I.y;
		body_omega_dt.z = (I.x - I.y) * body_omega.x * body_omega.y / I.z;

		body_omega += body_omega_dt * dt;

		Omega = Orientation * body_omega;
	}

	private void Reset()
	{
		transform.localScale = Extents;

		Orientation = Quaternion.identity;

		// referencing http://scienceworld.wolfram.com/physics/MomentofInertiaEllipsoid.html
		I = new Vector3((Extents.y * Extents.y + Extents.z * Extents.z) * 1 / 5,
							  (Extents.x * Extents.x + Extents.z * Extents.z) * 1 / 5,
							  (Extents.x * Extents.x + Extents.y * Extents.y) * 1 / 5
							  );

		Debug.Log(string.Format("Inertia Values: ({0},{1},{2})", I.x, I.y, I.z));

		// Intially the body and world coordinates match:
		L.x = I.x * Omega.x;
		L.y = I.y * Omega.y;
		L.z = I.z * Omega.z;

		Energy = ELSolver.EnergyFromOrientation(Orientation, L, I);

		ELSolver = new ELSolver(Energy, L, I);

		Debug.Log(string.Format("Initial L {0} E {1}", L, Energy));
	}

	private void Awake()
	{
		Reset();
	}

	// Start is called before the first frame update
	void Start()
    {
		Debug.Log(string.Format("Initial L {0} E {1}", L, Energy));
		ShowParms();
	}

	private void ShowParms()
	{
		var body_omega = BodyOmega();
		var body_l = BodyL();

		var e = Vector3.Dot(body_l, body_omega) * .5f;

		Vector3 l = Orientation * body_l;
		float l2 = Vector3.Dot(body_l, body_l);

		Debug.Log(string.Format("L={0} L^2={1} |L|={2} E={3}", l, l2, Mathf.Sqrt(l2), e));
	}

	private void Normalize()
	{
		// Renormalize the orientation, and make sure angular momentum
		// and energy are being conserved.
		
		Orientation.Normalize();

		if (ApplyAdjustment)
			Orientation = ELSolver.AdjustOrientation(Orientation);

		var ir = Quaternion.Inverse(Orientation);
		var body_l = ir * L;

		Vector3 body_omega = new Vector3();
		body_omega.x = body_l.x / I.x;
		body_omega.y = body_l.y / I.y;
		body_omega.z = body_l.z / I.z;

		Omega = Orientation * body_omega;

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
