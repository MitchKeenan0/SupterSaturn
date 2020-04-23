using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
	public GameObject planetMesh;
	public GameObject atmosphereMesh;
	public float maxSize = 3f;
	public float minSize = 0.3f;
	public bool bScannable = false;
	public float colliderScale = 0.5f;
	public float vertexRangeScale = 1f;

	private Mesh mesh;
	private ScoreHUD scoreHud;
	private RaycastManager raycastManager;
	private VertexVisualizer vertexVisualizer;
	private List<Vector3> vertexList;
	private float planetSize = 1f;

	public float GetSize() { return planetSize; }
	public List<Vector3> GetVertexList() { return vertexList; }
	private List<GameObject> moonList;

	void Start()
    {
		InitPlanet();
	}

	void InitPlanet()
	{
		raycastManager = FindObjectOfType<RaycastManager>();
		scoreHud = FindObjectOfType<ScoreHUD>();
		mesh = planetMesh.GetComponent<MeshFilter>().mesh;
		vertexVisualizer = GetComponentInChildren<VertexVisualizer>();
		vertexList = new List<Vector3>();
		foreach (Vector3 ve in mesh.vertices)
			vertexList.Add(ve);

		Vector3[] vertexArray = vertexList.ToArray();
		Color32[] colors = new Color32[vertexArray.Length];
		byte randoGreyscale = (byte)Random.Range(50, 100);
		Color32 rando = new Color32(randoGreyscale, randoGreyscale, randoGreyscale, 255);
		for (int i = 0; i < vertexArray.Length; i++)
			colors[i] = rando;
		mesh.colors32 = colors;
	}

	public int GetScanned(Vector3 position, float range, Color32 color)
	{
		// Store original colors
		Color32[] colors = new Color32[vertexList.Count];
		for (int i = 0; i < colors.Length; i++)
			colors[i] = mesh.colors[i];

		int hits = 0;
		foreach(Vector3 ve in vertexList)
		{
			Vector3 vertex = transform.TransformPoint(ve);
			if (Vector3.Distance(position, vertex) <= (range * vertexRangeScale))
			{
				Vector3 hitVertex = ve;
				if (vertexList.Contains(hitVertex) && (vertexVisualizer != null))
				{
					List<Vector3> visList = new List<Vector3>();
					visList = vertexVisualizer.GetVertexList();
					if (visList.Contains(hitVertex))
					{
						int vertexIndex = vertexList.IndexOf(hitVertex);
						if (colors.Length > vertexIndex)
							colors[vertexIndex] = color;
					}
				}
			}
		}

		mesh.colors32 = colors;
		Vector3[] verts = vertexList.ToArray();
		mesh.vertices = verts;
		mesh.RecalculateBounds();

		return hits;
	}

	public void SetScale(float sizeMagnitude)
	{
		planetMesh.transform.localScale = Vector3.one * sizeMagnitude;
		GetComponentInChildren<SphereCollider>().radius = colliderScale;
		planetSize = sizeMagnitude;
	}
}
