using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCoordinator : MonoBehaviour
{
	private Spacecraft spacecraft;
	private Agent agent;
	private List<Weapon> weapons;
	private Transform targetTransform;

    void Start()
    {
		spacecraft = GetComponent<Spacecraft>();
		agent = GetComponent<Agent>();
		InitWeapons();
    }

	void InitWeapons()
	{
		weapons = new List<Weapon>();
		Weapon[] spacecraftWeapons = GetComponents<Weapon>();
		foreach (Weapon wp in spacecraftWeapons)
		{
			weapons.Add(wp);
		}
	}

    public void UpdateFireCoordinator()
	{
		if (agent.GetTargetTransform() != null)
		{
			targetTransform = agent.GetTargetTransform();
			foreach (Weapon wp in weapons)
			{
				if (wp.CanFire())
				{
					wp.FireAt(targetTransform);
				}
			}
		}
	}

	public void StandDown()
	{
		foreach(Weapon wp in weapons)
		{
			wp.SetTarget(null);
		}
	}
}
