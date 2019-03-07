using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoinsotSetup : MonoBehaviour
{
	public PRigidBody Body;

	// Normalized angular velocity vector:
	Vector3 L_dir;
	// The Poinsot display is in angular velecity space. The scale is set so that
	// MasterScale = 1 unit in the Unity display. The value is chose to be the projection
	// of the angular velocity along angular momentum direction:
	float MasterScale;

	GameObject UpperPlane;
	GameObject LowerPlane;

	Arrow L;
	Arrow Omega;

	GameObject AngularVelocityTrail;

	GameObject InertiaEllipsoid;
	GameObject Camera;

	// Start is called before the first frame update
	void Start()
    {
		UpperPlane = transform.Find("UpperPlane").gameObject;
		LowerPlane = transform.Find("LowerPlane").gameObject;
		L = transform.Find("L").gameObject.GetComponent<Arrow>();
		Omega = transform.Find("Omega").gameObject.GetComponent<Arrow>();
		AngularVelocityTrail = transform.Find("AngularVelocityTrail").gameObject;
		InertiaEllipsoid = transform.Find("InertiaEllipsoid").gameObject;
		Camera = transform.Find("Camera").gameObject;

		// Calculate the master scale based on the state of the body:
		L_dir = Body.L.normalized;
		MasterScale = Vector3.Dot(L_dir, Body.Omega);

		// The angular momentum is just a directional display, so its length is arbitrarty.
		L.SetLength(1.5f);

		Debug.Log(string.Format("MasterScale {0},", MasterScale));

		OrientCamera();
		OrientPlanes();
		PositionPlanes();
	}

	void OrientCamera()
	{
		var ctt = InertiaEllipsoid.transform.position - Camera.transform.position;
		ctt = ctt.normalized;

		var up = Body.L;
		up = up - Vector3.Dot(up, ctt) * ctt;

		//Camera.transform.LookAt(InertiaEllipsoid.transform, up);

		Camera.GetComponent<OrbitCamera>().UpVector = up;
		//Camera.GetComponent<OrbitCamera>().enabled = true;
	}

	void _OrientCamera()
	{
		var cam_dist = Camera.transform.localPosition.magnitude;

		var lcs = Camera.transform.rotation * Body.L;
		lcs.z = 0;

		Quaternion q = new Quaternion();
		q.SetFromToRotation(Vector3.up, lcs);
		Camera.transform.localRotation = q;

		var pos = q * (Vector3.back * cam_dist);

		Camera.transform.localPosition = pos;

		//Camera.GetComponent<OrbitCamera>().enabled = true;
	}


	void OrientPlanes()
	{
		// We want the planes to be perpendicular to the angular momentum of the body.

		Quaternion q = new Quaternion();

		q.SetFromToRotation(Vector3.down, Body.L);
		UpperPlane.transform.rotation = q;

		q.SetFromToRotation(Vector3.up, Body.L);
		LowerPlane.transform.localRotation = q;

		L.transform.localRotation = q;
	}

	void PositionPlanes()
	{
		// The upper and low invariable planes are 1 unit along the angular momentum
		// direction.
		UpperPlane.transform.localPosition = L_dir;
		LowerPlane.transform.localPosition = -L_dir;
	}

	// Helper for checking ellipsoid values:
	static float square(float x) { return x * x;  }
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

		Omega.transform.localRotation = q;
		Omega.SetLength(Body.Omega.magnitude / MasterScale);

		AngularVelocityTrail.transform.localPosition = Body.Omega / MasterScale;

		// For a given angular velocity in body-fixed-corrdinates, the energy is:
		// 2E = w_x^2Ix + w_y^2Iy+w_z^2I_z

		float E = Body.Energy;
		Vector3 ellipsoid_scale = new Vector3(Mathf.Sqrt(2 * E / Body.I.x),
											  Mathf.Sqrt(2 * E / Body.I.y),
											  Mathf.Sqrt(2 * E / Body.I.z));

		//Debug.Log(string.Format("ellipsoid_scale ({0},{1},{2})", ellipsoid_scale.x, ellipsoid_scale.y, ellipsoid_scale.z));

		ellipsoid_scale *= 1 / MasterScale;

		InertiaEllipsoid.transform.localScale = ellipsoid_scale * 2;
		InertiaEllipsoid.transform.rotation = Body.Orientation;

		PositionPlanes();

		//float f = EvalEllipsoid(Body.BodyOmega(), ellipsoid_scale);
		//Debug.Log(string.Format("Curr ellipse eq value: {0}", f));

	}
}
