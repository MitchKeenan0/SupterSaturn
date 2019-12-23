using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
	public List<Spacecraft> GetSpacecraftList() { return spacecraftList; }
	private List<Spacecraft> spacecraftList;

	public List<Gravity> GetGravityList() { return gravityList; }
	private List<Gravity> gravityList;

	private CraftIconHUD hud;

	void Awake()
    {
		InitSpacecraftList();
		InitGravityList();
    }

	void Start()
	{
		hud = FindObjectOfType<CraftIconHUD>();
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

	public void SpacecraftDestroyed(Spacecraft value)
	{
		hud.ModifySpacecraftList(value, false);
		spacecraftList.Remove(value);
	}
}
