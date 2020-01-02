using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCoordinator : MonoBehaviour
{
	private Spacecraft spacecraft;
	private Agent agent;
	private List<Weapon> weapons;
	private List<Prediction> predictions;

    void Start()
    {
		spacecraft = GetComponent<Spacecraft>();
		predictions = new List<Prediction>();
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
		if (agent.GetTargetTransform() != null){
			Transform fireTransform = agent.GetTargetTransform();
			if (fireTransform != null)
			{
				foreach (Weapon wp in weapons)
				{
					if (wp.CanFire())
					{
						wp.FireAt(fireTransform);
					}
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
