using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFleet : MonoBehaviour
{
	public int teamID = 0;
	public bool bSphericalPositions = true;
	public Vector3[] groupPositions;

	private Game game;
	private Player player;
	private CardToSpacecraftLoader cardLoader;

	private IEnumerator loadWaitCoroutine;
    
	void Awake()
	{
		game = FindObjectOfType<Game>();
		player = FindObjectOfType<Player>();
		cardLoader = FindObjectOfType<CardToSpacecraftLoader>();
	}

    void Start()
    {
		loadWaitCoroutine = LoadWait(0.1f);
		StartCoroutine(loadWaitCoroutine);
	}

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		InitSavedFleet();
	}

	void InitSavedFleet()
	{
		List<Spacecraft> spacecraftList = new List<Spacecraft>(game.GetSpacecraftList());
		if (teamID != 0)
			spacecraftList = game.GetEnemySpacecraftList();

		int numCards = spacecraftList.Count;
		for (int i = 0; i < numCards; i++)
		{
			Spacecraft sp = spacecraftList[i];
			if (sp != null)
			{
				Vector3 amidstPosition = Vector3.forward * -i;
				if (groupPositions.Length > i)
					amidstPosition = groupPositions[i];
				Vector3 worldPosition = transform.position;
				if (bSphericalPositions)
					worldPosition = Random.onUnitSphere * transform.position.z;
				sp.transform.position = worldPosition + amidstPosition;
				sp.transform.SetParent(transform);

				Card spCard = null;
				if (teamID == 0)
					spCard = game.GetSelectedCard();
				else
					spCard = game.cardLibrary[Random.Range(0, game.npcCardLibrary.Length - 1)];
				cardLoader.UploadCardToSpacecraft(spCard, sp);

				sp.gameObject.SetActive(true);

				if (sp.GetAgent() != null)
				{
					sp.GetAgent().teamID = teamID;
				}
			}
		}
	}
}
