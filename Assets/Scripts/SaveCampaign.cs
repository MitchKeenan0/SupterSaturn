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
	public List<string> identityNames = new List<string>();
	public List<int> identityLocationList = new List<int>();
	public List<float> identityColorsR = new List<float>();
	public List<float> identityColorsG = new List<float>();
	public List<float> identityColorsB = new List<float>();
}
