using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveListing : MonoBehaviour
{
	public Text valueText;
	public GameObject ratingPanel;
	public GameObject ratingStarPrefab;
	public Transform surroundingsPanel;
	public Transform primaryT;
	public Transform secondaryT;

	public ObjectiveType objectiveType;
	private MainMenu menu;
	private CanvasGroup surroundingsCanvasGroup;
	private HudController hudController;

	public string objectiveName = "";
	private Sprite objectiveIcon;
	public int objectiveValue = 0;
	public int objectiveRating = 1;
	private List<GameObject> ratingStarList;
	public ObjectiveElement primaryObjElement;
	public List<ObjectiveElement> surroundingsElementList;
	private List<Vector3> offsetList;
	private List<Quaternion> rotationList;

	void Awake()
	{
		ratingStarList = new List<GameObject>();
		offsetList = new List<Vector3>();
		rotationList = new List<Quaternion>();
		if (surroundingsElementList == null)
			surroundingsElementList = new List<ObjectiveElement>();
		menu = FindObjectOfType<MainMenu>();
		surroundingsCanvasGroup = surroundingsPanel.GetComponent<CanvasGroup>();
		SetCanvasGroupEnabled(false);
	}

	void InitRatingStars(int value)
	{
		if (ratingStarList == null)
			ratingStarList = new List<GameObject>();
		if (ratingStarList.Count < value)
		{
			for (int i = 0; i < value; i++)
			{
				GameObject star = Instantiate(ratingStarPrefab, ratingPanel.transform);
				ratingStarList.Add(star);
			}
		}
	}

	public void SetCanvasGroupEnabled(bool value)
	{
		surroundingsCanvasGroup.alpha = value ? 1f : 0f;
		surroundingsCanvasGroup.blocksRaycasts = value;
		surroundingsCanvasGroup.interactable = value;
	}

	public void SelectObjective()
	{
		SetCanvasGroupEnabled(true);
	}

	public void LaunchObjective()
	{
		ObjectiveType spawnedObjective = Instantiate(objectiveType, null);
		if (surroundingsElementList.Count > 0)
		{
			surroundingsElementList.Add(primaryObjElement);

			ObjectiveSurroundingsIcon[] icons = surroundingsPanel.GetComponentsInChildren<ObjectiveSurroundingsIcon>();
			foreach(ObjectiveSurroundingsIcon osi in icons)
			{
				Vector3 osiPosition = osi.startOrbitOffset;
				offsetList.Add(osiPosition);
				Quaternion osiRotation = osi.startOrbitRotation;
				rotationList.Add(osiRotation);

				//ObjectiveElement objE = osi.objectiveElementPrefab;
				//surroundingsElementList.Add(objE);
			}

			Vector3[] offsets = offsetList.ToArray();
			Quaternion[] rotations = rotationList.ToArray();
			spawnedObjective.SetElements(surroundingsElementList, offsets, rotations);
			spawnedObjective.Activate();

			menu.StartGame();
		}
	}
}
