using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cone : MonoBehaviour
{
	private Mesh mesh;
	private Vector3[] vertices;
	private int[] triangles;

	void Start()
    {
		gameObject.AddComponent<MeshFilter>();

		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.name = "Arrow";


		// Create a pyarmid. We need 5 verats and 6 triangles.
		vertices = new Vector3[5];
		vertices[0].Set(0, 1, 0);
		vertices[1].Set(-1, 0, -1);
		vertices[2].Set( 1, 0, -1);
		vertices[3].Set( 1, 0, 1);
		vertices[4].Set(-1, 0, 1);

		triangles = new int[]
		{
			0, 1, 2,
			0, 2, 3,
			0, 3, 4,
			0, 4, 1,
			4, 3, 2,
			4, 2, 1
		};

		mesh.vertices = vertices;
		mesh.triangles = triangles;
	}

}
