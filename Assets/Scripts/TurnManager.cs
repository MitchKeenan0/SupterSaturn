﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
	public float turnPassWaitTime = 1f;
	public GameObject endTurnPanel;
	public GameObject warningPanel;
	public GameObject undoPanel;

	private List<FleetController> fleetControllerList;
	private FleetController playerFleetController;
	private LocationDisplay locationDisplay;
	private bool bTurnAction = false;

	private IEnumerator turnPassCoroutine;

	public bool TurnActionTaken() { return bTurnAction; }

    void Start()
    {
		locationDisplay = FindObjectOfType<LocationDisplay>();
		InitFleetControllers();
		BeginTurn(false);
    }

	void InitFleetControllers()
	{
		fleetControllerList = new List<FleetController>();
		FleetController[] fleetControllers = FindObjectsOfType<FleetController>();
		foreach (FleetController fc in fleetControllers)
		{
			fleetControllerList.Add(fc);
			if (fc.GetTeamID() == 0)
				playerFleetController = fc;
		}
	}

	public void BeginTurn(bool turnActionExplicit)
	{
		bTurnAction = turnActionExplicit;
		endTurnPanel.SetActive(true);
		undoPanel.SetActive(false);
		warningPanel.SetActive(false);
	}

	public void SetTurnAction()
	{
		bTurnAction = true;
		warningPanel.SetActive(false);
		undoPanel.SetActive(true);
	}

	public void EnableWarningPanel(bool value)
	{
		if (!bTurnAction && value)
			warningPanel.SetActive(true);
		else
			warningPanel.SetActive(false);
	}

	public void EndTurn()
	{
		foreach(FleetController fc in fleetControllerList)
		{
			fc.ExecuteTurn();
		}
		warningPanel.SetActive(false);
		endTurnPanel.SetActive(false);
		undoPanel.SetActive(false);

		if (!bTurnAction)
		{
			turnPassCoroutine = PassTurn(turnPassWaitTime);
			StartCoroutine(turnPassCoroutine);
		}
	}

	public void Undo()
	{
		playerFleetController.Undo();
		undoPanel.SetActive(false);
		bTurnAction = false;
		locationDisplay.Undo();
	}

	private IEnumerator PassTurn(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		BeginTurn(false);
	}
}