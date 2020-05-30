using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveSurroundings : MonoBehaviour
{
	public ObjectiveSurroundingsIcon[] primarySystemObjects;
	public ObjectiveSurroundingsIcon[] moonObjects;
	public float[] distances;
	public float[] rotations;

	public ObjectiveSurroundingsIcon[] secondarySystemObjects;

	public ObjectiveSurroundingsIcon[] salientObjects;
	

	private List<ObjectiveSurroundingsIcon> surroundingsList;

	public List<ObjectiveSurroundingsIcon> GetSurroundings() { return surroundingsList; }

	void Start()
    {
		surroundingsList = new List<ObjectiveSurroundingsIcon>();
	}

	
}
