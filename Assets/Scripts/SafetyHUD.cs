using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyHUD : MonoBehaviour
{
	public GameObject collisionWarningPanel;
	public float updateInterval = 0.1f;

	private OrbitController orbitController;
	private RaycastManager raycastManager;
	private Camera cameraMain;
	private IEnumerator updateCoroutine;
	private List<Vector3> trajectoryList;

    void Start()
    {
		collisionWarningPanel.SetActive(false);
		trajectoryList = new List<Vector3>();
		orbitController = FindObjectOfType<OrbitController>();
		raycastManager = FindObjectOfType<RaycastManager>();
		cameraMain = Camera.main;
		updateCoroutine = UpdateSafetyHud(updateInterval);
		StartCoroutine(updateCoroutine);
	}

	IEnumerator UpdateSafetyHud(float interval)
	{
		while (true)
		{
			CheckTrajectoryCollision();
			yield return new WaitForSeconds(interval);
		}
	}

	void CheckTrajectoryCollision()
	{
		trajectoryList.Clear();
		foreach (Vector3 vector in orbitController.GetTrajectory())
			trajectoryList.Add(vector);

		bool bHit = false;
		int numVectors = trajectoryList.Count;
		for (int i = 0; i < numVectors; i++)
		{
			if (trajectoryList.Count > (i + 1))
			{
				Vector3 lineStart = trajectoryList[i];
				Vector3 lineEnd = lineStart + (trajectoryList[i + 1] - trajectoryList[i]);
				RaycastHit hit = raycastManager.CustomLinecast(lineStart, lineEnd);
				if ((hit.transform != null))
				{
					if (hit.transform.gameObject.GetComponent<Gravity>())
					{
						Vector3 hitPositionOnscreen = cameraMain.WorldToScreenPoint(hit.point);
						if (hitPositionOnscreen.z > 0f)
						{
							collisionWarningPanel.transform.position = hitPositionOnscreen;
							collisionWarningPanel.SetActive(true);
							CollisionWarningPanel cwp = collisionWarningPanel.GetComponent<CollisionWarningPanel>();
							if (cwp != null)
								cwp.SetDistance(Vector3.Distance(cameraMain.transform.position, hit.point));
							bHit = true;
						}
					}
				}
			}
		}

		if (!bHit)
			collisionWarningPanel.SetActive(false);
	}
}
