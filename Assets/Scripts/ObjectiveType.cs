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

	private ObjectiveHUD objectiveHud;
	private NameLibrary nameLibrary;
	private bool bSpawnOnLoad = true;
	private bool bLoaded = false;
	private List<GameObject> elementPrefabList;

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		//
		elementPrefabList = new List<GameObject>();
		objectiveHud = FindObjectOfType<ObjectiveHUD>();
		nameLibrary = FindObjectOfType<NameLibrary>();
		//if (!bLoaded)
		//	LoadObjective();
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
		if (bSpawnOnLoad && !bLoaded)
			LoadObjective();
	}

	public virtual void SetElements(List<ObjectiveElement> elementList)
	{
		elementPrefabList.Clear();
		List<GameObject> elementGameObjectPrefabList = new List<GameObject>();
		foreach(ObjectiveElement oe in elementList)
		{
			if (oe.elementPrefab != null)
				elementGameObjectPrefabList.Add(oe.elementPrefab);
		}
		if (elementGameObjectPrefabList.Count > 0)
			elementPrefabList = elementGameObjectPrefabList;
	}

	public virtual void LoadObjective()
	{
		bLoaded = true;
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
		int numObjects = elementPrefabList.Count;
		for(int i = 0; i < numObjects; i++)
		{
			GameObject ep = Instantiate(elementPrefabList[i]);
			ep.transform.SetParent(transform);
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
