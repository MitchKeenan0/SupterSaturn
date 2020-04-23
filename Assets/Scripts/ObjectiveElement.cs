using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveElement : MonoBehaviour
{
	public GameObject elementPrefab;

	public GameObject GetElement() { return elementObject; }
	private GameObject elementObject;

    void Start()
    {
		
    }

	public virtual void Init()
	{
		GameObject spawnElement = Instantiate(elementPrefab);
		elementObject = spawnElement;
		elementObject.SetActive(false);
	}

	public virtual void UpdateElement()
	{

	}

	public virtual void SetElementEnabled(bool value)
	{

	}
}
