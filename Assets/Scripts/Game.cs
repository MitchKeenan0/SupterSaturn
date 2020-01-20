using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
	private Fleet playerFleet;
	private Fleet enemyFleet;

	//private int currentMission = -1;

	void Start()
	{
		
	}

	public void FleetCreator()
	{
		SceneManager.LoadScene("FleetScene");
	}

	public void StartGame()
	{
		SceneManager.LoadScene("CampaignScene");
	}

	public void SetPlayerFleet(Fleet fleet)
	{
		playerFleet = fleet;
	}

	public void SetEnemyFleet(Fleet fleet)
	{
		enemyFleet = fleet;
	}

	public void SaveGame()
	{
		// 1
		Save save = CreateSaveGameObject();

		// 2
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
		bf.Serialize(file, save);
		file.Close();

		// 3
		//currentMission = -1;
		//shotsText.text = "Shots: " + shots;
		//hitsText.text = "Hits: " + hits;

		//ClearRobots();
		//ClearBullets();
		Debug.Log("Game Saved");
	}

	public void LoadGame()
	{
		// 1
		if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
		{
			//ClearBullets();
			//ClearRobots();
			//RefreshRobots();

			// 2
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
			Save save = (Save)bf.Deserialize(file);
			file.Close();

			// 3
			//for (int i = 0; i < save.livingTargetPositions.Count; i++)
			//{
			//	int position = save.livingTargetPositions[i];
			//	Target target = targets[position].GetComponent<Target>();
			//	target.ActivateRobot((RobotTypes)save.livingTargetsTypes[i]);
			//	target.GetComponent<Target>().ResetDeathTimer();
			//}

			// 4
			//shotsText.text = "Shots: " + save.shots;
			//hitsText.text = "Hits: " + save.hits;
			//shots = save.shots;
			//hits = save.hits;

			Debug.Log("Game Loaded");

			//Unpause();
		}
		else
		{
			Debug.Log("No game saved!");
		}
	}

	private Save CreateSaveGameObject()
	{
		Save save = new Save();

		//foreach (GameObject targetGameObject in targets)
		//{
		//	Target target = targetGameObject.GetComponent<Target>();
		//	if (target.activeRobot != null)
		//	{
		//		save.livingTargetPositions.Add(target.position);
		//		save.livingTargetsTypes.Add((int)target.activeRobot.GetComponent<Robot>().type);
		//	}
		//}

		//save.hits = hits;
		//save.shots = shots;

		return save;
	}

	public void RestartLevel()
	{
		SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
	}
}
