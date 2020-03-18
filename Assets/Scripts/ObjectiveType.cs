using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveType : MonoBehaviour
{
	public string objectiveName = "";
	public string objectiveDesctription = "";
	public Sprite objectiveIcon;
	public int objectiveRating = 1;
	public GameObject[] principleObjectPrefabs;

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
			objectiveHud.objectiveText.text = objectiveDesctription;
	}

	public virtual void SpawnPrincipleObjects()
	{
		int numObjects = principleObjectPrefabs.Length;
		for(int i = 0; i < numObjects; i++)
		{
			GameObject po = Instantiate(principleObjectPrefabs[i], transform);
		}
	}

	public void Activate()
	{
		bSpawnOnLoad = true;
	}
}
