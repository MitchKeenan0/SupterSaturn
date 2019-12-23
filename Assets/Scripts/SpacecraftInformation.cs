using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacecraftInformation : MonoBehaviour
{
	private Spacecraft spacecraft;
	private string spacecraftName = "Unknown";
	private Vector3 position = Vector3.zero;
	//private float totalDamageRecieved = 0;
	//private float previousDamage = 0;
	//private float deltaDamage = 0;

    void Start()
    {
		spacecraft = GetComponent<Spacecraft>();
		spacecraftName = spacecraft.spacecraftName;
    }

	public string GetSpacecraftName()
	{ return spacecraftName; }

	public float GetDistance(Vector3 fromPosition)
	{ return Vector3.Distance(transform.position, fromPosition) * 69f; }

	public Vector3 GetPosition()
	{ return transform.position; }

	public int GetMarks()
	{ return spacecraft.GetMarks(); }

	public int GetTeamID()
	{ return spacecraft.GetAgent().teamID; }

	public string GetHeadline()
	{ return spacecraft.GetAgent().GetHeadline(); }
}
