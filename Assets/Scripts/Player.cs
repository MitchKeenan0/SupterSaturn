using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public Card[] spacecraftCards;

	private int chevrons = 100;
	public int GetChevrons() { return chevrons; }

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

    void Start()
    {
		chevrons = Random.Range(chevrons, chevrons + 1);
    }

	public void AddChevrons(int value)
	{
		chevrons += value;
	}
}
