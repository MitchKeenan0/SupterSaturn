using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
	public List<Spacecraft> GetSpacecraftList() { return spacecraftList; }
	private List<Spacecraft> spacecraftList;

	public List<Gravity> GetGravityList() { return gravityList; }
	private List<Gravity> gravityList;

	private Game game;
	private CraftIconHUD hud;
	private BattleOutcome battleOutcome;
	private GameAgent gameAgent;

	private IEnumerator loadWaitCoroutine;

	void Awake()
	{
		spacecraftList = new List<Spacecraft>();
		game = FindObjectOfType<Game>();
		gameAgent = FindObjectOfType<GameAgent>();
	}

	void Start()
	{
		game.LoadGame();
		hud = FindObjectOfType<CraftIconHUD>();
		battleOutcome = FindObjectOfType<BattleOutcome>();
		loadWaitCoroutine = LoadWait(0.2f);
		StartCoroutine(loadWaitCoroutine);
	}

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		InitSpacecraftList();
		InitGravityList();
		if (gameAgent != null)
			gameAgent.SetAgentList(game.GetEnemySpacecraftList());
	}

	void InitSpacecraftList()
	{
		spacecraftList.Clear();
		Spacecraft[] allSpacecraft = FindObjectsOfType<Spacecraft>();
		foreach (Spacecraft sp in allSpacecraft)
			spacecraftList.Add(sp);
		///Debug.Log("object manager reading list " + allSpacecraft.Length);
	}

	void InitGravityList()
	{
		gravityList = new List<Gravity>();
		Gravity[] gr = FindObjectsOfType<Gravity>();
		foreach (Gravity g in gr)
		{
			gravityList.Add(g);
		}
	}

	void TestBattleConcluded()
	{
		int numSpacecraft = spacecraftList.Count;
		int numTeam = 0;
		int numEnemy = 0;

		for (int i = 0; i < numSpacecraft; i++)
		{
			Spacecraft sp = spacecraftList[i];
			if ((sp != null) && (sp.IsAlive()))
			{
				if (sp.GetAgent().teamID == 0)
					numTeam++;
				else if (sp.GetAgent().teamID == 1)
					numEnemy++;
			}
		}

		if (Time.timeSinceLevelLoad > 5f)
		{
			if (numTeam == 0)
				battleOutcome.BattleOver(false);
			else if (numEnemy == 0)
				battleOutcome.BattleOver(true);
		}
	}

	public void SpacecraftDestroyed(Spacecraft value)
	{
		hud.ModifySpacecraftList(value, false);
		spacecraftList.Remove(value);
		TestBattleConcluded();
	}
}
