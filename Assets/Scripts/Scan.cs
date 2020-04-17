using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scan : MonoBehaviour
{
	public float scanSpeed = 10f;
	public float scanLifetime = 2f;

	private AbilityScanner scanner;
	public AbilityScanner GetScanner() { return scanner; }
	
    void Start()
    {
        
    }

	public virtual void InitScan(AbilityScanner owningScanner, Quaternion rotation, float lifeTime)
	{
		scanner = owningScanner;
		transform.rotation = rotation;
		scanLifetime = lifeTime;
		transform.parent = owningScanner.transform;
	}

	public virtual void SetEnabled(bool value)
	{
		if (GetComponent<Renderer>() != null)
			GetComponent<Renderer>().enabled = value;
	}
}
