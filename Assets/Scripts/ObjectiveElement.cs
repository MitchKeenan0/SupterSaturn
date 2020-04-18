using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveElement : MonoBehaviour
{
	public GameObject elementPrefab;

	private GameObject element;

    void Start()
    {
        
    }

	public virtual void Init()
	{
		GameObject spawnElement = Instantiate(elementPrefab, transform);
		element = spawnElement;
	}

	public virtual void UpdateElement()
	{

	}

	public virtual void SetElementEnabled(bool value)
	{

	}
}
