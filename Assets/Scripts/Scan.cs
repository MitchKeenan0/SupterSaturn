using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scan : MonoBehaviour
{
	public float scanSpeed = 10f;
	public float scanLifetime = 2f;

	private Transform scannerTransform;
	
    void Start()
    {
        
    }

	public virtual void SetOwningScanner(Transform owningScanner)
	{
		scannerTransform = owningScanner;
	}

	public void SetEnabled(bool value)
	{
		GetComponent<Renderer>().enabled = value;
	}
}
