using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameLibrary : MonoBehaviour
{
	public string[] fighterNames = { "Bauer", "Carrera", "Voltez", "Abbey" };
	public string[] schoonerNames = { "Libra", "Calet", "Figurine" };

	public string[] commonLocationNames = { "Obes", "Tatir", "Bando", "Sirius", "Vega", "Caliope", "Specta", "Maoa", "Iyu", "Blue Star", "Aax" };
	public string[] uniqueLocationNames = { "Reccez", "Aqua Nebula", "Karaoke Nebula", "Randal's Cloud", "The Maelstrom", "Teh Garden", "Hydrogean", "Helix Nebula", "Sauna", "Guedes Giant", "Harrow" };
	public string[] rareLocationNames = { "Family Fortress", "Edne", "Reasearch Base", "Nike", "HG-413", "Lost Temple", "Super Saturn", "Seed of the Stars", "Matrices of Or", "Father Star", "Never", "Empress" };

	public string[] identityNames = { "Bauer", "CultureViper", "Carrera", "Voltez", "Kara", "Abriette", "Tangerine", "Minerva", "Chandra", "Ali", "Mahdi" };
	public string[] fleetNames = { "Rogue Navy", "Scerzaron", "Thief Cartel", "Herald Fleet", "Bodysnatchers" };

	public List<int> usedCommonLocations;
	public List<int> usedUniqueLocations;
	public List<int> usedRareLocations;
	public List<int> usedIdentityNames;

	void Awake()
	{
		usedCommonLocations = new List<int>();
		usedUniqueLocations = new List<int>();
		usedRareLocations = new List<int>();
		usedIdentityNames = new List<int>();
		DontDestroyOnLoad(gameObject);
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

		bool foundName = false;
		string result = "";
		while (!foundName)
		{
			int randomIndex = Random.Range(0, nameArray.Length);
			if (!usedList.Contains(randomIndex))
			{
				result = nameArray[randomIndex];
				switch(rarity)
				{
					case 0:
						usedCommonLocations.Add(randomIndex);
						break;
					case 1:
						usedUniqueLocations.Add(randomIndex);
						break;
					case 2:
						usedRareLocations.Add(randomIndex);
						break;
					default:
						break;
				}
				foundName = true;
			}
		}

		return result;
	}

	public string GetFleetName()
	{
		string fleetName = "";
		int randomIndex = Random.Range(0, fleetNames.Length);
		fleetName = fleetNames[randomIndex];
		return fleetName;
	}

	public string GetIdentityName()
	{
		string idName = "";
		bool nameFound = false;
		while (!nameFound)
		{
			int rando = Random.Range(0, identityNames.Length - 1);
			if (!usedIdentityNames.Contains(rando))
			{
				idName = identityNames[rando];
				usedIdentityNames.Add(rando);
				nameFound = true;
			}
		}
		return idName;
	}
}
