using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
	public int maxHealth = 10;

	private Spacecraft spacecraft;
	private Agent agent;
	private TeamFleetHUD teamHud;
	private BattleOutcome battleOutcome;
	private int currentHealth = -1;

    void Start()
    {
		currentHealth = maxHealth;
		spacecraft = GetComponent<Spacecraft>();
		agent = GetComponent<Agent>();
		teamHud = FindObjectOfType<TeamFleetHUD>();
		battleOutcome = FindObjectOfType<BattleOutcome>();
	}

	public void ModifyHealth(int value, Transform responsibleTransform)
	{
		int damage = Mathf.Clamp(value, -(maxHealth + 1), maxHealth + 1);
		currentHealth += damage;

		if (teamHud != null)
			teamHud.SetHealthBarValue(spacecraft, Mathf.Floor(currentHealth) / Mathf.Floor(maxHealth));

		if (currentHealth <= 0)
		{
			currentHealth = 0;
			spacecraft.SpacecraftDestroyed(responsibleTransform);
		}
		
		if (spacecraft.GetAgent().teamID == 0)
		{
			battleOutcome.AddLost(damage);
		}
		else if (spacecraft.GetAgent().teamID == 1)
		{
			battleOutcome.AddScore(damage);
		}
	}

	public int GetHealth() { return currentHealth; }
}
