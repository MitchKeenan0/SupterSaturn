using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class SunLight : MonoBehaviour
{
	private DirectionalLight directionallight;

    void Start()
    {
		directionallight = GetComponent<DirectionalLight>();
    }


}
