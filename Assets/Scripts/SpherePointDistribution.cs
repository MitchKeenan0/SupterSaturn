using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpherePointDistribution : MonoBehaviour
{
	public bool bStartActive = false;
	public GameObject pointPrefab;
	public float sphereScale = 32f;
	public int numPoints = 128;

	private List<GameObject> pointsList;

	void Start()
	{
		pointsList = new List<GameObject>();

		if (bStartActive)
		{
			Vector3[] pts = PointsOnSphere(numPoints);
			List<GameObject> uspheres = new List<GameObject>();
			int i = 0;

			foreach (Vector3 value in pts)
			{
				uspheres.Add(Instantiate(pointPrefab, transform));
				uspheres[i].transform.parent = transform;
				uspheres[i].transform.position = transform.position + (value * sphereScale);
				i++;
			}
		}
	}

	Vector3[] PointsOnSphere(int n)
	{
		List<Vector3> upts = new List<Vector3>();
		float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
		float off = 2.0f / n;
		float x = 0;
		float y = 0;
		float z = 0;
		float r = 0;
		float phi = 0;

		for (var k = 0; k < n; k++)
		{
			y = k * off - 1 + (off / 2);
			r = Mathf.Sqrt(1 - y * y);
			phi = k * inc;
			x = Mathf.Cos(phi) * r;
			z = Mathf.Sin(phi) * r;

			upts.Add(new Vector3(x, y, z));
		}
		Vector3[] pts = upts.ToArray();
		return pts;
	}

	public void UpdateSphere(float size)
	{
		sphereScale = size;

		Vector3[] pts = PointsOnSphere(numPoints);
		int i = 0;

		if (pointsList == null)
			pointsList = new List<GameObject>();

		foreach (Vector3 value in pts)
		{
			if (pointsList.Count < numPoints)
				pointsList.Add(Instantiate(pointPrefab, transform));
			pointsList[i].transform.parent = transform;
			pointsList[i].transform.position = transform.position + (value * sphereScale);
			i++;
		}
	}

	public void SetPointSize(float value)
	{
		if (pointsList != null)
		{
			foreach (GameObject go in pointsList)
			{
				if (value > 0.001f)
				{
					go.transform.localScale = Vector3.one * value;
					go.SetActive(true);
				}
				else
				{
					go.SetActive(false);
				}
			}
		}
	}
}
