using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveType : MonoBehaviour
{
	public string objectiveName = "";
	public int objectiveValue = 100;
	public string objectiveDescription = "";
	public Sprite objectiveIcon;
	public int objectiveRating = 1;

	private ObjectiveHUD objectiveHud;
	private NameLibrary nameLibrary;
	private Vector3[] offsets;
	private Quaternion[] rotations;
	private bool bSpawnOnLoad = true;
	private bool bLoaded = false;
	private List<ObjectiveElement> objectiveElementList;

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		//
		objectiveElementList = new List<ObjectiveElement>();
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

	public void SetValue(int value)
	{
		objectiveValue = value;
	}

	public virtual void SetElements(List<ObjectiveElement> elementList, Vector3[] os, Quaternion[] rs)
	{
		objectiveElementList.Clear();
		offsets = os;
		rotations = rs;
		List<ObjectiveElement> elementGameObjectPrefabList = new List<ObjectiveElement>();
		foreach(ObjectiveElement oe in elementList)
		{
			if (oe.elementPrefab != null)
				elementGameObjectPrefabList.Add(oe);
		}
		if (elementGameObjectPrefabList.Count > 0)
			objectiveElementList = elementGameObjectPrefabList;
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
		int numObjects = objectiveElementList.Count;
		for(int i = 0; i < numObjects; i++)
		{
			ObjectiveElement oe = Instantiate(objectiveElementList[i]);
			Vector3 os = offsets[i];
			Quaternion rs = rotations[i];
			oe.Init(os, rs);
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
