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

	public virtual void Init(Vector3 offset, Quaternion rotation)
	{
		GameObject spawnElement = Instantiate(elementPrefab, offset, Quaternion.identity);
		spawnElement.transform.rotation = rotation;
		elementObject = spawnElement;
		elementObject.transform.rotation = rotation;
		elementObject.SetActive(false);
	}

	public virtual void UpdateElement()
	{

	}

	public virtual void SetElementEnabled(bool value)
	{

	}
}
