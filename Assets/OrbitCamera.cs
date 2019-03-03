using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
	// Simple orbit camera. Implements orbit and field of view change;

	public GameObject Target;
	public float Sensitivity = 15;
	public float FovSensitivty = 8;

	bool Orbiting;

	Camera Camera;
    // Start is called before the first frame update
    void Start()
    {
		Camera = GetComponent<Camera>();
		Orbiting = false;

		Camera.transform.LookAt(Target.transform);
    }

	private void OnMouseDown()
	{
		Debug.Log(string.Format("On mouse down {0}", name));
	}


	void DoOrbit()
	{
		var dx = Input.GetAxis("Mouse X") * Sensitivity;
		var dy = Input.GetAxis("Mouse Y") * Sensitivity;

		//Debug.Log(string.Format("({0},{1})", dx, dy));

		var targ_to_cam = transform.position - Target.transform.position;

		var up = transform.rotation * Vector3.up;
		var left = transform.rotation * Vector3.left;

		Quaternion qleft = Quaternion.AngleAxis(dx, up);
		Quaternion qup = Quaternion.AngleAxis(dy, left);

		var product = qleft * qup;

		targ_to_cam = product * targ_to_cam;

		transform.position = targ_to_cam + Target.transform.position;

		var new_up = product * up;

		//transform.LookAt(Target.transform);
		var new_rotation = Quaternion.LookRotation(-targ_to_cam, new_up);
		transform.rotation = new_rotation;
	}

	void DoZoom()
	{
		var ds = Input.mouseScrollDelta.y;

		float factor = (ds / FovSensitivty) + 1;

		var cur_fov = Camera.fieldOfView;
		var new_fov = cur_fov * factor;

		// Constrain the new FOV to some reasonable values:
		new_fov = Mathf.Clamp(new_fov, 2, 170);

		Camera.fieldOfView = new_fov;
	}

	// Update is called once per frame
	void Update()
    {
		if (Input.GetMouseButtonDown(0))
		{
			if (Camera.pixelRect.Contains(Input.mousePosition))
			{
				Orbiting = true;
			}
		}
		else
		{
			if (Input.GetMouseButton(0) == false)
					Orbiting = false;
		}

		if (Orbiting)
			DoOrbit();

		if (Camera.pixelRect.Contains(Input.mousePosition))
				DoZoom();
	}
}
