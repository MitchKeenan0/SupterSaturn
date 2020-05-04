using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFleet : MonoBehaviour
{
	public int teamID = 0;
	public bool bSphericalPositions = true;
	public Vector3[] groupPositions;

	private Game game;
	private TeamFleetHUD teamHud;
	private Player player;
	private CardToSpacecraftLoader cardLoader;

	private IEnumerator loadWaitCoroutine;
    
	void Awake()
	{
		game = FindObjectOfType<Game>();
		teamHud = FindObjectOfType<TeamFleetHUD>();
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
		Spacecraft sp = game.GetSpacecraftList()[0];
		if (sp != null)
		{
			Card spCard = null;

			if (teamID == 0)
				spCard = game.GetSelectedCard();
			else
				spCard = game.cardLibrary[Random.Range(0, game.npcCardLibrary.Length - 1)];
			cardLoader.UploadCardToSpacecraft(spCard, sp);

			sp.gameObject.SetActive(true);

			if (sp.GetAgent() != null)
				sp.GetAgent().teamID = teamID;
		}
	}
}
