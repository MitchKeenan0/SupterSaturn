using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	public float weaponSpeed = 1000f;
	public float weaponAccuracy = 0.95f;
	public Transform munitionPrefab;
	public ParticleSystem armingParticles;
	public float prefireTime = 0.1f;
	public float fireRate = 1f;
	public float rateDeviation = 0.5f;

	private Spacecraft spacecraft;
	private BattleOutcome battleOutcome;
	private Transform targetTransform;
	private bool bCanFire = true;
	private bool bHit = false;
	private IEnumerator prefireCoroutine;
	private IEnumerator recoveryCoroutine;

	public bool CanFire() { return bCanFire; }

	public void SetTarget(Transform value) { targetTransform = value; }

	void Start()
	{
		spacecraft = GetComponent<Spacecraft>();
		battleOutcome = FindObjectOfType<BattleOutcome>();
	}

	void Fire()
	{
		bHit = false;
		Transform munition = Instantiate(munitionPrefab, transform.position, Quaternion.identity);

		Projectile projectile = munition.GetComponent<Projectile>();
		if (projectile != null)
		{
			Vector3 targetPosition = targetTransform.position;
			Vector3 targetVelocity = Vector3.zero;
			if (targetTransform.gameObject.GetComponent<Rigidbody>())
				targetVelocity = targetTransform.GetComponent<Rigidbody>().velocity;

			float distToTarget = Vector3.Distance(targetPosition, transform.position + targetVelocity);
			float random = Random.Range(55f, 60f);
			float predictionScalar = (distToTarget / weaponSpeed) * random;
			Vector3 targetAtVelocity = targetPosition + (targetVelocity * predictionScalar);
			Vector3 accuracyVector = Random.onUnitSphere * (0.1f / weaponAccuracy) * distToTarget * 0.05f;
			Vector3 fireVector = (targetAtVelocity + accuracyVector) - transform.position;

			munition.transform.rotation = Quaternion.LookRotation(fireVector, Vector3.up);
			Rigidbody munitionRb = munition.GetComponent<Rigidbody>();
			munitionRb.AddForce(munition.forward * weaponSpeed);
			projectile.ArmProjectile(this);
		}

		recoveryCoroutine = Recover(fireRate);
		StartCoroutine(recoveryCoroutine);
	}

	void Recover()
	{
		bCanFire = true;
	}

	public void FireAt(Transform target)
	{
		bCanFire = false;
		SetTarget(target);
		prefireCoroutine = Prefire(prefireTime);
		StartCoroutine(prefireCoroutine);
	}

	public void ReturnHit(float damage, Vector3 hitLocation)
	{
		if (!bHit && (spacecraft.GetAgent().teamID == 0))
		{
			int distanceToHit = Mathf.Clamp(Mathf.FloorToInt(Vector3.Distance(hitLocation, transform.position)), 1, 10);
			int hitScore = Mathf.FloorToInt(damage * distanceToHit);
			battleOutcome.AddScore(hitScore);
		}

		bHit = true;
	}

	private IEnumerator Prefire(float waitTime)
	{
		if (targetTransform != null)
		{
			armingParticles.transform.rotation = Quaternion.Euler(targetTransform.position - transform.position);
		}

		var em = armingParticles.emission;
		em.enabled = true;

		yield return new WaitForSeconds(waitTime);

		if (targetTransform != null)
		{
			Fire();
			em.enabled = false;
		}
	}

	private IEnumerator Recover(float waitTime)
	{
		float fireRateDeviationDelay = Random.Range(0f, rateDeviation);
		yield return new WaitForSeconds(waitTime + fireRateDeviationDelay);
		Recover();
	}
}
