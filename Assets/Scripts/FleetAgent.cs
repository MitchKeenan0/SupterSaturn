using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetAgent : MonoBehaviour
{
	private Fleet fleet;
	private FleetController fleetController;
	private LocationManager locationManager;

    void Start()
    {
		fleet = GetComponent<Fleet>();
		fleetController = GetComponent<FleetController>();
    }
}
