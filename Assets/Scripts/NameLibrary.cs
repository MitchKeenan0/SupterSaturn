﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameLibrary : MonoBehaviour
{
	public string[] fighterNames = { "Bauer", "Carrera", "Voltez", "Abbey" };
	public string[] schoonerNames = { "Libra", "Calet", "Figurine" };

	public string[] commonLocationNames = { "Obes", "Tatir", "Bando", "Sirius", "Vega", "Caliope", "Specta", "Maoa", "Iyu", "Blue Star", "Aax" };
	public string[] uniqueLocationNames = { "Reccez", "Aqua Nebula", "Karaoke Nebula", "Randal's Cloud", "The Maelstrom", "Teh Garden", "Hydrogean", "Helix Nebula", "Sauna", "Guedes Giant", "Harrow" };
	public string[] rareLocationNames = { "Family Fortress", "Edne", "Reasearch Base", "Nike", "HG-413", "Lost Temple", "Super Saturn", "Seed of the Stars", "Matrices of Or", "Father Star", "Never", "Empress" };

	public List<int> usedCommonLocations;
	public List<int> usedUniqueLocations;
	public List<int> usedRareLocations;

	void Start()
	{
		usedCommonLocations = new List<int>();
		usedUniqueLocations = new List<int>();
		usedRareLocations = new List<int>();
	}

	public string GetLocationName(int rarity)
	{
		string[] nameArray = new string[0];
		List<int> usedList = new List<int>();
		switch (rarity)
		{
			case 0:
				nameArray = commonLocationNames;
				usedList = usedCommonLocations;
				break;
			case 1:
				nameArray = uniqueLocationNames;
				usedList = usedUniqueLocations;
				break;
			case 2:
				nameArray = rareLocationNames;
				usedList = usedRareLocations;
				break;
			default:
				break;
		}

		int randomIndex = Random.Range(0, nameArray.Length);
		//while (usedList.Contains(randomIndex))
		//	randomIndex = Random.Range(0, nameArray.Length);

		string result = nameArray[randomIndex];
		usedList.Add(randomIndex);
		return result;
	}
}