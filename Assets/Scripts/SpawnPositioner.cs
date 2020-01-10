using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPositioner : MonoBehaviour
{
	public bool bRandomPosition = true;

    void Awake()
    {
		float distance = transform.position.magnitude;
		transform.position = Random.onUnitSphere * distance;
		transform.rotation = Quaternion.Euler(transform.position);
    }
}
