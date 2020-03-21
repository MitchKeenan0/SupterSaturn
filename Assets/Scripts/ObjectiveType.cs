using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveType : MonoBehaviour
{
	public string objectiveName = "";
	public string objectiveDescription = "";
	public Sprite objectiveIcon;
	public int objectiveRating = 1;
	public GameObject[] principleObjectPrefabs;
	public bool bRandomPosition = true;
	public float spawnRange = 150f;

	private ObjectiveHUD objectiveHud;
	private NameLibrary nameLibrary;
	private bool bSpawnOnLoad = false;

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		objectiveHud = FindObjectOfType<ObjectiveHUD>();
		nameLibrary = FindObjectOfType<NameLibrary>();
	}

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
		if (bSpawnOnLoad)
			LoadObjective();
	}

	public virtual void LoadObjective()
	{
		SpawnPrincipleObjects();
		objectiveHud = FindObjectOfType<ObjectiveHUD>();
		if (objectiveHud != null)
		{
			string description = objectiveDescription;
			int charLength = description.Length;
			if (charLength > 0)
			{
				objectiveHud.objectiveText.text = objectiveDescription;
			}
		}
	}

	public virtual void SpawnPrincipleObjects()
	{
		int numObjects = principleObjectPrefabs.Length;
		for(int i = 0; i < numObjects; i++)
		{
			Vector3 spawnPosition = transform.position;
			if (bRandomPosition)
			{
				Vector3 randomSpawnPosition = Random.onUnitSphere * spawnRange;
				randomSpawnPosition.z = Mathf.Clamp(randomSpawnPosition.z, (spawnRange / 2), spawnRange);
				spawnPosition = randomSpawnPosition;
			}
			GameObject po = Instantiate(principleObjectPrefabs[i], spawnPosition, Quaternion.identity);
			po.transform.SetParent(transform);
		}
	}

	public void Activate()
	{
		bSpawnOnLoad = true;
	}

	public void SetDescription(string value)
	{
		objectiveHud.objectiveText.text = value;
	}
}
