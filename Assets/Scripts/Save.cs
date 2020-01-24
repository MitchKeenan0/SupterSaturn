using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
	public string playerName = "";
	public List<int> cardIDList = new List<int>();
	public List<int> spacecraftHealthList = new List<int>();
	public int chevrons = 0;
}
