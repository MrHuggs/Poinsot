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
	public bool ApplyAdjustment = false;
	#endregion

	// Size of the inertia ellipsoid:
	[HideInInspector]
	DVector3 Extents;

    // Diagonal values of the inertia tensor:
    [HideInInspector]
	public DVector3 I;

	// Angular velocity used when simulation was last reset:
	[HideInInspector]
	public DVector3 InitialOmega;

	// Current angular velocity in world coordinates:
	[HideInInspector]
	public DVector3 Omega
	{
		get { return Orientation * _BodyOmega; }
	}

	// Current angular velocity in body coordinates:
	[HideInInspector]
	public DVector3 _BodyOmega;

	public DVector3 BodyOmega
	{
		get { return _BodyOmega; }
		private set { _BodyOmega = value; }
	}

	// Conserved values:
	[HideInInspector]
	public DVector3 L;
	[HideInInspector]
	public double Energy;

	// Solver used to adjust orientation to preserve I and Ke. Controlled by ApplyAdjustment.
	ELSolver ELSolver;

	// Number of updates - just for logging.
	Int64 UpdateCount;

	// Body --> World transform
	[HideInInspector]
	public DQuaternion Orientation;
	[HideInInspector]
	public DQuaternion PrevOrientation;

	public DVector3 BodyL()
	{
		var body_l = new DVector3();
		body_l.x = BodyOmega.x * I.x;
		body_l.y = BodyOmega.y * I.y;
		body_l.z = BodyOmega.z * I.z;

		return body_l;
	}

	#region Get current values of things that should be concerved, but may not acutally be:
	public DVector3 CurrentL()
	{
		return Orientation * BodyL();
	}
	public double CurrentE()
	{
		var E = DVector3.Dot(BodyL(), BodyOmega) * .5;
		return E;
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

		PrevOrientation = Orientation;
		Orientation = inc * Orientation;
	}

	void UpdateOmega(targ_type dt)
	{
		var body_omega = BodyOmega;

		DVector3 body_omega_dt = new DVector3();
		// Euler's equations for torque free motion.
		// Even the the Unity coordinate system is left-handed, this still works.
		// Uses explicit Euler integration...
		body_omega_dt.x = (I.y - I.z) * body_omega.y * body_omega.z / I.x;
		body_omega_dt.y = (I.z - I.x) * body_omega.z * body_omega.x / I.y;
		body_omega_dt.z = (I.x - I.y) * body_omega.x * body_omega.y / I.z;

		body_omega += body_omega_dt * dt;

		BodyOmega = body_omega;

		Debug.Assert(DVector3.Distance(body_omega, DQuaternion.Inverse(Orientation) * Omega) < 1.0e10);
	}


	void UpdateOmegaRK(targ_type dt)
	{
		// Uses Runge-Kutta integrator...
		targ_type[] omega_vec = { BodyOmega.x, BodyOmega.y, BodyOmega.z };

		targ_type[] f(targ_type t, targ_type[] body_omega_vec)
		{
			targ_type[] body_omega_dt = new targ_type[3];
			body_omega_dt[0] = (I.y - I.z) * body_omega_vec[1] * body_omega_vec[2] / I.x;
			body_omega_dt[1] = (I.z - I.x) * body_omega_vec[2] * body_omega_vec[0] / I.y;
			body_omega_dt[2] = (I.x - I.y) * body_omega_vec[0] * body_omega_vec[1] / I.z;

			return body_omega_dt;
		}

		targ_type[] next_omega = Assets.RigidBody.RK4.RK4vec(0, omega_vec, dt, f);

		_BodyOmega.x = next_omega[0];
		_BodyOmega.y = next_omega[1];
		_BodyOmega.z = next_omega[2];

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

	static bool ExtentsFromInertia(DVector3 inertia, out DVector3 extents)
	{
        // Note that is very easy to pick inertia values that don't coincide with an ellipsoidal
        // object.
        const targ_type min_extent2 = .01f * .01f;

        bool success = true;
        var ex2 = (inertia.y + inertia.z - inertia.x) * 5 / 2;
        if (ex2 < min_extent2)
        {
            ex2 = min_extent2;
            success = false;
        }
        var ey2 = (inertia.x + inertia.z - inertia.y) * 5 / 2;
        if (ey2 < min_extent2)
        {
            ey2 = min_extent2;
            success = false;
        }
        var ez2 = (inertia.x + inertia.y - inertia.z) * 5 / 2;
        if (ez2 < min_extent2)
        {
            ez2 = min_extent2;
            success = false;
        }

        extents = new DVector3(Math.Sqrt(ex2), Math.Sqrt(ey2), Math.Sqrt(ez2) );
		return success;
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

		BodyOmega = InitialOmega; // Initial BTW is identity

		bool b = ExtentsFromInertia(DVector3.FromUnity(inertia), out Extents);
        if (b)
            Debug.Assert((InertiaFromExtents(Extents) - DVector3.FromUnity(inertia)).magnitude < .001f);
        else
            Debug.Log(("Requested inertia values don't correspond to an ellipsoid object."));

        ApplyAdjustment = apply_adjustment;

		transform.localScale = DVector3.ToUnity(Extents);

		Orientation = DQuaternion.identity;
		PrevOrientation = DQuaternion.identity;

		I = DVector3.FromUnity(inertia);

		Debug.Log(string.Format("Inertia Values: ({0},{1},{2})", I.x, I.y, I.z));

		// Initially the body and world coordinates match:
		L.x = I.x * Omega.x;
		L.y = I.y * Omega.y;
		L.z = I.z * Omega.z;

		Energy = ELSolver.EnergyFromOrientation(Orientation, L, I);
		Debug.Assert(Energy == CurrentE());

		ELSolver = new ELSolver(Energy, L, I);
		UpdateCount = 0;

		DumpParameters();

		BodyParmsChanged?.Invoke();

    }


    // Start is called before the first frame update
    void Start()
    {
		DQuaternion.Test();


		SetParameters(StartingInertia, StartingOmega, ApplyAdjustment);
	}

	private void DumpParameters()
	{
		Debug.Log(string.Format("Initial L {0} E {1}", L, Energy));

		var body_omega = BodyOmega;
		var body_l = BodyL();

		var e = DVector3.Dot(body_l, body_omega) * .5f;

		DVector3 l = Orientation * body_l;
		double l2 = DVector3.Dot(body_l, body_l);

		Debug.Log(string.Format("t ={0} L={1} L^2={2} |L|={3} E={4}", Time.fixedDeltaTime * UpdateCount, l, l2, Math.Sqrt(l2), e));
	}

	private void Normalize()
	{
		// Renormalize the orientation, and make sure angular momentum
		// and energy are being conserved.
		Orientation.Normalize();

		if (ApplyAdjustment)
		{
			Orientation = ELSolver.AdjustOrientation(Orientation);

			var ir = DQuaternion.Inverse(Orientation);
			var body_l = ir * L;

			_BodyOmega.x = body_l.x / I.x;
			_BodyOmega.y = body_l.y / I.y;
			_BodyOmega.z = body_l.z / I.z;
		}
	}

	private void FixedUpdate()
	{
		// Some experimentation suggests .001 is the largest stepsize we can use.
		// .0001 doesn't seem to produce difference results
		// .01 givens significantly different ones.
		// If we use the Runge-Kutta integrator, .01 works just fine...
		const targ_type target_dt = .001f;
		for (targ_type elapsed = 0; elapsed < Time.fixedDeltaTime; elapsed += target_dt)
		{
			// Explicit Euler integrator:
			UpdateOmega(target_dt);
			// Runge-Kutta integrator:
			//UpdateOmegaRK(target_dt);
			UpdateOrientation(target_dt);
		}
		Normalize();

		if ((UpdateCount % 30) == 0)
			DumpParameters();

		UpdateCount++;
	}

	// Update is called once per frame
	void Update()
    {
		transform.rotation = DQuaternion.ToUnity(Orientation);
    }
}
