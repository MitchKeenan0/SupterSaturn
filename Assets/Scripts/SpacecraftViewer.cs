using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacecraftViewer : MonoBehaviour
{
	public GameObject[] displayPrefabs;
	public Transform spawnWidget;

	private List<GameObject> displayList;
	private GameObject currentObject;

    void Start()
    {
		displayList = new List<GameObject>();
		if (spawnWidget == null)
			spawnWidget = FindObjectOfType<SpawnPositioner>().transform;
		InitModels();
		DisplaySpacecraft(0);
    }

	void InitModels()
	{
		int numPrefabs = displayPrefabs.Length;
		for(int i = 0; i < numPrefabs; i++)
		{
			GameObject displayObj = Instantiate(displayPrefabs[i], spawnWidget.position, spawnWidget.rotation);
			displayObj.transform.SetParent(spawnWidget);
			displayList.Add(displayObj);
			displayObj.SetActive(false);
		}
	}

	public void DisplaySpacecraft(int index)
	{
		if (currentObject != null)
			currentObject.SetActive(false);
		GameObject displayObject = displayList[index];
		displayObject.SetActive(true);
		currentObject = displayObject;
	}
}
