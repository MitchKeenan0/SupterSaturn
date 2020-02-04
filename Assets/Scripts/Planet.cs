using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
	public GameObject planetMesh;
	public GameObject atmosphereMesh;
	public float maxSize = 3f;
	public float minSize = 0.3f;

    void Start()
    {
		InitPlanet();
    }

	void InitPlanet()
	{
		Vector3 newSize = Vector3.one * Random.Range(minSize, maxSize);
		if (planetMesh != null)
			planetMesh.transform.localScale = newSize;
		if (atmosphereMesh != null)
			atmosphereMesh.transform.localScale = newSize;
	}
}
