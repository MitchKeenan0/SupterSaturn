using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialHUD : MonoBehaviour
{
	public GameObject panelPrefab;

	private Camera cameraMain;
	private List<CelestialBody> celestialBodyList;
	private List<RectTransform> panelList;
	private bool bUpdating = false;

    void Awake()
    {
		celestialBodyList = new List<CelestialBody>();
		panelList = new List<RectTransform>();
		cameraMain = Camera.main;
	}

    public void InitCelestialHud()
	{
		if (celestialBodyList != null)
		{
			CelestialBody[] celestials = FindObjectsOfType<CelestialBody>();
			int num = celestials.Length;
			for (int i = 0; i < num; i++)
			{
				if (celestials[i] != null)
				{
					celestialBodyList.Add(celestials[i]);
					SpawnPanel(celestials[i]);
				}
			}
			bUpdating = true;
		}
	}

	void Update()
	{
		if (bUpdating)
		{
			int cels = celestialBodyList.Count;
			for(int i = 0; i < cels; i++)
			{
				if ((i < panelList.Count) && (panelList[i] != null))
				{
					RectTransform rt = panelList[i];
					Vector3 celestialBodyPosition = celestialBodyList[i].GetPlanetPosition();
					if (Vector3.Distance(cameraMain.transform.position, celestialBodyPosition) >= 900)
					{
						Vector3 toBody = celestialBodyPosition - cameraMain.transform.position;
						celestialBodyPosition = cameraMain.transform.position + Vector3.ClampMagnitude(toBody, 900f);
					}
					Vector3 celestialScreenPosition = cameraMain.WorldToScreenPoint(celestialBodyPosition);
					if (celestialScreenPosition.z > 0)
					{
						celestialScreenPosition.z = 0f;
						rt.transform.position = celestialScreenPosition;
					}
				}
			}

			// touch celestials
			if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
			{
				Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
				RaycastHit[] raycastHits = Physics.RaycastAll(raycast);
				if (raycastHits.Length > 0)
				{
					for(int i = 0; i < raycastHits.Length; i++)
					{
						Debug.Log("Something Hit");
						RaycastHit hit = raycastHits[i];
						if (hit.collider.transform.root.gameObject.GetComponent<CelestialBody>())
						{
							Debug.Log("celestial clicked");
							FindObjectOfType<TouchOrbit>().SetFocusTransform(hit.collider.transform);
							break;
						}
					}
				}
			}
		}
	}

	void SpawnPanel(CelestialBody cb)
	{
		GameObject panel = Instantiate(panelPrefab, transform);
		CelestialBodyPanel cbp = panel.GetComponent<CelestialBodyPanel>();
		cbp.SetPanel(null, cb.celestialBodyName, cb.transform);
		RectTransform rt = panel.GetComponent<RectTransform>();
		if (rt != null)
			panelList.Add(rt);
	}
}
