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

	private Mesh mesh;
	private ScoreHUD scoreHud;
	private RaycastManager raycastManager;
	private VertexVisualizer vertexVisualizer;
	private List<Vector3> vertexList;
	private List<Vector3> freshVertices;
	private float planetSize = 1f;

	public float GetSize() { return planetSize; }
	public List<Vector3> GetVertexList() { return vertexList; }

	void Awake()
    {
		InitPlanet();
	}

	void InitPlanet()
	{
		raycastManager = FindObjectOfType<RaycastManager>();
		scoreHud = FindObjectOfType<ScoreHUD>();
		mesh = GetComponent<MeshFilter>().mesh;
		vertexVisualizer = FindObjectOfType<VertexVisualizer>();
		vertexList = new List<Vector3>();
		freshVertices = new List<Vector3>();
		foreach (Vector3 ve in mesh.vertices)
		{
			vertexList.Add(ve);
			freshVertices.Add(ve);
		}

		Vector3[] vertexArray = vertexList.ToArray();
		Color32[] colors = new Color32[vertexArray.Length];
		byte randoGreyscale = (byte)Random.Range(50, 100);
		Color32 rando = new Color32(randoGreyscale, randoGreyscale, randoGreyscale, 255);
		for (int i = 0; i < vertexArray.Length; i++)
			colors[i] = rando;
		mesh.colors32 = colors;
	}

	public void GetScanned(Vector3 position, float range, Color32 color)
	{
		/// vertex hit detection
		List<Vector3> hitVerts = new List<Vector3>();
		foreach(Vector3 ve in vertexList)
		{
			Vector3 vertex = transform.TransformPoint(ve);
			if (Vector3.Distance(position, vertex) <= range)
			{
				//RaycastHit hit = raycastManager.CustomLinecast(position, vertex);
				//if (hit.transform != null)
				//{
				//	if (Vector3.Distance(hit.point, vertex) <= range)
				//		hitVerts.Add(ve);
				//}
				if (freshVertices.Contains(ve))
					hitVerts.Add(ve);
			}
		}

		if (hitVerts.Count > 0)
		{
			Color32[] colors = new Color32[vertexList.Count];
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

					if (vertexVisualizer != null)
					{
						vertexVisualizer.DisableLine(hitVertex);
						int maxScore = vertexVisualizer.GetNumVerticies();

						List<Vector3> visList = new List<Vector3>();
						visList = vertexVisualizer.GetVertexList();
						if (visList.Contains(hitVertex) && (scoreHud != null))
						{
							Vector3 vertexWorldPosition = transform.TransformPoint(hitVertex);
							scoreHud.PopupScore(vertexWorldPosition, 1, maxScore);
						}
					}

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
		GetComponent<SphereCollider>().radius = 0.485f;
		planetSize = sizeMagnitude;
	}
}
