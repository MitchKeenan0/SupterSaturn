using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
	public string playerName = "";
	public int chevrons = 0;
	public bool fleetTutorialClosed = false;
	public bool gameTutorialClosed = false;

	public int playerSpacecraftCard = 0;
	public int playerHealth = 0;
}
