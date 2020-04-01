using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexVisualizer : MonoBehaviour
{
	public LineRenderer linePrefab;
	public float vertexAccountPercentage = 0.3f;

	private ScoreHUD scoreHud;
	private List<Vector3> vertexList;
	private List<LineRenderer> lineList;

	private IEnumerator loadCoroutine;

	public List<Vector3> GetVertexList() { return vertexList; }
	public int GetNumVerticies() { return (vertexList != null) ? vertexList.Count : 0; }

    void Start()
    {
		scoreHud = FindObjectOfType<ScoreHUD>();
		loadCoroutine = LoadWait(0.6f);
		StartCoroutine(loadCoroutine);
    }

	IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		InitLines();
		scoreHud.InitScore(vertexList.Count);
	}

	void InitLines()
	{
		lineList = new List<LineRenderer>();
		vertexList = new List<Vector3>();

		Planet pl = null;
		Planet[] planets = FindObjectsOfType<Planet>();
		int numPlanets = planets.Length;
		for(int i = 0; i < numPlanets; i++)
		{
			Planet pp = planets[i];
			if (pp.CompareTag("Planet"))
			{
				pl = pp;
				break;
			}
		}
		if (pl != null)
		{
			transform.parent = pl.planetMesh.transform;
			transform.transform.position = Vector3.zero;
			transform.transform.rotation = pl.planetMesh.transform.rotation;
			transform.localScale = Vector3.one;
			for(int i = 0; i < pl.GetVertexList().Count; i++)
			{
				if (Random.Range(0f, 1f) < vertexAccountPercentage)
				{
					Vector3 vertex = pl.GetVertexList()[i];
					vertexList.Add(vertex);
				}
			}
		}

		int numVerts = vertexList.Count;
		for (int i = 0; i < numVerts; i++)
		{
			LineRenderer line = SpawnLineRenderer();
			Vector3 vertexPosition = vertexList[i];
			Vector3 vertexNormal = vertexPosition * 1.3f;
			line.SetPosition(0, vertexPosition);
			line.SetPosition(1, vertexNormal);
		}
	}

	LineRenderer SpawnLineRenderer()
	{
		LineRenderer line = Instantiate(linePrefab, transform);
		line.useWorldSpace = false;
		lineList.Add(line);
		return line;
	}

	public void DisableLine(Vector3 vertexPosition)
	{
		if (vertexList.Contains(vertexPosition))
		{
			if (lineList == null)
				lineList = new List<LineRenderer>();
			int lineIndex = vertexList.IndexOf(vertexPosition);
			if (lineList.Count > lineIndex)
			{
				lineList[lineIndex].enabled = false;
				lineList[lineIndex].gameObject.SetActive(false);
			}
		}
	}
}
