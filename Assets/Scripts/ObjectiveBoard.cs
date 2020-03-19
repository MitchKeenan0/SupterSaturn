using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveBoard : MonoBehaviour
{
	public ObjectiveType[] objectiveTypeList;
	public GameObject objectiveListingPrefab;
	public GameObject listingPanel;
	public GameObject theWholeThing;

	private List<ObjectiveListing> objectiveList;

	void Start()
    {
		objectiveList = new List<ObjectiveListing>();
		InitListingBoard();
		SetActive(false);
	}

	void InitListingBoard()
	{
		for(int i = 0; i < 5; i++)
		{
			ObjectiveListing listing = SpawnListing();
			ObjectiveType type = null;
			bool bTypeSet = false;
			int counter = 0;
			while (!bTypeSet && (counter < 100))
			{
				counter++;
				int randomType = Random.Range(0, objectiveTypeList.Length - 1);
				type = objectiveTypeList[randomType];
				int typeRating = type.objectiveRating;
				int indexRating = Mathf.FloorToInt(Mathf.Sqrt(randomType)) + 1;
				if (typeRating <= indexRating)
				{
					if (objectiveTypeList.Length > randomType)
						bTypeSet = true;
				}
			}
			if (type != null)
				listing.SetObjective(type);
		}
	}

	ObjectiveListing SpawnListing()
	{
		GameObject listing = Instantiate(objectiveListingPrefab, listingPanel.transform);
		ObjectiveListing objListing = listing.GetComponent<ObjectiveListing>();
		objectiveList.Add(objListing);
		return objListing;
	}

	public void SetActive(bool value)
	{
		theWholeThing.SetActive(value);
	}

	public void Back()
	{
		FindObjectOfType<MainMenu>().ObjectiveBoard(false);
	}
}
