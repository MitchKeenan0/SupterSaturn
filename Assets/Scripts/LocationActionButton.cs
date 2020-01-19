using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LocationActionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private LocationDisplay locationDisplay;
	private FleetHUD fleetHud;
	private TurnManager turnManager;

    void Start()
	{
		locationDisplay = FindObjectOfType<LocationDisplay>();
		fleetHud = FindObjectOfType<FleetHUD>();
		turnManager = FindObjectOfType<TurnManager>();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		fleetHud.MouseOnUI(true);
		if (CompareTag("EndTurn"))
			turnManager.EnableWarningPanel(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		fleetHud.MouseOnUI(false);
		if (CompareTag("EndTurn"))
			turnManager.EnableWarningPanel(false);
	}
}
