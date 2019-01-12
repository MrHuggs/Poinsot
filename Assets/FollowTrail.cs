using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTrail : MonoBehaviour
{

    void Start()
    {
		// The trail starts out disabled so that the initial postion (which is meaningless)
		// isn't used in the trail.
		GetComponent<TrailRenderer>().enabled = true;
	}

    // Update is called once per frame
    void Update()
    {
    }
}
