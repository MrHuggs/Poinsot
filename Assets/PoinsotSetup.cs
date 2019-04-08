using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.DoubleMath;

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

	Arrow L;
	Arrow Omega;

	TrailRenderer AngularVelocityTrail;

	GameObject InertiaEllipsoid;
	GameObject Camera;
	Vector3 InitialCameraPosition;

	void Awake()
	{
		UpperPlane = transform.Find("UpperPlane").gameObject;
		L = transform.Find("L").gameObject.GetComponent<Arrow>();
		Omega = transform.Find("Omega").gameObject.GetComponent<Arrow>();
		AngularVelocityTrail = transform.Find("AngularVelocityTrail").gameObject.GetComponent<TrailRenderer>();
		InertiaEllipsoid = transform.Find("InertiaEllipsoid").gameObject;
		Camera = transform.Find("Camera").gameObject;

		// Save off the cameria postion as set in the editor. After every reset, we'll return
		// to this location:
		InitialCameraPosition = Camera.transform.position;

		Body.BodyParmsChanged += SetParameters;
	}


	// Start is called before the first frame update
	void SetParameters()
	{
		// Calculate the master scale based on the state of the body:
		L_dir = DVector3.ToUnity(Body.L.normalized);
		MasterScale = Vector3.Dot(L_dir, DVector3.ToUnity(Body.Omega));

		// The angular momentum is just a directional display, so its length is arbitrarty.
		L.SetLength(1.5f);

		Debug.Log(string.Format("MasterScale {0},", MasterScale));

		OrientCamera();
		OrientPlanesAndL();
		PositionPlanes();

		// For a given angular velocity in body-fixed-corrdinates, the energy is:
		// 2E = w_x^2Ix + w_y^2Iy+w_z^2I_z
		var body_i = DVector3.ToUnity(Body.I);
		float E = (float) Body.Energy;
		Vector3 ellipsoid_scale = new Vector3(Mathf.Sqrt(2 * E / body_i.x),
											  Mathf.Sqrt(2 * E / body_i.y),
											  Mathf.Sqrt(2 * E / body_i.z));

		//Debug.Log(string.Format("ellipsoid_scale ({0},{1},{2})", ellipsoid_scale.x, ellipsoid_scale.y, ellipsoid_scale.z));

		ellipsoid_scale *= 1 / MasterScale;
		InertiaEllipsoid.transform.localScale = ellipsoid_scale * 2; // Remember unity sphere has radius 1/2.

		//float f = EvalEllipsoid(Body.BodyOmega(), ellipsoid_scale);
		//Debug.Log(string.Format("Curr ellipse eq value: {0}", f));

		AngularVelocityTrail.enabled = false;
		AngularVelocityTrail.Clear();
	}

	void OrientCamera()
	{
		var ctt = InertiaEllipsoid.transform.position - InitialCameraPosition;
		ctt = ctt.normalized;

		var up = DVector3.ToUnity(Body.L);
		up = up - Vector3.Dot(up, ctt) * ctt;

		Camera.GetComponent<OrbitCamera>().SetUpVector(up);
	}

	void OrientPlanesAndL()
	{
		// We want the planes to be perpendicular to the angular momentum of the body.

		Quaternion q = new Quaternion();

		q.SetFromToRotation(Vector3.down, DVector3.ToUnity(Body.L));
		UpperPlane.transform.rotation = q;

		q.SetFromToRotation(Vector3.up, DVector3.ToUnity(Body.L));

		L.transform.localRotation = q;
	}

	void PositionPlanes()
	{
		// The upper and low invariable planes are 1 unit along the angular momentum
		// direction.
		UpperPlane.transform.localPosition = L_dir;
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
		q.SetFromToRotation(Vector3.up, DVector3.ToUnity(Body.Omega));

		Omega.transform.localRotation = q;
		Omega.SetLength((float) Body.Omega.magnitude / MasterScale);

		AngularVelocityTrail.transform.localPosition = DVector3.ToUnity(Body.Omega / MasterScale);

		InertiaEllipsoid.transform.rotation = DQuaternion.ToUnity(Body.Orientation);

		AngularVelocityTrail.enabled = true;			
	}
}
