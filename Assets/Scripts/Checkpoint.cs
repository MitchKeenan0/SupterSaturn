using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	public Transform renderTransform;
	public Color checkpointColor;

	private GameObject playerObject = null;
	private CelestialBody celestialBody;
	private CheckpointHud checkpointHud = null;
	private CircleRenderer circleRenderer = null;
	private ParticleSystem clearParticles;
	private bool bHit = false;

    void Start()
    {
		playerObject = GameObject.FindGameObjectWithTag("Player");
		celestialBody = transform.root.GetComponent<CelestialBody>();
		circleRenderer = transform.root.GetComponentInChildren<CircleRenderer>();
		clearParticles = GetComponentInChildren<ParticleSystem>();
		checkpointHud = FindObjectOfType<CheckpointHud>();
	}

	void ClearCheckpoint()
	{
		if (!bHit)
		{
			checkpointHud.CheckpointClear(celestialBody);
			renderTransform.gameObject.SetActive(false);
			bHit = true;
			SetColor(Color.gray);
			clearParticles.Play();
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

	public void SetColor(Color value)
	{
		circleRenderer.SetColor(value);
	}
}
