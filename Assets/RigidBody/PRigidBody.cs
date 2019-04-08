using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.DoubleMath;

#if targ_double
using targ_type = System.Double;
#else
using targ_type = System.Single;
#endif

public class PRigidBody : MonoBehaviour
{
	#region Inspector values

	// Diagonal values of the intertia tensor
	public Vector3 StartingInertia = new Vector3(.3f, .35f, .4f);

	// Starting angular velocity (world) coordinates:
	public Vector3 StartingOmega = new Vector3(.1f, 15, .1f);

	// Controls applying the ELSolver during normalize:
	public bool ApplyAdjustment;
	#endregion

	// Size of the intertia eillipsoid:
	[HideInInspector]
	DVector3 Extents;

	// Diagonal values of the interia tensor:
	[HideInInspector]
	public DVector3 I;

	// Angular velocity used when simulation was last reset:
	[HideInInspector]
	public DVector3 InitialOmega;

	// Current angular velocity in world coordinates:
	[HideInInspector]
	public DVector3 Omega;

	// Conserved values:
	[HideInInspector]
	public DVector3 L;
	[HideInInspector]
	public double Energy;

	// Solver used to adjust orientation to preserve I and Ke. Controlled by ApplyAdjustment.
	ELSolver ELSolver;

	// Body --> World transform
	[HideInInspector]
	public DQuaternion Orientation;

	#region Get values in the Body frame
	public DVector3 BodyOmega()
	{
		var ir = DQuaternion.Inverse(Orientation);
		var body_omega = ir * Omega;
		return body_omega;
	}

	public DVector3 BodyL()
	{
		var body_omega = BodyOmega();
		var body_l = new DVector3();
		body_l.x = body_omega.x * I.x;
		body_l.y = body_omega.y * I.y;
		body_l.z = body_omega.z * I.z;

		return body_l;
	}
	#endregion

	void UpdateOrientation(targ_type dt)
	{
		DVector3 axis = Omega.normalized;
		// Omega is in radians/sec, but AngleAxis need degrees.
		targ_type w = (targ_type) (Omega.magnitude * (360 / (2 * Math.PI)));

		// Note that in Unity, a positive angle of rotation is clockwise around the
		// axis of rotation. This is unusual....

		var inc = DQuaternion.AngleAxis(w * dt, axis);

		Orientation = inc * Orientation;
	}
	
	void UpdateOmega(targ_type dt)
	{
		var ir = DQuaternion.Inverse(Orientation);

		var body_omega = ir * Omega;

		DVector3 body_omega_dt = new DVector3();

		// Euler's equations for torque free motion.
		// Even the the Unity coordinate system is left-handed, this still works.
		body_omega_dt.x = (I.y - I.z) * body_omega.y * body_omega.z / I.x;
		body_omega_dt.y = (I.z - I.x) * body_omega.z * body_omega.x / I.y;
		body_omega_dt.z = (I.x - I.y) * body_omega.x * body_omega.y / I.z;

		body_omega += body_omega_dt * dt;

		Omega = Orientation * body_omega;
	}

	#region Ellipsoid helpers
	static DVector3 InertiaFromExtents(DVector3 extents)
	{
		// referencing http://scienceworld.wolfram.com/physics/MomentofInertiaEllipsoid.html
		var inertia = new DVector3((extents.y * extents.y + extents.z * extents.z) * 1 / 5,
							  (extents.x * extents.x + extents.z * extents.z) * 1 / 5,
							  (extents.x * extents.x + extents.y * extents.y) * 1 / 5
							  );
		return inertia;
	}

	static DVector3 ExtentsFromInertia(DVector3 inertia)
	{
		var extents = new DVector3(Math.Sqrt((inertia.y + inertia.z - inertia.x) * 5 /2),
								Math.Sqrt((inertia.x + inertia.z - inertia.y) * 5 / 2),
								Math.Sqrt((inertia.x + inertia.y - inertia.z) * 5 / 2)
							    );
		return extents;
	}
	#endregion


	public delegate void BodyParmsChangedHanlder();
	public event BodyParmsChangedHanlder BodyParmsChanged;

	void ConditionParameters(ref Vector3 inertia, ref Vector3 omega)
	{
		if (omega.magnitude < .001f)
			omega = new Vector3(0, .001f, 0);

		inertia.x = Mathf.Max(inertia.x, .001f);
		inertia.y = Mathf.Max(inertia.y, .001f);
		inertia.z = Mathf.Max(inertia.z, .001f);
	}


	public void SetParameters(Vector3 inertia, Vector3 omega, bool apply_adjustment)
	{
		ConditionParameters(ref inertia, ref omega);

		InitialOmega = DVector3.FromUnity(omega);
		Omega = InitialOmega;
		Extents = ExtentsFromInertia(DVector3.FromUnity(inertia));
		ApplyAdjustment = apply_adjustment;

		transform.localScale = DVector3.ToUnity(Extents);

		Orientation = DQuaternion.identity;

		I = DVector3.FromUnity(inertia);

		Debug.Log(string.Format("Inertia Values: ({0},{1},{2})", I.x, I.y, I.z));

		// Intially the body and world coordinates match:
		L.x = I.x * Omega.x;
		L.y = I.y * Omega.y;
		L.z = I.z * Omega.z;

		Energy = ELSolver.EnergyFromOrientation(Orientation, L, I);

		ELSolver = new ELSolver(Energy, L, I);

		DumpParameters();

		BodyParmsChanged?.Invoke();
	}


	// Start is called before the first frame update
	void Start()
    {
		DQuaternion.Test();


		SetParameters(StartingInertia, StartingOmega, ApplyAdjustment);

		Debug.Assert((InertiaFromExtents(Extents) - DVector3.FromUnity(StartingInertia)).magnitude < .001f);
	}

	private void DumpParameters()
	{
		Debug.Log(string.Format("Initial L {0} E {1}", L, Energy));

		var body_omega = BodyOmega();
		var body_l = BodyL();

		var e = DVector3.Dot(body_l, body_omega) * .5f;

		DVector3 l = Orientation * body_l;
		double l2 = DVector3.Dot(body_l, body_l);

		Debug.Log(string.Format("L={0} L^2={1} |L|={2} E={3}", l, l2, Math.Sqrt(l2), e));
	}

	private void Normalize()
	{
		// Renormalize the orientation, and make sure angular momentum
		// and energy are being conserved.
		
		Orientation.Normalize();

		if (ApplyAdjustment)
			Orientation = ELSolver.AdjustOrientation(Orientation);

		var ir = DQuaternion.Inverse(Orientation);
		var body_l = ir * L;

		DVector3 body_omega = new DVector3();
		body_omega.x = body_l.x / I.x;
		body_omega.y = body_l.y / I.y;
		body_omega.z = body_l.z / I.z;

		Omega = Orientation * body_omega;

	}

	private void FixedUpdate()
	{
		const targ_type target_dt = .00001f;
		for (targ_type elapsed = 0; elapsed < Time.fixedDeltaTime; elapsed += target_dt)
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
		transform.rotation = DQuaternion.ToUnity(Orientation);
    }
}
