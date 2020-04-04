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

	public virtual void InitScan(Transform owningScanner, Quaternion rotation, float lifeTime)
	{
		scannerTransform = owningScanner;
		transform.rotation = rotation;
		scanLifetime = lifeTime;
	}

	public virtual void SetEnabled(bool value)
	{
		if (GetComponent<Renderer>() != null)
			GetComponent<Renderer>().enabled = value;
	}
}
