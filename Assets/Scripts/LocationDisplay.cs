using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationDisplay : MonoBehaviour
{
	public GameObject displayPanel;
	public Button scoutButton;
	public Button moveToButton;
	public Text nameText;
	public Text valueText;

	// circle rendering
	[Range(0.1f, 100f)]
	public float circleRadius = 3.0f;
	[Range(3, 256)]
	public int numCircleSegments = 55;
	public GameObject circleLineObject;

	private LineRenderer circleLineRenderer;
	private CampaignLocation displayLocation;
	private FleetController playerFleetController;
	private TurnManager turnManager;
	private LocationManager locationManager;
	private Camera cameraMain;
	private float timeAtLastClick = 0f;
	private int lastActionTaken = -1;

	public CampaignLocation GetLocation() { return displayLocation; }

	void Start()
    {
		turnManager = FindObjectOfType<TurnManager>();
		locationManager = FindObjectOfType<LocationManager>();
		displayPanel.SetActive(false);
		circleLineRenderer = circleLineObject.gameObject.GetComponentInChildren<LineRenderer>();
		circleLineRenderer.positionCount = numCircleSegments + 1;
		cameraMain = Camera.main;
	}

	public void SetPlayerFleetController(FleetController fc)
	{
		playerFleetController = fc;
	}

	public void SetDisplayLocation(CampaignLocation location)
	{
		if (location != null)
		{
			displayPanel.SetActive(true);
			displayLocation = location;

			nameText.text = location.locationName;
			valueText.text = location.locationValue.ToString();

			if (locationManager.IsConnectedByRoute(playerFleetController.GetLocation(), displayLocation))
				moveToButton.interactable = true;
			else
				moveToButton.interactable = false;
		}
		else
		{
			displayPanel.SetActive(false);
		}
	}

	public void UpdateCircleRender()
	{
		circleLineObject.transform.position = displayLocation.transform.position;
		circleLineObject.transform.LookAt(cameraMain.transform);
		circleLineObject.transform.position += circleLineObject.transform.forward * 5f;

		float deltaTheta = (float)(2.0 * Mathf.PI) / numCircleSegments;
		float theta = 0f;

		for (int i = 0; i < numCircleSegments + 1; i++)
		{
			float x = circleRadius * Mathf.Cos(theta);
			float z = circleRadius * Mathf.Sin(theta);
			Vector3 pos = new Vector3(x, 0, z);
			circleLineRenderer.SetPosition(i, pos);
			theta += deltaTheta;
		}
	}

	public void Scout()
	{
		if ((displayLocation != null) && (playerFleetController != null))
		{
			if (turnManager.TurnActionTaken())
				Undo();
			playerFleetController.StandbyScout(displayLocation);
			if ((Time.time - timeAtLastClick) < 1f)
				turnManager.EndTurn();
			timeAtLastClick = Time.time;
			turnManager.SetTurnAction();
			lastActionTaken = 0;
		}
	}

	public void MoveTo()
	{
		if ((displayLocation != null) && (playerFleetController != null))
		{
			if (turnManager.TurnActionTaken())
				Undo();
			if (playerFleetController.StandbyMove(displayLocation))
			{
				if ((Time.time - timeAtLastClick) < 1f)
					turnManager.EndTurn();
				timeAtLastClick = Time.time;
				turnManager.SetTurnAction();
				lastActionTaken = 1;
			}
		}
	}

	public void Undo()
	{
		lastActionTaken = -1;
		playerFleetController.Undo();
	}

	bool SameAction(int actionID)
	{
		return actionID == lastActionTaken;
	}

	public void Close()
	{
		SetDisplayLocation(null);
	}

	public void RefreshControlInteractivity()
	{
		if (locationManager.IsConnectedByRoute(playerFleetController.GetLocation(), displayLocation))
			moveToButton.interactable = true;
		else
			moveToButton.interactable = false;
	}
}
