using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
	public GameObject turnPanel;
	private List<FleetController> fleetControllerList;

    void Start()
    {
		InitFleetControllers();
    }

	void InitFleetControllers()
	{
		fleetControllerList = new List<FleetController>();
		FleetController[] fleetControllers = FindObjectsOfType<FleetController>();
		foreach (FleetController fc in fleetControllers)
			fleetControllerList.Add(fc);
	}

	public void EndTurn()
	{
		foreach(FleetController fc in fleetControllerList)
		{
			fc.ExecuteTurn();
		}
	}
}
