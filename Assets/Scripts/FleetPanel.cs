using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FleetPanel : MonoBehaviour
{
	public GameObject iconPrefab;
	public Transform gridLayout;

	private Fleet fleet;
	private List<Spacecraft> spacecraftList;
	private List<FleetPanelIcon> iconList;
	private Text fleetNameText = null;

	public Fleet GetFleet() { return fleet; }

    void Awake()
    {
		fleetNameText = GetComponentInChildren<Text>();
		spacecraftList = new List<Spacecraft>();
		iconList = new List<FleetPanelIcon>();
		iconList.Add(iconPrefab.GetComponent<FleetPanelIcon>());
    }

	public void SetFleet(Fleet f)
	{
		fleet = f;
		fleetNameText.text = fleet.fleetName;
		if (fleet.teamID == 1)
			fleetNameText.color = Color.red;
		spacecraftList = fleet.GetSpacecraftList();
		for (int i = 0; i < spacecraftList.Count; i++)
		{
			Spacecraft sp = spacecraftList[i];
			FleetPanelIcon icon = CreateIcon();
			icon.SetSpacecraft(sp);
		}
	}

	FleetPanelIcon CreateIcon()
	{
		GameObject iconObject = Instantiate(iconPrefab, Vector3.zero, Quaternion.identity);
		iconObject.transform.SetParent(gridLayout);
		FleetPanelIcon icon = iconObject.GetComponent<FleetPanelIcon>();
		iconList.Add(icon);
		return icon;
	}
}
