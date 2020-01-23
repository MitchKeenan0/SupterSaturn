using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetAgent : MonoBehaviour
{
	private Fleet fleet;
	private FleetController playerFleetController;
	private LocationManager locationManager;
	private NameLibrary nameLibrary;

    void Awake()
    {
		fleet = GetComponentInParent<Fleet>();
		locationManager = FindObjectOfType<LocationManager>();
		playerFleetController = GetComponent<FleetController>();
		nameLibrary = FindObjectOfType<NameLibrary>();
		InitAgent();
    }

	void InitAgent()
	{
		fleet.fleetName = nameLibrary.GetFleetName();
	}
}
