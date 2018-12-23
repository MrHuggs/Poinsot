using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetCameraName : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		var r = transform.root;
		var c = GetComponent<Text>();
		c.text = r.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
