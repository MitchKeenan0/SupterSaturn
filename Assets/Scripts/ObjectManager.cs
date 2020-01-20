﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
	public List<Spacecraft> GetSpacecraftList() { return spacecraftList; }
	private List<Spacecraft> spacecraftList;

	public List<Gravity> GetGravityList() { return gravityList; }
	private List<Gravity> gravityList;

	private CraftIconHUD hud;
	private BattleOutcome battleOutcome;

	void Awake()
    {
		InitSpacecraftList();
		InitGravityList();
    }

	void Start()
	{
		hud = FindObjectOfType<CraftIconHUD>();
		battleOutcome = FindObjectOfType<BattleOutcome>();
	}

	void InitSpacecraftList()
	{
		spacecraftList = new List<Spacecraft>();
		Spacecraft[] sp = FindObjectsOfType<Spacecraft>();
		foreach (Spacecraft s in sp)
		{
			spacecraftList.Add(s);
		}
	}

	void InitGravityList()
	{
		gravityList = new List<Gravity>();
		Gravity[] gr = FindObjectsOfType<Gravity>();
		foreach (Gravity g in gr)
		{
			gravityList.Add(g);
		}
	}

	void TestBattleConcluded()
	{
		int numSpacecraft = spacecraftList.Count;
		int numTeam = 0;
		int numEnemy = 0;
		for(int i = 0; i < numSpacecraft; i++)
		{
			Spacecraft sp = spacecraftList[i];
			if (sp.GetAgent().teamID == 0)
				numTeam++;
			else if (sp.GetAgent().teamID == 1)
				numEnemy++;
		}

		if (numEnemy == 0)
			battleOutcome.BattleOver(true);
		else if (numTeam == 0)
			battleOutcome.BattleOver(false);
	}

	public void SpacecraftDestroyed(Spacecraft value)
	{
		hud.ModifySpacecraftList(value, false);
		spacecraftList.Remove(value);
		TestBattleConcluded();
	}
}
