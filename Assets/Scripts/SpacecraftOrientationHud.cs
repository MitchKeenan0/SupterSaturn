using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacecraftOrientationHud : MonoBehaviour
{
	public Transform bowMarker;
	public Transform crossMarker;
	public LineRenderer crossLine;
	public float horizonDistance = 1500f;
	public float loadDelay = 0.2f;

	private Spacecraft spacecraft = null;
	private Game game = null;
	private Camera cameraMain;
	private CanvasGroup crossCG;
	private IEnumerator loadCoroutine;
	private IEnumerator looper;
	private Vector3 bowHorizonVector = Vector3.zero;
	private bool bUpdating = false;

	void Start()
	{
		game = FindObjectOfType<Game>();
		cameraMain = FindObjectOfType<Camera>();
		crossCG = crossMarker.GetComponent<CanvasGroup>();
		crossCG.alpha = 0f;
		bowMarker.gameObject.SetActive(false);
		loadCoroutine = LoadDelay(loadDelay);
		StartCoroutine(loadCoroutine);
	}

	private IEnumerator LoadDelay(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		GetReferences();
	}

	void Update()
    {
		if (bUpdating)
		{
			//UpdateBowHorizon();
			CheckHorizonHit();
		}
		else
		{
			GetReferences();
		}
    }

	void UpdateBowHorizon()
	{
		if (spacecraft != null)
		{
			bowHorizonVector = spacecraft.transform.position + (spacecraft.transform.forward * horizonDistance);
			Vector3 toHorizon = (bowHorizonVector - cameraMain.transform.position);
			float dotToHorizon = Vector3.Dot(cameraMain.transform.forward, toHorizon);
			if (dotToHorizon > 0.1f)
			{
				Vector3 clampedHorizon = cameraMain.transform.position + Vector3.ClampMagnitude(toHorizon, 900f);
				Vector3 bowScreenPosition = cameraMain.WorldToScreenPoint(clampedHorizon);
				bowMarker.position = bowScreenPosition;
			}
		}
		else if (game != null)
		{
			spacecraft = game.GetSpacecraftList()[0];
		}
	}

	void CheckHorizonHit()
	{
		bool bHit = false;
		RaycastHit[] hits = Physics.RaycastAll(spacecraft.transform.position, bowHorizonVector);
		int numHits = hits.Length;
		for(int i = 0; i < numHits; i++)
		{
			RaycastHit hit = hits[i];
			if ((hit.transform != null) && (hit.transform != spacecraft.transform))
			{
				DrawCross(hit.point);
				bHit = true;
			}
		}
		if (!bHit && (crossCG.alpha != 0f))
			DisableCross();
	}

	public void DrawCross(Vector3 crossPoint)
	{
		Vector3 hitScreenPosition = cameraMain.WorldToScreenPoint(crossPoint);
		crossMarker.transform.position = hitScreenPosition;
		if (crossCG.alpha != 1f)
			crossCG.alpha = 1f;
		crossLine.enabled = true;
		crossLine.SetPosition(0, spacecraft.transform.position);
		crossLine.SetPosition(1, crossPoint);
	}

	void DisableCross()
	{
		crossCG.alpha = 0f;
		crossLine.enabled = false;
	}

	void GetReferences()
	{
		if (game == null)
			game = FindObjectOfType<Game>();
		if (game != null)
		{
			if ((game.GetSpacecraftList() != null) && (game.GetSpacecraftList().Count > 0))
				spacecraft = game.GetSpacecraftList()[0];
		}
		if (spacecraft != null)
			bUpdating = true;
	}
}
