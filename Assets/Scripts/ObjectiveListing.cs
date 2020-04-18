using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveListing : MonoBehaviour
{
	public Text nameText;
	public GameObject ratingPanel;
	public GameObject ratingStarPrefab;
	public Transform primaryT;
	public Transform secondaryT;
	public Transform salientT;

	private ObjectiveType objectiveType;
	private MainMenu menu;
	private string objectiveName = "";
	private Sprite objectiveIcon;
	private int objectiveRating = 1;
	private List<GameObject> ratingStarList;
	private List<ObjectiveElement> surroundingsElementList;

	void Awake()
	{
		ratingStarList = new List<GameObject>();
		if (surroundingsElementList == null)
			surroundingsElementList = new List<ObjectiveElement>();
		menu = FindObjectOfType<MainMenu>();
	}

	public void SetObjective(ObjectiveType obj)
	{
		objectiveType = obj;
		objectiveName = objectiveType.objectiveName;
		objectiveIcon = objectiveType.objectiveIcon;
		objectiveRating = objectiveType.objectiveRating;

		nameText.text = objectiveName;
		FillRatings(objectiveRating);
	}

	public void SetSurroundings(List<ObjectiveSurroundingsIcon> iconList)
	{
		int listCount = iconList.Count;
		List<ObjectiveSurroundingsIcon> secondaryIcons = new List<ObjectiveSurroundingsIcon>();
		List<ObjectiveSurroundingsIcon> rareSecondaries = new List<ObjectiveSurroundingsIcon>();
		if (surroundingsElementList == null)
			surroundingsElementList = new List<ObjectiveElement>();

		/// initial sorting
		for (int i = 0; i < listCount; i++)
		{
			ObjectiveSurroundingsIcon osi = iconList[i];
			surroundingsElementList.Add(osi.objectiveElementPrefab);

			if (i == 0)
			{
				osi.transform.SetParent(primaryT);
			}
			else
			{
				if (osi.bSecondary)
				{
					if (osi.rarity < 1f)
						rareSecondaries.Add(osi);
					else
						secondaryIcons.Add(osi);
				}
				else
				{
					if (osi.bSalient)
						osi.transform.SetParent(salientT);
					else
						osi.transform.SetParent(secondaryT);
				}
			}
			osi.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		}

		/// normal secondaries
		if (secondaryIcons.Count > 0)
		{
			for (int j = 0; j < secondaryIcons.Count; j++)
			{
				ObjectiveSurroundingsIcon sosi = secondaryIcons[j];
				sosi.transform.SetParent(secondaryT);
				sosi.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			}
			secondaryIcons.Clear();
		}

		/// rare secondaries
		if (rareSecondaries.Count > 0)
		{
			for (int j = 0; j < rareSecondaries.Count; j++)
			{
				ObjectiveSurroundingsIcon rosi = rareSecondaries[j];
				rosi.transform.SetParent(secondaryT);
				rosi.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			}
			rareSecondaries.Clear();
		}
	}

	void FillRatings(int value)
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

	public void SelectObjective()
	{
		ObjectiveType spawnedObjective = Instantiate(objectiveType, null);
		if (surroundingsElementList.Count > 0)
		{
			spawnedObjective.SetElements(surroundingsElementList);
			spawnedObjective.Activate();
			menu.StartGame();
		}
	}
}
