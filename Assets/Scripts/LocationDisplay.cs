using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationDisplay : MonoBehaviour
{
	public GameObject displayPanel;
	public Text nameText;
	public Text valueText;

	private CampaignLocation displayLocation;
	private FleetController playerFleetController;
	private TurnManager turnManager;
	private float timeAtLastClick = 0f;
	private int lastActionTaken = -1;

	void Start()
    {
		turnManager = FindObjectOfType<TurnManager>();
		displayPanel.SetActive(false);
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
		}
		else
		{
			displayPanel.SetActive(false);
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
			playerFleetController.StandbyMove(displayLocation);
			if ((Time.time - timeAtLastClick) < 1f)
				turnManager.EndTurn();
			timeAtLastClick = Time.time;
			turnManager.SetTurnAction();
			lastActionTaken = 1;
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
}
