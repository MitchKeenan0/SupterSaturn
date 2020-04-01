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
				int randomType = Random.Range(1, objectiveTypeList.Length) - 1;
				type = objectiveTypeList[randomType];

				bTypeSet = true;
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

	public void Refresh()
	{
		int numListings = objectiveList.Count;
		for (int i = 0; i < numListings; i++)
		{
			ObjectiveListing listing = objectiveList[i];
			ObjectiveType type = null;
			bool bTypeSet = false;
			int counter = 0;
			while (!bTypeSet && (counter < 100))
			{
				counter++;
				int randomType = Random.Range(1, objectiveTypeList.Length) - 1;
				type = objectiveTypeList[randomType];
				bTypeSet = true;
			}
			if (type != null)
				listing.SetObjective(type);
		}
	}
}
