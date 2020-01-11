using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campaign : MonoBehaviour
{
	public List<Spacecraft> fleetList;

    void Start()
    {
		fleetList = new List<Spacecraft>();
    }


}
