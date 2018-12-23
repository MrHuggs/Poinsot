using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
	public GameObject Target;
    // Start is called before the first frame update
    void Start()
    {
		transform.LookAt(Target.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
