using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityTargetPanel : MonoBehaviour
{
	public GameObject targetObject = null;

    void Start()
    {
        
    }

	public void SetTargetObject(GameObject value)
	{
		targetObject = value;
	}
}
