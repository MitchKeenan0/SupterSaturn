using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
	public string playerName = "";
	public int chevrons = 0;
	public int playerSpacecraftCard = 0;
	public int playerHealth = 0;
	public int crewPilot = 1;
	public int crewEngine = 1;
	public int crewPower = 1;
	public bool fleetTutorialClosed = false;
	public bool gameTutorialClosed = false;

	public float levelOneBestTime = 0f;
}
