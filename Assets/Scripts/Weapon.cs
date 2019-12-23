using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	public float weaponSpeed = 1000f;
	public Transform munitionPrefab;
	public ParticleSystem armingParticles;
	public float prefireTime = 0.1f;
	public float fireRate = 1f;
	public float rateDeviation = 0.5f;

	private Transform targetTransform;
	private bool bCanFire = true;
	private IEnumerator prefireCoroutine;
	private IEnumerator recoveryCoroutine;

	public bool CanFire() { return bCanFire; }

	public void SetTarget(Transform value) { targetTransform = value; }

	public void FireAt(Transform target)
	{
		bCanFire = false;
		SetTarget(target);
		prefireCoroutine = Prefire(prefireTime);
		StartCoroutine(prefireCoroutine);
	}

	void Fire()
	{
		Transform munition = Instantiate(munitionPrefab, transform.position, Quaternion.identity);
		//munition.LookAt(targetTransform);

		Projectile projectile = munition.GetComponent<Projectile>();
		if (projectile != null)
		{
			float distToTarget = Vector3.Distance(targetTransform.position, transform.position + targetTransform.GetComponent<Rigidbody>().velocity);
			float random = Random.Range(55f, 60f);
			float predictionScalar = (distToTarget / weaponSpeed) * random;
			Vector3 targetAtVelocity = targetTransform.position + (targetTransform.GetComponent<Rigidbody>().velocity * predictionScalar);
			munition.transform.rotation = Quaternion.LookRotation((targetAtVelocity - transform.position), Vector3.up);

			Rigidbody munitionRb = munition.GetComponent<Rigidbody>();
			munitionRb.AddForce(munition.forward * weaponSpeed);

			projectile.ArmProjectile(transform);
		}

		recoveryCoroutine = Recover(fireRate);
		StartCoroutine(recoveryCoroutine);
	}

	void Recover()
	{
		bCanFire = true;
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
