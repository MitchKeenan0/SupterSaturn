using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FleetPanel : MonoBehaviour
{
	public Transform gridLayout;
	public GameObject iconPrefab;
	public Sprite unknownSprite;

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

		Identity identity = fleet.gameObject.GetComponentInChildren<Identity>();
		if (identity != null)
		{
			fleetNameText.text = identity.identityName;
			fleetNameText.color = identity.identityColor;
		}
		else
		{
			fleetNameText.text = fleet.fleetName;
		}
		
		spacecraftList = fleet.GetSpacecraftList();
		for (int i = 0; i < spacecraftList.Count; i++)
		{
			Spacecraft sp = spacecraftList[i];
			FleetPanelIcon icon = CreateIcon();
			if (fleet.teamID == 0)
				icon.SetSpacecraft(sp);
			else
				icon.SetImage(unknownSprite);
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
