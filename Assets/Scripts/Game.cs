using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
	public static Game game;
	public Card[] cardLibrary;
	public Card[] npcCardLibrary;
	public List<Card> initialSpacecraftCards;
	public int initialChevrons = 200;

	private Player player;
	private GameHUD gameHud;
	private Campaign campaign;
	private GameObject ghostCampaign;
	private Card playerSpacecraftCard;
	private List<Card> enemyCardList;
	private List<Spacecraft> spacecraftList;
	private List<Spacecraft> enemySpacecraftList;
	private int savedHealth = 0;
	private int sceneSetting = 0;
	private int chevrons = 0;
	private int gameMode = -1;
	private bool bEscapeMenu = false;
	private bool bFleetTutorialClosed = false;
	private bool bGameTutorialClosed = false;

	public Card GetSelectedCard() { return playerSpacecraftCard; }
	public List<Card> GetEnemyCards() { return enemyCardList; }
	public List<Spacecraft> GetSpacecraftList() { return spacecraftList; }
	public List<Spacecraft> GetEnemySpacecraftList() { return enemySpacecraftList; }
	public int GetSavedHealth() { return savedHealth; }
	public int GetChevrons() { return chevrons; }
	public int GetSceneSetting() { return sceneSetting; }
	public void UpdateChevronAccount(int value) { chevrons += value; }

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}
	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}
	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		enemyCardList = new List<Card>();
		spacecraftList = new List<Spacecraft>();
		enemySpacecraftList = new List<Spacecraft>();
		player = FindObjectOfType<Player>();
		gameHud = FindObjectOfType<GameHUD>();
		campaign = FindObjectOfType<Campaign>();
	}

	void Awake()
	{
		if (game == null)
		{
			DontDestroyOnLoad(gameObject);
			game = this;
		}
		else if (game != this)
		{
			Destroy(gameObject);
		}

		enemyCardList = new List<Card>();
		spacecraftList = new List<Spacecraft>();
		enemySpacecraftList = new List<Spacecraft>();
		player = FindObjectOfType<Player>();
		gameHud = FindObjectOfType<GameHUD>();
		campaign = FindObjectOfType<Campaign>();
		Application.targetFrameRate = 30;
	}

	void Start()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	public int GetGameMode() { return gameMode; }
	public void SetGameMode(int value)
	{
		gameMode = value;
	}

	public void LoadScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	public void LoadGameMode()
	{
		string sceneName = "";
		switch(gameMode)
		{
			case 1: sceneName = "BattleScene";
				break;
			case 2: sceneName = "BayScene";
				break;
			default:
				break;
		}
		if (sceneName != "")
			SceneManager.LoadScene(sceneName);
	}

	public void EscapeMenu()
	{
		bEscapeMenu = !bEscapeMenu;
		if (gameHud != null)
			gameHud.SetEscapeMenuActive(bEscapeMenu);
		else
			Debug.Log("no game menu");
		if (bEscapeMenu)
			Time.timeScale = Time.deltaTime * 5f;
		else
			Time.timeScale = 1f;
	}

	public void SetSelectedCard(Card card)
	{
		playerSpacecraftCard = card;
	}

	public void SetPlayerSpacecraft(Spacecraft sp)
	{
		if (spacecraftList == null)
			spacecraftList = new List<Spacecraft>();
		if (spacecraftList.Count > 0)
			spacecraftList[0] = sp;
		else
			spacecraftList.Add(sp);
	}

	public void SetSavedHealth(int value)
	{
		savedHealth = value;
	}

	public void SetEnemyCard(int index, Card card)
	{
		if (index < enemyCardList.Count)
			enemyCardList[index] = card;
		else if (card != null)
			enemyCardList.Add(card);
	}

	public void AddEnemy(Spacecraft sp)
	{
		if (!enemySpacecraftList.Contains(sp))
			enemySpacecraftList.Add(sp);
	}

	public void RemoveEnemy(Spacecraft sp)
	{
		if (enemySpacecraftList.Contains(sp))
			enemySpacecraftList.Remove(sp);
	}

	public void SetSceneSetting(int sceneTypeID)
	{
		sceneSetting = sceneTypeID;
	}

	public void LoadGame()
	{
		if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
			Save save = (Save)bf.Deserialize(file);
			file.Close();

			if (enemyCardList != null)
			{
				enemyCardList.Clear();
				spacecraftList.Clear();
				enemySpacecraftList.Clear();
			}

			// player name
			if (player != null)
			{
				string playerName = save.playerName;
				if (playerName == "")
					playerName = "No Name";
				player.SetName(playerName);
			}

			// player spacecraft card
			int selectedCardID = save.playerSpacecraftCard;
			Card selectedCard = cardLibrary[selectedCardID];
			playerSpacecraftCard = selectedCard;
			SetSelectedCard(selectedCard);
			Spacecraft sp = CreateSpacecraft(selectedCard);
			SetPlayerSpacecraft(sp);

			// player health
			int savedHealth = save.playerHealth;
			if ((savedHealth == -1) && (spacecraftList != null) && (spacecraftList.Count > 0))
				savedHealth = sp.GetComponent<Health>().maxHealth;
			if (savedHealth == 0)
				savedHealth = playerSpacecraftCard.health;
			SetSavedHealth(savedHealth);
			int maxHealth = sp.GetHealth();
			if (savedHealth < maxHealth)
			{
				int spawnDamage = savedHealth - maxHealth;
				sp.GetComponent<Health>().ModifyHealth(spawnDamage, null);
			}

			// chevrons
			chevrons = save.chevrons;
			if (chevrons < initialChevrons)
				chevrons = initialChevrons;

			// tutorial
			bFleetTutorialClosed = save.fleetTutorialClosed;
			bGameTutorialClosed = save.gameTutorialClosed;
			gameHud = FindObjectOfType<GameHUD>();
			if (gameHud != null)
			{
				string sceneName = SceneManager.GetActiveScene().name;
				if (sceneName == "FleetScene")
				{
					if (bFleetTutorialClosed)
						gameHud.SetTutorialActive(false);
					//else
					//	gameHud.SetTutorialActive(true);
				}
				else if (sceneName == "GameScene")
				{
					if (bGameTutorialClosed)
						gameHud.SetTutorialActive(false);
					//else
					//	gameHud.SetTutorialActive(true);
				}
			}

			Debug.Log("Game Loaded");
		}
		else
		{
			Debug.Log("Load found no save data");
		}
	}

	public void CloseTutorial()
	{
		string sceneName = SceneManager.GetActiveScene().name;
		if (sceneName == "FleetScene")
			bFleetTutorialClosed = true;
		else if (sceneName == "GameScene")
			bGameTutorialClosed = true;
		SaveGame();
	}

	Spacecraft CreateSpacecraft(Card card)
	{
		Spacecraft sp = null;
		if (card != null)
		{
			GameObject spacecraftObject = Instantiate(card.cardObjectPrefab, null);
			spacecraftObject.SetActive(false);
			sp = spacecraftObject.GetComponent<Spacecraft>();
			Debug.Log("Spawned player spacecraft");
		}
		return sp;
	}

	public void SaveGame()
	{
		Save save = CreateSaveGameObject();
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
		bf.Serialize(file, save);
		file.Close();
	}

	private Save CreateSaveGameObject()
	{
		Save save = new Save();

		// player name
		if (player != null)
		{
			string playerName = player.playerName;
			if (playerName == "")
				playerName = "No Name";
			save.playerName = player.playerName;
		}

		// player spacecraft card
		if (playerSpacecraftCard == null)
			playerSpacecraftCard = initialSpacecraftCards[0];
		save.playerSpacecraftCard = playerSpacecraftCard.numericID;

		// player health
		SpacecraftEditor spacecraftEditor = FindObjectOfType<SpacecraftEditor>();
		/// there's an issue here - spacecraft list is empty for some reason.
		/// Workaround below
		if ((spacecraftList != null) && (spacecraftList.Count > 0))
		{
			Spacecraft liveSpacecraft = spacecraftList[0];
			if (liveSpacecraft.isActiveAndEnabled)
			{
				savedHealth = liveSpacecraft.GetHealth();
				if (savedHealth == -1)
					savedHealth = liveSpacecraft.GetComponent<Health>().maxHealth;
				save.playerHealth = savedHealth;
			}
			else if ((spacecraftEditor != null) || (campaign != null))
			{
				save.playerHealth = savedHealth;
			}
		}
		else if ((spacecraftEditor != null) || (campaign != null))
		{
			if (savedHealth == 0)
				savedHealth = playerSpacecraftCard.health;
			save.playerHealth = savedHealth;
		}
		else
		{
			spacecraftList = new List<Spacecraft>();
			ObjectManager objManager = FindObjectOfType<ObjectManager>();
			if (objManager != null)
			{
				List<Spacecraft> allSp = objManager.GetSpacecraftList();
				int numSp = allSp.Count;
				for(int i = 0; i < numSp; i++)
				{
					Spacecraft thisSp = allSp[i];
					if (thisSp.GetAgent() != null && (thisSp.GetAgent().teamID == 0))
						spacecraftList.Add(thisSp);
				}
				if (spacecraftList.Count > 0)
					save.playerHealth = spacecraftList[0].GetHealth();
			}
		}

		// chevrons
		if (chevrons < initialChevrons)
			chevrons = initialChevrons;
		save.chevrons = chevrons;

		// tutorial
		save.fleetTutorialClosed = bFleetTutorialClosed;
		save.gameTutorialClosed = bGameTutorialClosed;

		Debug.Log("game saved");

		return save;
	}

	public void DeleteSave()
	{
		try
		{
			File.Delete(Application.persistentDataPath + "/gamesave.save");
			File.Delete(Application.persistentDataPath + "/campaignsave.save");

			Spacecraft[] allSpacecraft = FindObjectsOfType<Spacecraft>();
			int numSp = allSpacecraft.Length;
			for (int i = 0; i < numSp; i++)
				Destroy(allSpacecraft[i]);
			
			spacecraftList = new List<Spacecraft>();
			enemyCardList = new List<Card>();
			enemySpacecraftList = new List<Spacecraft>();

			bFleetTutorialClosed = false;
			bGameTutorialClosed = false;

			SaveGame();
			LoadScene("FleetScene");
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	void OnApplicationQuit()
	{
		SaveGame();
	}
}
