using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveBoard : MonoBehaviour
{
	public ObjectiveType[] objectiveTypeArray;
	public GameObject[] objectiveListingPrefabs;
	public GameObject listingPanel;
	public GameObject theWholeThing;

	private List<ObjectiveListing> objectiveList;
	private int listingCount = 0;

	void Start()
    {
		objectiveList = new List<ObjectiveListing>();
		InitListingBoard();
		SetActive(false);
	}

	void InitListingBoard()
	{
		listingCount = objectiveListingPrefabs.Length;
		for(int i = 0; i < listingCount; i++)
		{
			ObjectiveListing listing = SpawnListing(i);

			/// factions
			//ObjectiveType type = null;
			//bool bTypeSet = false;
			//int counter = 0;
			//while (!bTypeSet && (counter < 100))
			//{
			//	counter++;
			//	int randomType = Random.Range(1, objectiveTypeArray.Length) - 1;
			//	if (randomType < objectiveTypeArray.Length)
			//	{
			//		type = objectiveTypeArray[randomType];
			//		if (type != null)
			//			bTypeSet = true;
			//	}
			//}
			//if (type != null)
			//{
			//	listing.SetObjective(type);
			//	///listing.SetSurroundings(osiList);
			//}
		}
	}

	ObjectiveListing SpawnListing(int prefabIndex)
	{
		GameObject listing = Instantiate(objectiveListingPrefabs[prefabIndex], listingPanel.transform);
		RectTransform rt = listing.GetComponent<RectTransform>();
		float marginX = Screen.width * 0.2f;
		float marginY = Screen.height * 0.2f;
		float x = Random.Range(marginX, Screen.width - marginX);
		float y = Random.Range(marginY, Screen.height - marginY);
		rt.transform.position = new Vector3(x, y, 0);
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
		
	}
}
