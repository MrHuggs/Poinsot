using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
	public PRigidBody Body;

	public InputField Ix;
	public InputField Iy;
	public InputField Iz;

	public InputField Omega_x;
	public InputField Omega_y;
	public InputField Omega_z;

	public Toggle PreserverLKe;


	void GetFromBody()
	{
		const string fmt = "F2";
		Ix.text = Body.I.x.ToString(fmt);
		Iy.text = Body.I.y.ToString(fmt);
		Iz.text = Body.I.z.ToString(fmt);

		Omega_x.text = Body.Omega.x.ToString(fmt);
		Omega_y.text = Body.Omega.y.ToString(fmt);
		Omega_z.text = Body.Omega.z.ToString(fmt);

		PreserverLKe.isOn = Body.ApplyAdjustment;
	}

	static float safe_parse(InputField field)
	{
		float f;
		try
		{
			f = float.Parse(field.text);

		}
		catch
		{
			f = 1;
		}
		return f;
	}

	public void PutToBody()
	{
		Debug.Log("Controls - put to body");

		var inertia = new Vector3(safe_parse(Ix), safe_parse(Iy), safe_parse(Iz));
		var omega = new Vector3(safe_parse(Omega_x), safe_parse(Omega_y), safe_parse(Omega_z));
		bool apply = PreserverLKe.isOn;

		Debug.Log(string.Format("{0}, {1}, {2}", inertia, omega, apply));

		Body.SetParameters(inertia, omega, apply);
	}



	// Start is called before the first frame update
	void Start()
    {
		GetFromBody();
		Body.BodyParmsChanged += GetFromBody;
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
