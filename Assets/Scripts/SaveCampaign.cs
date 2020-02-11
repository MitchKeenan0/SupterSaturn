using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveCampaign
{
	public List<int> locationList = new List<int>();
	public List<float> locationPositionsX = new List<float>();
	public List<float> locationPositionsY = new List<float>();
	public List<float> locationPositionsZ = new List<float>();
	public List<string> locationNameList = new List<string>();
	//
	public List<int> identityList = new List<int>();
	public List<int> identityLocationList = new List<int>();
}
