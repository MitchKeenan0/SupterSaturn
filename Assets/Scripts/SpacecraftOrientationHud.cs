using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacecraftOrientationHud : MonoBehaviour
{
	public Transform bowMarker;
	public Transform crossMarker;
	public float horizonDistance = 1500f;
	public float loadDelay = 0.2f;

	private Spacecraft spacecraft = null;
	private Game game = null;
	private Camera cameraMain;
	private CanvasGroup crossCG;
	private IEnumerator loadCoroutine;
	private IEnumerator looper;
	Vector3 bowHorizonVector = Vector3.zero;

	void Start()
	{
		game = FindObjectOfType<Game>();
		cameraMain = FindObjectOfType<Camera>();
		crossCG = crossMarker.GetComponent<CanvasGroup>();
		crossCG.alpha = 0f;
		loadCoroutine = LoadDelay(loadDelay);
		StartCoroutine(loadCoroutine);
	}

	private IEnumerator LoadDelay(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		if (game == null)
			game = FindObjectOfType<Game>();
		if (game != null)
			spacecraft = game.GetSpacecraftList()[0];
	}

	void Update()
    {
		UpdateBowHorizon();
		CheckHorizonHit();
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
				Vector3 hitScreenPosition = cameraMain.WorldToScreenPoint(hit.point);
				crossMarker.transform.position = hitScreenPosition;
				if (crossCG.alpha != 1f)
					crossCG.alpha = 1f;
				bHit = true;
			}
		}
		if (!bHit && (crossCG.alpha != 0f))
			crossCG.alpha = 0f;
	}
}
