using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crew : MonoBehaviour
{
	public static Crew crew;
	private int pilot = 1;
	private int engine = 1;
	private int power = 1;
	private int levelGrade = 10;

	void Awake()
	{
		if (crew == null)
		{
			DontDestroyOnLoad(gameObject);
			crew = this;
		}
		else if (crew != this)
		{
			Destroy(gameObject);
		}
	}

	public void ImbueSpacecraft(Spacecraft sp)
	{
		sp.turningPower += pilot;
		sp.mainEnginePower += engine;
		Weapon wp = sp.gameObject.GetComponent<Weapon>();
		if (wp != null)
			wp.damage += power;
	}

	public void PilotPoints(int value)
	{
		pilot += value;
	}

	public void EnginePoints(int value)
	{
		engine += value;
	}

	public void PowerPoints(int value)
	{
		power += value;
	}
}
