using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
	public int maxHealth = 10;

	private Spacecraft spacecraft;
	private Agent agent;
	private CraftIconHUD craftIconHud;
	private TeamFleetHUD teamFleetHud;
	private BattleOutcome battleOutcome;
	private int currentHealth = -1;

    void Awake()
    {
		currentHealth = maxHealth;
		spacecraft = GetComponent<Spacecraft>();
		agent = GetComponent<Agent>();
		craftIconHud = FindObjectOfType<CraftIconHUD>();
		teamFleetHud = FindObjectOfType<TeamFleetHUD>();
		battleOutcome = FindObjectOfType<BattleOutcome>();
	}

	public void ModifyHealth(int value, Transform responsibleTransform)
	{
		int damage = Mathf.Clamp(value, -(maxHealth + 1), maxHealth + 1);
		currentHealth += damage;

		if (currentHealth <= 0)
		{
			currentHealth = 0;
			if (responsibleTransform != null)
				spacecraft.SpacecraftDestroyed(responsibleTransform);
		}
		
		if (!battleOutcome)
			battleOutcome = FindObjectOfType<BattleOutcome>();

		if ((battleOutcome != null) && (responsibleTransform != null))
		{
			if ((spacecraft.GetAgent() != null) && (spacecraft.GetAgent().teamID == 0))
			{
				battleOutcome.AddLost(damage);
			}
			else if ((spacecraft.GetAgent() != null) && (spacecraft.GetAgent().teamID == 1))
			{
				battleOutcome.AddScore(damage);
			}
		}

		if (!craftIconHud)
			craftIconHud = FindObjectOfType<CraftIconHUD>();
		if (craftIconHud != null)
			craftIconHud.UpdateSpacecraftHealth(spacecraft);

		if (!teamFleetHud)
			teamFleetHud = FindObjectOfType<TeamFleetHUD>();
		if (teamFleetHud != null)
			teamFleetHud.SetHealthBarValue(spacecraft);
	}

	public int GetHealth() { return currentHealth; }
}
