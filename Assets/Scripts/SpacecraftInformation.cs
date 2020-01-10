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
		if (spacecraft != null)
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
	{
		if (spacecraft.GetAgent() != null)
			return spacecraft.GetAgent().teamID;
		else
			return 0;
	}

	public string GetHeadline()
	{
		if (spacecraft.GetAgent() != null)
			return spacecraft.GetAgent().GetHeadline();
		else
			return "";
	}
}
