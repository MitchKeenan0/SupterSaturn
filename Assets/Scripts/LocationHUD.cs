using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationHUD : MonoBehaviour
{
	public GameObject locationPanel;
	public Text locationNameText;
	public Text locationValueText;

	private LocationInteraction highlightInteraction;
	private CampaignLocation highlightedLocation;
	private Camera cameraMain;
	private bool bUpdating = false;

    void Start()
    {
		locationPanel.SetActive(false);
		cameraMain = Camera.main;
    }

	void Update()
	{
		if (bUpdating)
			UpdateLocationPanel();
	}

	public void Highlight(LocationInteraction interaction)
	{
		if (!locationPanel.activeInHierarchy)
			locationPanel.SetActive(true);

		highlightInteraction = interaction;
		highlightedLocation = highlightInteraction.GetLocation();

		if (highlightedLocation != null)
			bUpdating = true;
	}

	public void Deselect()
	{
		bUpdating = false;
		locationPanel.SetActive(false);
	}

	void UpdateLocationPanel()
	{
		string nameString = highlightedLocation.locationName.ToString();
		locationNameText.text = nameString;
		string valueString = highlightedLocation.locationValue.ToString();
		locationValueText.text = valueString;

		Vector3 locationScreenPosition = cameraMain.WorldToScreenPoint(highlightedLocation.transform.position);
		locationPanel.transform.position = locationScreenPosition;
	}
}
