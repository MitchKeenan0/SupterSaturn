﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FleetHUD : MonoBehaviour
{
	public GameObject fleetPanelPrefab;
	public Vector3 panelOffset = new Vector3(0f, 100f, 0f);

	private List<Fleet> fleetList;
	private List<FleetPanel> panelList;
	private Camera cameraMain;

	public List<Fleet> GetFleetList() { return fleetList; }

	void Start()
    {
		cameraMain = Camera.main;
		fleetList = new List<Fleet>();
		Fleet[] allFleets = FindObjectsOfType<Fleet>();
		foreach(Fleet f in allFleets)
			fleetList.Add(f);

		InitPanels();
	}

	void OnGUI()
	{
		foreach(FleetPanel panel in panelList)
		{
			UpdateScreenPosition(panel);
		}
	}

	void UpdateScreenPosition(FleetPanel panel)
	{
		if (panel.GetFleet() != null)
		{
			Vector3 fleetScreenPosition = cameraMain.WorldToScreenPoint(panel.GetFleet().transform.position);
			panel.transform.position = fleetScreenPosition + panelOffset;
		}
	}

	void InitPanels()
	{
		panelList = new List<FleetPanel>();
		panelList.Add(fleetPanelPrefab.GetComponent<FleetPanel>());
		int numFleets = fleetList.Count;
		for (int i = 0; i < numFleets; i++)
		{
			FleetPanel panel = null;
			if (panelList.Count > i)
				panel = panelList[i];
			else
				panel = CreateFleetPanel();

			Fleet panelFleet = fleetList[i];
			panel.SetFleet(panelFleet);
		}
	}

	FleetPanel CreateFleetPanel()
	{
		GameObject panelObject = Instantiate(fleetPanelPrefab, transform.position, Quaternion.identity);
		panelObject.transform.SetParent(transform);
		FleetPanel panel = panelObject.GetComponent<FleetPanel>();
		panelList.Add(panel);
		return panel;
	}
}
