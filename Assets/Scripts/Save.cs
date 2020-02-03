using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
	public string playerName = "";
	public int chevrons = 0;
	public bool fleetTutorialClosed = false;
	public bool battleTutorialClosed = false;

	public List<int> cardList = new List<int>();
	public List<int> healthList = new List<int>();
	public List<int> enemyCardList = new List<int>();

	//public List<int> locationList = new List<int>();
	//public List<Vector3> locationPositionList = new List<Vector3>();
}
