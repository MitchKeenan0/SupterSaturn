using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour
{
	public string objectiveName = "";
	public int objectiveRating = 1;
	public bool bRandomStartPosition = true;
	public float startPositionRangeMax = 200f;
	public float startPositionRangeMin = 50f;

	private Rigidbody rb;
	private BattleOutcome outcome;
	private bool bTriggered = false;
	private bool bDestroyed = false;

	public bool IsTriggered() { return bTriggered; }
	public bool IsDestroyed() { return bDestroyed; }

    void Start()
    {
		rb = GetComponent<Rigidbody>();
		outcome = FindObjectOfType<BattleOutcome>();

		if (bRandomStartPosition)
		{
			float randomRange = Random.Range(startPositionRangeMin, startPositionRangeMax);
			Vector3 randomPos = Random.onUnitSphere * randomRange;
			randomPos.z = Mathf.Clamp(randomPos.z, (startPositionRangeMax / 2), startPositionRangeMax);
			transform.position = randomPos;
		}
    }

	public void Trigger()
	{
		bTriggered = true;
		ArrestMovement();
	}

	public void ArrestMovement()
	{
		if (rb != null)
		{
			rb.drag = 1.6f;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.GetComponentInChildren<Planet>())
		{
			bDestroyed = true;
			outcome.AddLost(objectiveRating * 10);
			outcome.BattleOver(false);
		}
	}
}
