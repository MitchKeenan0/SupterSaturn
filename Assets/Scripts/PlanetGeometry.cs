using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGeometry : MonoBehaviour
{
	public float initialMin = 0f;
	public float initialMax = 0.1f;

	private Mesh mesh;
	private Vector3[] vertices;
	private List<Vector3> modifiedVertList;
	private int vertexCount = 0;

    void Awake()
    {
		mesh = GetComponent<MeshFilter>().mesh;
		modifiedVertList = new List<Vector3>();
		InitMesh();
		GeneratePlanet();
	}

	void InitMesh()
	{
		vertices = mesh.vertices;
		vertexCount = mesh.vertexCount;
	}

	public void GeneratePlanet()
	{
		for(int i = 0; i < vertexCount; i++)
		{
			Vector3 vertexPosition = vertices[i];
			vertexPosition += (vertexPosition * Random.Range(initialMin, initialMax));
			vertices[i] = vertexPosition;
		}

		mesh.vertices = vertices;
		mesh.RecalculateBounds();
	}
}
