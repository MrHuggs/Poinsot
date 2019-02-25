using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetCameraName : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		var c = GetComponent<Text>();
		for (var parent = transform; ; parent = parent.parent)
		{
			if (parent == null)
			{
				c.text = "Camera not found";
				break;
			}
			if (parent.GetComponent<Camera>() != null)
			{
				c.text = parent.name;
				break;
			}
		}

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
