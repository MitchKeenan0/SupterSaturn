using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	public Transform renderTransform;

	private GameObject playerObject = null;
	private CelestialBody celestialBody;
	private CheckpointHud checkpointHud = null;
	private bool bHit = false;

    void Start()
    {
		playerObject = GameObject.FindGameObjectWithTag("Player");
		celestialBody = transform.root.GetComponent<CelestialBody>();
		checkpointHud = FindObjectOfType<CheckpointHud>();
	}

	void ClearCheckpoint()
	{
		if (!bHit)
		{
			checkpointHud.CheckpointClear(celestialBody);
			renderTransform.gameObject.SetActive(false);
			bHit = true;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log(other.gameObject.name);
		if (!playerObject)
			playerObject = GameObject.FindGameObjectWithTag("Player");
		if (other.gameObject == playerObject)
		{
			ClearCheckpoint();
		}
	}
}
