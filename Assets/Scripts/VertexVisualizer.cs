using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexVisualizer : MonoBehaviour
{
	public LineRenderer linePrefab;

	private List<Vector3> vertexList;
	private List<LineRenderer> lineList;

	private IEnumerator loadCoroutine;

    void Start()
    {
		loadCoroutine = LoadWait(0.6f);
		StartCoroutine(loadCoroutine);
    }

	IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		InitLines();
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
				Vector3 vertex = pl.GetVertexList()[i];
				vertexList.Add(vertex);
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
