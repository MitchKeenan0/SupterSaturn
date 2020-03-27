using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpacecraft : MonoBehaviour
{
	public float updateInterval = 0.1f;

	private Spacecraft spacecraft;
	private Agent agent;
	private IEnumerator updateCoroutine;

    void Start()
    {
		spacecraft = GetComponent<Spacecraft>();
		agent = GetComponent<Agent>();
		InitNPCAgent();
		updateCoroutine = UpdateNPCSpacecraft(updateInterval);
		StartCoroutine(updateCoroutine);
    }

	void InitNPCAgent()
	{
		agent.Scan();
	}

	IEnumerator UpdateNPCSpacecraft(float interval)
	{
		while(true)
		{
			spacecraft.Brake();
			yield return new WaitForSeconds(interval);
		}
	}
}
