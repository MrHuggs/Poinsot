using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoinsotSetup : MonoBehaviour
{
	public PRigidBody Body;

	GameObject UpperPlane;
	GameObject LowerPlane;

	GameObject AngularMomentum;
	GameObject AngularVelocity;

	GameObject InertiaEllipsoid;

	// Start is called before the first frame update
	void Start()
    {
		UpperPlane = transform.Find("UpperPlane").gameObject;
		LowerPlane = transform.Find("LowerPlane").gameObject;

		AngularMomentum = transform.Find("AngularMomentum").gameObject;
		AngularVelocity = transform.Find("AngularVelocity").gameObject;

		InertiaEllipsoid = transform.Find("InertiaEllipsoid").gameObject;

		OrientPlanes();
		PositionPlanes();

	}

	void OrientPlanes()
	{
		// We want the planes to be perpendicular to the angular momentum of the body.

		Quaternion q = new Quaternion();

		q.SetFromToRotation(Vector3.down, Body.L);
		UpperPlane.transform.rotation = q;
		AngularMomentum.transform.localRotation = q;

		q.SetFromToRotation(Vector3.up, Body.L);
		LowerPlane.transform.localRotation = q;
	}

	void PositionPlanes()
	{
		var L_dir = Body.L.normalized;

		float omega_projection = Vector3.Dot(L_dir, Body.Omega);

		Debug.Log(string.Format("omega_projection {0}", omega_projection));

		Vector3 pos = new Vector3();
		pos.y = omega_projection;

		//UpperPlane.transform.position = pos;

		pos.y = -omega_projection;
		//LowerPlane.transform.position = pos;

	}

	static float square(float x) { return x * x;  }

	// Update is called once per frame
	static float EvalEllipsoid(Vector3 coordinate, Vector3 ellipse_vals)
	{
		float res=	square(coordinate.x / ellipse_vals.x) +
					square(coordinate.y / ellipse_vals.y) +
					square(coordinate.z / ellipse_vals.z);
		return res;
	}



	void Update()
    {

		Quaternion q = new Quaternion();
		q.SetFromToRotation(Vector3.up, Body.Omega);

		AngularVelocity.transform.localRotation = q;

		// For a given angular velocity in body-fixed-corrdinates, the energy is:
		// 2E = w_x^2Ix + w_y^2Iy+w_z^2I_z

		float E = Body.Energy;
		Vector3 ellipsoid_scale = new Vector3(Mathf.Sqrt(2 * E / Body.I.x),
											  Mathf.Sqrt(2 * E / Body.I.y),
											  Mathf.Sqrt(2 * E / Body.I.z));

		Debug.Log(string.Format("ellipsoid_scale ({0},{1},{2})", ellipsoid_scale.x, ellipsoid_scale.y, ellipsoid_scale.z));

		var L_dir = Body.L.normalized;
		float omega_projection = Vector3.Dot(L_dir, Body.Omega);

		ellipsoid_scale *= 1 / omega_projection;

		InertiaEllipsoid.transform.localScale = ellipsoid_scale * 2;
		InertiaEllipsoid.transform.rotation = Body.Orientation;

		PositionPlanes();

		float f = EvalEllipsoid(Body.BodyOmega(), ellipsoid_scale);
		Debug.Log(string.Format("Curr ellipse eq value: {0}", f));

	}
}
