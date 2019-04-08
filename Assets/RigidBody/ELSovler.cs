
using UnityEngine;
using System;
using Assets.DoubleMath;

public class ELSolver
{
	// Solver to help normalize orientation so that L and E are conserved.
	// Idea is that we have a current orientation which might not convserve L/E, but which his close.
	// We will apply small rotations to bring them back into balance.
	// Basic ideal is gradiant descent.

	DVector3 AngularMomentum;
	DVector3 Inertia;
	double Energy;

	public ELSolver(double energy, DVector3 l, DVector3 inertia)
	{
		AngularMomentum = l;
		Energy = energy;
		Inertia = inertia;
	}

	public static double EnergyFromOrientation(DQuaternion o, DVector3 L, DVector3 I)
	{
		var ir = DQuaternion.Inverse(o);

		DVector3 body_l = ir * L;

		double e = body_l.x * body_l.x / I.x +
				  body_l.y * body_l.y / I.y +
				  body_l.z * body_l.z / I.z;

		e *= .5f;

		return e;
	}
	double EnergyFromOrientation(DQuaternion o)
	{
		return EnergyFromOrientation(o, AngularMomentum, Inertia);
	}

	// List of potential perturbations:
	const double AdjustAngle = .05f;
	static DQuaternion[] Adjustments =
	{
			DQuaternion.AngleAxis(AdjustAngle, DVector3.left),
			DQuaternion.AngleAxis(AdjustAngle, DVector3.up),
			DQuaternion.AngleAxis(AdjustAngle, DVector3.forward)
	};

	static DVector3[] AdjustmentDirs =
	{
			DVector3.left,
			DVector3.up,
			DVector3.forward
	};

	// Perform a single iteration: Find the energy delta, measure the gradient WRT
	// each perturbation. Find the one with the largest effect and apply a scaled adjustment
	// to the orientation.
	//
	DQuaternion Iterate(DQuaternion cur_orientation)
	{
		double ecur = EnergyFromOrientation(cur_orientation);
		double del = Energy - ecur;

		double dm = 0;
		double delbest = 0;
		int ibest = 0;
		for (int im = 0; im < Adjustments.Length; im++)
		{
			var o = Adjustments[im] * cur_orientation;
			var ep = EnergyFromOrientation(o);
			var cd = ep - ecur;
			if (Math.Abs(cd) > dm)
			{
				ibest = im;
				delbest = cd;
				dm = Math.Abs(cd);
			}
		}

		double factor = del / delbest;

		if (factor < -4) factor = -4;			// There is no clamp for doubles.
		else if (factor > 4) factor = 4;

		double angle = AdjustAngle * factor;

		var adjust = DQuaternion.AngleAxis(angle, AdjustmentDirs[ibest]);

		var next_orientation = adjust * cur_orientation;

		return next_orientation;
	}

	public DQuaternion AdjustOrientation(DQuaternion initial)
	{
		DQuaternion cur = initial;
		int niter = 0;

		double ecur_i = EnergyFromOrientation(cur);
		double del_i = Energy - ecur_i;

		for (; ; )
		{
			double ecur = EnergyFromOrientation(cur);
			double del = Energy - ecur;

			if (Math.Abs(del) < Energy * .0001f)
				break;

			var next = Iterate(initial);


			double ef = EnergyFromOrientation(next);
			double delf = Energy - ef;
			Debug.Assert(delf <= del);

			cur = next;

			niter++;
			if (niter > 6)
				break;
		}

		if (niter > 0)
		{
			double ecur_f = EnergyFromOrientation(cur);
			double del_f = Energy - ecur_f;

			//Debug.Log(string.Format("ELSolver changed del from {0} to {1} out of {2} in {3} iter.",
			//			del_i, del_f, Energy, niter));
		}



		return cur;
	}
}
