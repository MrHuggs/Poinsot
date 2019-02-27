using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pyramid : MonoBehaviour
{
	private Mesh mesh;
	private Vector3[] vertices;
	private Vector3[] normals;
	private int[] triangles;

	void Start()
    {
		gameObject.AddComponent<MeshFilter>();

		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.name = "Pyramid";


		// Create a pyarmid. We need 5 verats and 6 triangles.
		vertices = new Vector3[16];
		normals = new Vector3[16];

		vertices[0].Set(0, 1, 0);
		normals [0].Set(0, 0.7071f, -0.7071f);
		vertices[1].Set(1, 0, -1);
		normals [1].Set(0, 0.7071f, -0.7071f);
		vertices[2].Set(-1, 0, -1);
		normals [2].Set(0, 0.7071f, -0.7071f);

		vertices[3].Set(0, 1, 0);
		normals [3].Set(0.7071f, 0.7071f, 0);
		vertices[4].Set(1, 0, 1);
		normals[4].Set(0.7071f, 0.7071f, 0);
		vertices[5].Set(1, 0, -1);
		normals[5].Set(0.7071f, 0.7071f, 0);

		vertices[6].Set(0, 1, 0);
		normals[6].Set(0, 0.7071f, 0.7071f);
		vertices[7].Set(-1, 0, 1);
		normals[7].Set(0, 0.7071f, 0.7071f);
		vertices[8].Set(1, 0, 1);
		normals[8].Set(0, 0.7071f, 0.7071f);


		vertices[9].Set(0, 1, 0);
		normals[9].Set(-0.7071f, 0.7071f, 0);
		vertices[10].Set(-1, 0, -1);
		normals[10].Set(-0.7071f, 0.7071f, 0);
		vertices[11].Set(-1, 0, 1);
		normals[11].Set(-0.7071f, 0.7071f, 0);

		vertices[12].Set(-1, 0, -1);  // Bottom
		normals [12] = Vector3.down;
		vertices[13].Set( 1, 0, -1);
		normals[13] = Vector3.down;
		vertices[14].Set( 1, 0, 1);
		normals[14] = Vector3.down;
		vertices[15].Set(-1, 0, 1);
		normals[15] = Vector3.down;

		// The front faces have a clockwise order in Unity.
		triangles = new int[]
		{
			0, 1, 2,
			3, 4, 5,
			6, 7, 8,
			9, 10, 11,
			14, 12, 13,
			14, 15, 12
		};

		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.triangles = triangles;
	}
}
