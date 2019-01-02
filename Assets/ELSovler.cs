using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ELSolver
{
	// Solver to help normalize orientation so that L and E are conserved.
	// Idea is that we have a current orientation which might not convserve L/E, but which his close.
	// We will apply small rotations to bring them back into balance.
	// Basic ideal is gradiant descent.

	Vector3 AngularMomentum;
	Vector3 Inertia;
	float Energy;

	public ELSolver(float energy, Vector3 l, Vector3 inertia)
	{
		AngularMomentum = l;
		Energy = energy;
		Inertia = inertia;
	}

	public static float EnergyFromOrientation(Quaternion o, Vector3 L, Vector3 I)
	{
		var ir = Quaternion.Inverse(o);

		Vector3 body_l = ir * L;

		float e = body_l.x * body_l.x / I.x +
				  body_l.y * body_l.y / I.y +
				  body_l.z * body_l.z / I.z;

		e *= .5f;

		return e;
	}
	float EnergyFromOrientation(Quaternion o)
	{
		return EnergyFromOrientation(o, AngularMomentum, Inertia);
	}

	// List of potential perturbations:
	const float AdjustAngle = .05f;
	static Quaternion[] Adjustments =
	{
			Quaternion.AngleAxis(AdjustAngle, Vector3.left),
			Quaternion.AngleAxis(AdjustAngle, Vector3.up),
			Quaternion.AngleAxis(AdjustAngle, Vector3.forward)
	};

	static Vector3[] AdjustmentDirs =
	{
			Vector3.left,
			Vector3.up,
			Vector3.forward
	};

	// Perform a single iteration: Find the energy delta, measure the gradient WRT
	// each perturbation. Find the one with the largest affect and apply a scaled adjustment
	// to the orientation.
	//
	Quaternion Interate(Quaternion cur_orientation)
	{
		float ecur = EnergyFromOrientation(cur_orientation);
		float del = Energy - ecur;

		float dm = 0;
		float delbest = 0;
		int ibest = 0;
		for (int im = 0; im < Adjustments.Length; im++)
		{
			var o = Adjustments[im] * cur_orientation;
			var ep = EnergyFromOrientation(o);
			var cd = ep - ecur;
			if (Mathf.Abs(cd) > dm)
			{
				ibest = im;
				delbest = cd;
				dm = Mathf.Abs(cd);
			}
		}

		float factor = del / delbest;
		factor = Mathf.Clamp(factor, -4, 4);
		float angle = AdjustAngle * factor;

		var adjust = Quaternion.AngleAxis(angle, AdjustmentDirs[ibest]);

		var next_orientation = adjust * cur_orientation;

		return next_orientation;
	}

	public Quaternion AdjustOrientation(Quaternion initial)
	{
		Quaternion cur = initial;
		int niter = 0;

		float ecur_i = EnergyFromOrientation(cur);
		float del_i = Energy - ecur_i;

		for (; ; )
		{
			float ecur = EnergyFromOrientation(cur);
			float del = Energy - ecur;

			if (Mathf.Abs(del) < Energy * .0001f)
				break;

			var next = Interate(initial);


			float ef = EnergyFromOrientation(next);
			float delf = Energy - ef;
			Debug.Assert(delf <= del);

			cur = next;

			niter++;
			if (niter > 6)
				break;
		}

		if (niter > 0)
		{
			float ecur_f = EnergyFromOrientation(cur);
			float del_f = Energy - ecur_f;

			Debug.Log(string.Format("ELSolver changed del from {0} to {1} out of {2} in {3} iter.",
						del_i, del_f, Energy, niter));
		}



		return cur;
	}
}
