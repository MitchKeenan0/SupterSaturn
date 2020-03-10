﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
	public GameObject planetMesh;
	public GameObject atmosphereMesh;
	public float maxSize = 3f;
	public float minSize = 0.3f;

	private Mesh mesh;
	private ScoreHUD scoreHud;
	private List<Vector3> vertexList;
	private List<Vector3> freshVertices;

	void Awake()
    {
		InitPlanet();
	}

	void InitPlanet()
	{
		scoreHud = FindObjectOfType<ScoreHUD>();
		mesh = GetComponent<MeshFilter>().mesh;
		vertexList = new List<Vector3>();
		freshVertices = new List<Vector3>();
		foreach (Vector3 ve in mesh.vertices)
		{
			vertexList.Add(ve);
			freshVertices.Add(ve);
		}

		Vector3[] vertexArray = vertexList.ToArray();
		Color32[] colors = new Color32[vertexArray.Length];
		for (int i = 0; i < vertexArray.Length; i++)
			colors[i] = new Color32(15, 15, 15, 255);
		mesh.colors32 = colors;
	}

	public void GetScanned(Vector3 position, float range, Color32 color)
	{
		List<Vector3> hitVerts = new List<Vector3>();
		foreach(Vector3 ve in vertexList)
		{
			Vector3 vertex = transform.TransformPoint(ve);
			if (Vector3.Distance(position, vertex) <= range)
				hitVerts.Add(ve);
		}

		Color32[] colors = new Color32[vertexList.Count];
		if (hitVerts.Count > 0)
		{
			for (int i = 0; i < colors.Length; i++)
				colors[i] = mesh.colors[i];
			Vector3[] hitArray = hitVerts.ToArray();
			for (int j = 0; j < hitArray.Length; j++)
			{
				Vector3 hitVertex = hitArray[j];
				if (vertexList.Contains(hitVertex) && freshVertices.Contains(hitVertex))
				{
					int vertexIndex = vertexList.IndexOf(hitVertex);
					if (colors.Length > vertexIndex)
						colors[vertexIndex] = color;
					scoreHud.PopupScore(transform.TransformPoint(hitVertex), 1, vertexList.Count);
					freshVertices.Remove(hitVertex);
				}
			}
			mesh.colors32 = colors;
		}

		Vector3[] verts = vertexList.ToArray();
		mesh.vertices = verts;
		mesh.RecalculateBounds();
	}

	public void SetScale(float sizeMagnitude)
	{
		transform.localScale = Vector3.one * sizeMagnitude;
		GetComponent<SphereCollider>().radius = 0.48f;
	}
}
