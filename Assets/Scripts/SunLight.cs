using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class SunLight : MonoBehaviour
{
	private DirectionalLight light;

    void Start()
    {
		light = GetComponent<DirectionalLight>();
    }


}
