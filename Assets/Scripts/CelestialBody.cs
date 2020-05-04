using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
	public string celestialBodyName = "";

	private NameLibrary nameLibrary;
	private Planet planet;

	public Vector3 GetPlanetPosition() { return planet.planetMesh.transform.position; }

    void Start()
    {
		nameLibrary = FindObjectOfType<NameLibrary>();
		planet = GetComponentInChildren<Planet>();
		GetName();
    }

	void GetName()
	{
		int moonPool = nameLibrary.moonNameMasterList.Count;
		int randoPool = Random.Range(0, moonPool);
		string[] moonNames = nameLibrary.moonNameMasterList[randoPool];
		int poolLength = moonNames.Length;
		int randoIndex = Random.Range(0, poolLength);
		celestialBodyName = moonNames[randoIndex];
	}
}
