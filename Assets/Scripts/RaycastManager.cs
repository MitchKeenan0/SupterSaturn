using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManager : MonoBehaviour
{
	private List<Projectile> projectileList;

    void Start()
    {
		projectileList = new List<Projectile>();
    }
	
    void Update()
    {
		ProjectileRaycasts();
    }

	void ProjectileRaycasts()
	{
		int numPrj = projectileList.Count;
		if (numPrj > 0)
		{
			for (int i = 0; i < numPrj; i++)
			{
				if (projectileList[i] != null)
				{
					Projectile pj = projectileList[i];
					Vector3 rayVector = pj.GetVelocity() * Time.fixedDeltaTime;
					RaycastHit[] hits = Physics.RaycastAll(pj.transform.position, rayVector, rayVector.magnitude);
					if (hits.Length > 0)
					{
						foreach (RaycastHit hit in hits)
						{
							if (hit.transform != pj.GetOwner())
							{
								pj.Hit(hit);
							}
						}
					}
				}
			}
		}
	}

	public RaycastHit CustomRaycast(Vector3 origin, Vector3 direction)
	{
		RaycastHit firstHit = new RaycastHit();
		RaycastHit[] hits = Physics.RaycastAll(origin, direction);
		if (hits.Length > 0)
		{
			float closestDistance = direction.magnitude;
			foreach (RaycastHit h in hits)
			{
				float hDist = Vector3.Distance(origin, h.point);
				if (hDist < closestDistance)
				{
					closestDistance = hDist;
					firstHit = h;
				}
			}
		}
		return firstHit;
	}

	public RaycastHit CustomLinecast(Vector3 origin, Vector3 end)
	{
		RaycastHit hit;
		Physics.Linecast(origin, end, out hit);
		return hit;
	}

	public void AddProjectile(Projectile value)
	{
		projectileList.Add(value);
	}

	public void RemoveProjectile(Projectile value)
	{
		projectileList.Remove(value);
	}
}
