using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftDataHUD : MonoBehaviour
{
	private ObjectManager objectManager;
	private List<Spacecraft> spacecraftList;
	private List<SpacecraftInformation> informationList;

    void Start()
    {
		objectManager = FindObjectOfType<ObjectManager>();
		spacecraftList = objectManager.GetSpacecraftList();
		informationList = new List<SpacecraftInformation>();
    }

	
}
