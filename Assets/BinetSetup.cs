﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BinetSetup : MonoBehaviour
{

	public GameObject TargetBody;

	GameObject MomentumSphere;
	GameObject EnergyEllipsoid;
	FollowAngularMomentum FollowAngularMomentum;
	BinetCamera BinetCamera;

	// Start is called before the first frame update
	void Start()
    {
		PRigidBody body = TargetBody.GetComponent<PRigidBody>();

		MomentumSphere = transform.Find("MomentumSphere").gameObject;
		EnergyEllipsoid = transform.Find("EnergyEllipsoid").gameObject;
		FollowAngularMomentum = transform.Find("FollowAngularMomentum").gameObject.GetComponent<FollowAngularMomentum>();

		float m2 = Vector3.Dot(body.L, body.L);
		float scale = 2 * body.Energy / m2;

		Debug.Log(string.Format("Energy: {0} Momentum {1} Scale {2} Inertia Tensor {3}", body.Energy, m2, scale, body.I));

		EnergyEllipsoid.transform.localScale = body.I * scale * 2;

		FollowAngularMomentum.Target = body;
		FollowAngularMomentum.Scale = 1 / Mathf.Sqrt(m2);

		BinetCamera = transform.Find("BinetCamera").gameObject.GetComponent<BinetCamera>();
		BinetCamera.SetTargets(MomentumSphere, body);
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
