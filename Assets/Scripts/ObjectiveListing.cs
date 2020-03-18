using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveListing : MonoBehaviour
{
	public Text nameText;
	public Image iconImage;
	public GameObject ratingPanel;
	public GameObject ratingStarPrefab;

	private ObjectiveType objectiveType;
	private MainMenu menu;
	private string objectiveName = "";
	private Sprite objectiveIcon;
	private int objectiveRating = 1;
	private List<GameObject> ratingStarList;

	void Awake()
	{
		ratingStarList = new List<GameObject>();
		menu = FindObjectOfType<MainMenu>();
	}

	public void SetObjective(ObjectiveType obj)
	{
		objectiveType = obj;
		objectiveName = objectiveType.objectiveName;
		objectiveIcon = objectiveType.objectiveIcon;
		objectiveRating = objectiveType.objectiveRating;

		nameText.text = objectiveName;
		iconImage.sprite = objectiveIcon;
		FillRatings(objectiveRating);
	}

	void FillRatings(int value)
	{
		if (ratingStarList == null)
			ratingStarList = new List<GameObject>();
		for (int i = 0; i < value; i++)
		{
			GameObject star = Instantiate(ratingStarPrefab, ratingPanel.transform);
			ratingStarList.Add(star);
		}
	}

	public void SelectObjective()
	{
		ObjectiveType spawnedObjective = Instantiate(objectiveType, null);
		spawnedObjective.Activate();
		menu.StartGame();
	}
}
