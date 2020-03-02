using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
	public bool bEscape = false;
	public Vector3 deltaTouch = Vector3.zero;
	Touch touch;
    
    void Update()
    {
        UpdateTouch();
    }

	void UpdateTouch()
	{
		if (Input.touchCount > 0)
		{
			touch = Input.GetTouch(0);
		}
	}
}
