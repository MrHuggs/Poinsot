﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowOmega : MonoBehaviour
{
	public GameObject Target;
	PRigidBody TargetBody;
    // Start is called before the first frame update
    void Awake()
    {
		TargetBody = Target.GetComponent<PRigidBody>();
	}

	// Update is called once per frame
	void Update()
    {
		transform.position = Target.transform.position;

		Quaternion q = new Quaternion();
		q.SetFromToRotation(Vector3.up, TargetBody.Omega);

		transform.rotation = q;
	}
}
