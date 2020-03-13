using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAgent : MonoBehaviour
{
	public float updateInterval = 0.1f;
	public float intervalDeviation = 0.1f;

	private List<Agent> agentList;
	private Identity identity;
	private IEnumerator updateCoroutine;

	void Awake()
	{
		agentList = new List<Agent>();
	}

	public void SetAgentList(List<Spacecraft> spacecraftList)
	{
		if (spacecraftList.Count > 0)
		{
			foreach (Spacecraft sp in spacecraftList)
			{
				if (sp.GetAgent() != null)
					agentList.Add(sp.GetAgent());
			}

			if (agentList.Count > 0)
			{
				updateCoroutine = UpdateAgents(0.2f);
				StartCoroutine(updateCoroutine);
			}
		}
	}

	private IEnumerator UpdateAgents(float intervalTime)
	{
		while (true)
		{
			yield return new WaitForSeconds(intervalTime);
			//UpdateMoveCommands();
		}
	}

	//void UpdateMoveCommands()
	//{
	//	int randomAgentIndex = Random.Range(0, agentList.Count - 1);
	//	agentList[randomAgentIndex].SetMoveOrder(Random.insideUnitSphere, true, null);
	//}
}
