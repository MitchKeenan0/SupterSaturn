using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFleet : MonoBehaviour
{
	public int teamID = 0;
	public Vector3[] groupPositions;

	private Game game;
	private Player player;

	private IEnumerator loadWaitCoroutine;
    
	void Awake()
	{
		game = FindObjectOfType<Game>();
		player = FindObjectOfType<Player>();
	}

    void Start()
    {
		game.LoadGame();
		loadWaitCoroutine = LoadWait(0.1f);
		StartCoroutine(loadWaitCoroutine);
	}

	void InitFleetSpacecraft()
	{
		if (teamID == 0)
		{
			List<Spacecraft> spacecraftList = new List<Spacecraft>(game.GetSpacecraftList());
			if (spacecraftList.Count > 0)
			{
				for (int i = 0; i < spacecraftList.Count; i++)
				{
					Spacecraft sp = spacecraftList[i];
					if ((sp != null) && (groupPositions.Length > i))
					{
						sp.transform.position = groupPositions[i];
						sp.gameObject.SetActive(true);
						sp.transform.SetParent(transform);
					}
				}
			}
		}
	}

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		InitFleetSpacecraft();
	}
}
