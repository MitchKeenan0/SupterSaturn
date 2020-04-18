using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveSurroundings : MonoBehaviour
{
	public ObjectiveSurroundingsIcon[] primarySystemObjects;
	public ObjectiveSurroundingsIcon[] moonObjects;
	public int moonObjectCountFactor = 5;
	public ObjectiveSurroundingsIcon[] secondarySystemObjects;
	public int secondaryObjectCountFactor = 2;
	public ObjectiveSurroundingsIcon[] salientObjects;
	public int salientObjectCountFactor = 1;

	private List<ObjectiveSurroundingsIcon> surroundingsList;

	public List<ObjectiveSurroundingsIcon> GetSurroundings() { return surroundingsList; }

	void Start()
    {
		surroundingsList = new List<ObjectiveSurroundingsIcon>();
	}

	public void CreateSurroundings(int rating)
	{
		surroundingsList.Clear();
		InitPrimary(rating);
		InitMoons(rating);
		InitSecondaries(rating);
		InitSalients(rating);
	}

	void InitPrimary(int rating)
	{
		int primaries = primarySystemObjects.Length;
		int randomPrimary = Random.Range(0, primaries);
		ObjectiveSurroundingsIcon osi = Instantiate(primarySystemObjects[randomPrimary], transform);
		if (osi != null)
		{
			osi.InitIcon();
			surroundingsList.Add(osi);
		}
	}

	void InitMoons(int rating)
	{
		int numMoons = Random.Range(rating, (rating * moonObjectCountFactor));
		for (int i = 0; i < numMoons; i++)
		{
			int moons = moonObjects.Length;
			int randomMoon = Random.Range(0, moons);
			ObjectiveSurroundingsIcon osi = Instantiate(moonObjects[randomMoon], transform);
			if (osi != null)
			{
				osi.InitIcon();
				surroundingsList.Add(osi);
			}
		}
	}

	void InitSecondaries(int rating)
	{
		int numSecondaries = Random.Range(rating, (rating * secondaryObjectCountFactor));
		for(int i = 0; i < numSecondaries; i++)
		{
			bool goodSpawn = false;
			int secondaries = secondarySystemObjects.Length;
			while(!goodSpawn)
			{
				int randomSecondary = Random.Range(0, secondaries);
				ObjectiveSurroundingsIcon osi = Instantiate(secondarySystemObjects[randomSecondary], transform);
				if (osi != null)
				{
					bool spawn = true;
					if ((osi.rarity < 1f) && (Random.Range(0f, 1f) >= osi.rarity))
						spawn = false;
					if (spawn)
					{
						osi.InitIcon();
						surroundingsList.Add(osi);
						goodSpawn = true;
					}
					else
					{
						Destroy(osi.gameObject);
					}
				}
			}
		}
	}

	void InitSalients(int rating)
	{
		int numSalients = Random.Range(0, salientObjectCountFactor);
		for(int i = 0; i < numSalients; i++)
		{
			int salients = salientObjects.Length;
			int randomSalient = Random.Range(0, salients);
			ObjectiveSurroundingsIcon osi = Instantiate(salientObjects[randomSalient], transform);
			if (osi != null)
			{
				osi.InitIcon();
				surroundingsList.Add(osi);
			}
		}
	}
}
