using UnityEngine;
using Assets.DoubleMath;

public class BinetCamera : MonoBehaviour
{
	public bool FollowCurrent;

	GameObject MomentumSphere;
	PRigidBody Body;

	public void SetTargets(
						GameObject momentum_sphere,
						PRigidBody body)
	{
		MomentumSphere = momentum_sphere;
		Body = body;
	}

	void UpdatePosition()
	{
		var body_l = Body.BodyL();
		var offset = body_l.normalized * 4;
		transform.localPosition = DVector3.ToUnity(offset);
		transform.LookAt(MomentumSphere.transform);
	}


	bool HaveUpdated;
	void Start()
	{
		HaveUpdated = false;
	}


	// Update is called once per frame
	void Update()
    {
		if (!HaveUpdated || FollowCurrent)
		{
			UpdatePosition();
			HaveUpdated = true;
		}
	}
}
