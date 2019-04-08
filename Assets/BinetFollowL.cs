using UnityEngine;
using Assets.DoubleMath;

public class BinetFollowL : MonoBehaviour
{
	// Start is called before the first frame update
	[HideInInspector]
	public PRigidBody Target;
	[HideInInspector]
	public float Scale;

	void Start()
    {
	}

    // Update is called once per frame
    void Update()
    {
		var body_l = Target.BodyL();
		var offset = body_l * Scale;

		transform.localPosition = DVector3.ToUnity(offset);
	}
}
