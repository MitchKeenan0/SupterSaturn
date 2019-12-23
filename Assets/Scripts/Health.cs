using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
	public int maxHealth = 10;

	private Spacecraft spacecraft;
	private Agent agent;
	private int currentHealth = -1;

    void Start()
    {
		currentHealth = maxHealth;
		spacecraft = GetComponent<Spacecraft>();
		agent = GetComponent<Agent>();
	}

	public void ModifyHealth(int value, Transform responsibleTransform)
	{
		currentHealth += value;
		if (currentHealth <= 0)
		{
			currentHealth = 0;
			spacecraft.SpacecraftDestroyed(responsibleTransform);
		}

		if (value < 0f && (agent != null))
		{

		}
	}

	public int GetHealth() { return currentHealth; }
}
