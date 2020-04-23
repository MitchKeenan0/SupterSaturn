using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexVisualizer : MonoBehaviour
{
	public ScanCollider scanColliderPrefab;
	public float vertexAccountPercentage = 0.3f;
	public float lineExtentScale = 2f;

	private ScoreHUD scoreHud;
	private List<Vector3> vertexList;
	private List<ScanCollider> scanColliderList;

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
		if (scoreHud != null)
			scoreHud.UpdateMaxScore(vertexList.Count);
	}

	void InitLines()
	{
		scanColliderList = new List<ScanCollider>();
		vertexList = new List<Vector3>();

		Planet pl = transform.GetComponentInParent<Planet>();
		if (pl != null)
		{
			//transform.parent = pl.planetMesh.transform;
			transform.localPosition = Vector3.zero;
			transform.rotation = pl.planetMesh.transform.rotation;
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
			Vector3 vertexNormal = vertexPosition * lineExtentScale;
			line.transform.position = transform.position + vertexPosition;
			line.SetPosition(0, (vertexPosition * (lineExtentScale * 0.95f)));
			line.SetPosition(1, vertexNormal);
			SphereCollider sc = line.gameObject.GetComponent<SphereCollider>();
			sc.center = vertexPosition;
		}
	}

	LineRenderer SpawnLineRenderer()
	{
		ScanCollider sc = Instantiate(scanColliderPrefab, transform);
		LineRenderer line = sc.GetComponent<LineRenderer>();
		line.useWorldSpace = false;
		scanColliderList.Add(sc);
		return line;
	}

	public void DisableLine(Vector3 vertexPosition)
	{
		Vector3 local = transform.TransformPoint(vertexPosition);
		if (vertexList.Contains(vertexPosition))
		{
			if (scanColliderList == null)
				scanColliderList = new List<ScanCollider>();
			int lineIndex = vertexList.IndexOf(vertexPosition);
			if (scanColliderList.Count > lineIndex)
			{
				scanColliderList[lineIndex].enabled = false;
				scanColliderList[lineIndex].gameObject.SetActive(false);
			}
		}
	}
}
