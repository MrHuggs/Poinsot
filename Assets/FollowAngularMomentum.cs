using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAngularMomentum : MonoBehaviour
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
		var body_l = Target.GetBodyAngularMomentum();
		var offset = body_l * Scale;

		transform.localPosition = offset;			
	}
}
