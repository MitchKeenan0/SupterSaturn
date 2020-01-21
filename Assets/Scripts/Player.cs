using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public Card[] spacecraftCards;
	public int initialChevrons = 1000;

	private List<Spacecraft> spacecraftList;

	private int chevrons = 0;
	public int GetChevrons() { return chevrons; }
	public void UpdateChevronAccount(int value) { chevrons += value; }

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		UpdateChevronAccount(initialChevrons);
		spacecraftList = new List<Spacecraft>();
	}

	public void AddSpacecraft(Spacecraft sp)
	{
		if (!spacecraftList.Contains(sp))
			spacecraftList.Add(sp);
	}

	public void RemoveSpacecraft(Spacecraft sp)
	{
		if (spacecraftList.Contains(sp))
			spacecraftList.Add(sp);
	}
}
