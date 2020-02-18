using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
	public GameObject planetMesh;
	public GameObject atmosphereMesh;
	public float maxSize = 3f;
	public float minSize = 0.3f;

    void Awake()
    {
		InitPlanet();
    }

	public void SetScale(float sizeMagnitude)
	{
		planetMesh.transform.localScale = Vector3.one * sizeMagnitude;
		GetComponentInChildren<SphereCollider>().radius = 0.5f;
	}

	void InitPlanet()
	{
		Color planetColor = new Color();
		planetColor.a = 1;
		planetColor.r = Random.Range(0.05f, 0.5f);
		planetColor.g = Random.Range(0.05f, 0.5f);
		planetColor.b = Random.Range(0.05f, 0.5f);
		planetMesh.GetComponent<Renderer>().material.color = planetColor;
	}
}
