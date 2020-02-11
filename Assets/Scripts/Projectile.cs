using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public int damage = 1;
	public float hitboxScale = 1;
	public float impactForce = 1f;
	public Transform impactPrefab;
	public float maxLifetime = 5f;

	private Rigidbody rb;
	private Weapon owner;
	private RaycastManager raycastManager;
	private TrailRenderer trail;
	private Vector3 lastPosition;
	private float timeAtFired = 0f;
	private float percentOfLifetime = 0f;
	private float originalTrailWidth = 0f;
	private bool bHit = false;
	private IEnumerator lifeTimeCoroutine;
	private IEnumerator fadeAwayCoroutine;

	void Start()
    {
		rb = GetComponent<Rigidbody>();
		trail = GetComponentInChildren<TrailRenderer>();
		raycastManager = FindObjectOfType<RaycastManager>();
		
		lifeTimeCoroutine = LifeTime(maxLifetime);
		StartCoroutine(lifeTimeCoroutine);
		lastPosition = rb.position;

		originalTrailWidth = trail.widthMultiplier;
		fadeAwayCoroutine = FadeAway(0.1f);
		StartCoroutine(fadeAwayCoroutine);
	}

	public void ArmProjectile(Weapon ownerWeapon)
	{
		owner = ownerWeapon;
		timeAtFired = Time.time;
		if (!raycastManager)
			raycastManager = FindObjectOfType<RaycastManager>();
		raycastManager.AddProjectile(this);
	}

	public void Hit(RaycastHit hit)
	{
		if (!bHit && !hit.collider.isTrigger && (hit.transform != owner) && (impactPrefab != null))
		{
			bHit = true;
			Vector3 impactVelocity = rb.velocity * impactForce;
			rb.isKinematic = true;

			Transform impact = Instantiate(impactPrefab, hit.point, Quaternion.identity);
			Destroy(impact.gameObject, 5f);

			Health hp = hit.transform.GetComponent<Health>();
			if (hp != null)
			{
				hp.ModifyHealth(-damage, owner.transform);
			}

			Rigidbody r = hit.transform.GetComponent<Rigidbody>();
			if (r != null)
			{
				r.AddForceAtPosition(impactVelocity / (rb.mass * 100f), hit.point);
			}
		}
	}

	public Vector3 GetVelocity()
	{
		if (rb.velocity.magnitude > 2f)
			return rb.velocity;
		else
			return transform.forward * 2f;
	}

	public Weapon GetOwner()
	{
		return owner;
	}

	private IEnumerator LifeTime(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		if (!raycastManager)
			raycastManager = FindObjectOfType<RaycastManager>();
		raycastManager.RemoveProjectile(this);
		Destroy(gameObject, 0.1f);
	}

	private IEnumerator FadeAway(float waitTime)
	{
		while (percentOfLifetime < 1f)
		{
			yield return new WaitForSeconds(waitTime);
			percentOfLifetime = (Time.time - timeAtFired) / maxLifetime;
			trail.widthMultiplier = originalTrailWidth * (1f - percentOfLifetime);
		}
	}
}
