using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
	public int maxHealth = 10;

	private Spacecraft spacecraft;
	private Agent agent;
	private TeamFleetHUD teamHud;
	private int currentHealth = -1;

    void Start()
    {
		currentHealth = maxHealth;
		spacecraft = GetComponent<Spacecraft>();
		agent = GetComponent<Agent>();
		teamHud = FindObjectOfType<TeamFleetHUD>();
	}

	public void ModifyHealth(int value, Transform responsibleTransform)
	{
		currentHealth += value;

		if (teamHud != null)
			teamHud.SetHealthBarValue(spacecraft, Mathf.Floor(currentHealth) / Mathf.Floor(maxHealth));

		if (currentHealth <= 0)
		{
			currentHealth = 0;
			spacecraft.SpacecraftDestroyed(responsibleTransform);
		}
	}

	public int GetHealth() { return currentHealth; }
}
