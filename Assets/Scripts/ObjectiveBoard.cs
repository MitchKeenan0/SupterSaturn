using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveBoard : MonoBehaviour
{
	public ObjectiveType[] objectiveTypeArray;
	public GameObject objectiveListingPrefab;
	public GameObject listingPanel;
	public GameObject theWholeThing;

	private ObjectiveSurroundings surroundings;
	private List<ObjectiveListing> objectiveList;

	void Start()
    {
		objectiveList = new List<ObjectiveListing>();
		surroundings = GetComponent<ObjectiveSurroundings>();
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
				int randomType = Random.Range(1, objectiveTypeArray.Length) - 1;
				if (randomType < objectiveTypeArray.Length)
				{
					type = objectiveTypeArray[randomType];
					if (type != null)
						bTypeSet = true;
				}
			}

			if (type != null)
			{
				listing.SetObjective(type);

				surroundings.CreateSurroundings(type.objectiveRating);
				List<ObjectiveSurroundingsIcon> osiList = new List<ObjectiveSurroundingsIcon>();
				osiList = surroundings.GetSurroundings();
				listing.SetSurroundings(osiList);
			}
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

	public void Refresh()
	{
		int numListings = objectiveList.Count;
		objectiveList.Clear();
		InitListingBoard();
		for (int i = 0; i < numListings; i++)
		{
			ObjectiveListing listing = objectiveList[i];
			if (listing != null)
			{
				ObjectiveType type = null;
				bool bTypeSet = false;
				int counter = 0;
				while (!bTypeSet && (counter < 100))
				{
					counter++;
					int randomType = Random.Range(1, objectiveTypeArray.Length) - 1;
					type = objectiveTypeArray[randomType];
					bTypeSet = true;
				}
				if (type != null)
					listing.SetObjective(type);

				surroundings.CreateSurroundings(type.objectiveRating);
				List<ObjectiveSurroundingsIcon> osiList = new List<ObjectiveSurroundingsIcon>();
				osiList = surroundings.GetSurroundings();
				listing.SetSurroundings(osiList);
			}
		}
	}
}
