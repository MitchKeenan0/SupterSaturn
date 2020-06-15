using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocusHud : MonoBehaviour
{
	public Transform targetBoxTransform;
	public Transform animationTransform;

	private CanvasGroup targetBoxCanvasGroup;
	private CanvasGroup animationCanvasGroup;
	private bool bUpdating = false;
	private bool bAnimatingTarget = false;
	private Transform locusTransform;
	private TouchOrbit touchOrbit;
	private Camera cameraMain;
	private GameObject[] suspects;

    void Start()
    {
		targetBoxCanvasGroup = targetBoxTransform.GetComponent<CanvasGroup>();
		animationCanvasGroup = animationTransform.GetComponent<CanvasGroup>();
		SetTargetBoxEnabled(false);
		touchOrbit = FindObjectOfType<TouchOrbit>();
		cameraMain = Camera.main;
		suspects = GameObject.FindGameObjectsWithTag("ElementObject");
	}

    void Update()
    {
        if (bUpdating)
			CheckForLocus();

		if (bAnimatingTarget)
			AnimateTarget();
    }

	void CheckForLocus()
	{
		List<GameObject> potentialTargetList = new List<GameObject>();
		Transform previousLocusTransform = locusTransform;

		/// round up the perps
		foreach(GameObject go in suspects)
		{
			Transform oeTransform = go.transform;
			Vector3 toElement = (oeTransform.position - cameraMain.transform.position).normalized;
			Vector3 camForward = cameraMain.transform.forward.normalized;
			float dotToElement = Vector3.Dot(camForward, toElement);
			if (dotToElement > 0f)
			{
				Vector3 elementScreenPosition = cameraMain.WorldToScreenPoint(oeTransform.position);
				elementScreenPosition.z = 0f;
				float top = Screen.height * 0.7f;
				float bottom = Screen.height - top;
				float right = Screen.width * 0.7f;
				float left = Screen.width - right;
				bool bTargetBoxed = ((elementScreenPosition.x >= left) && (elementScreenPosition.x <= right))
					&& ((elementScreenPosition.y >= bottom) && (elementScreenPosition.y <= top));
				if (bTargetBoxed && !potentialTargetList.Contains(go))
				{
					potentialTargetList.Add(go);
				}
			}
		}

		/// delineate the favorite
		float closestDistance = 9999999f;
		GameObject primeSuspect = null;
		foreach(GameObject oePerp in potentialTargetList)
		{
			Vector3 objectScreenPosition = cameraMain.WorldToScreenPoint(oePerp.transform.position);
			Vector3 screenCentre = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
			float screenPosToCentre = Vector3.Distance(screenCentre, objectScreenPosition);
			if (screenPosToCentre < closestDistance)
			{
				closestDistance = screenPosToCentre;
				primeSuspect = oePerp;
			}
		}

		/// set new locus
		if (primeSuspect != null)
		{
			Transform locus = primeSuspect.transform;
			if ((locusTransform != locus)
				&& (locus != previousLocusTransform))
			{
				touchOrbit.SetFocusTransform(locus);
				locusTransform = locus;
				ResetAnimation();
				bAnimatingTarget = true;
				animationCanvasGroup.alpha = 1f;
				animationTransform.localScale = Vector3.one * 10f;
			}
		}
		else if (closestDistance > (Screen.width / 1.5f))
		{
			if (locusTransform != null)
				previousLocusTransform = locusTransform;
			locusTransform = null;
			touchOrbit.SetFocusTransform(null);
			SetTargetBoxEnabled(false);
			animationCanvasGroup.alpha = 0f;
		}
	}

	void ResetAnimation()
	{
		animationTransform.localScale = Vector3.one;
	}

	void AnimateTarget()
	{
		if (animationTransform.localScale.magnitude > 0.001f)
		{
			if (locusTransform != null)
			{
				float animationSpeed = 20f;
				animationTransform.localScale = Vector3.MoveTowards(animationTransform.localScale, Vector3.zero, Time.deltaTime * animationSpeed);
				Vector3 targetScreenPosition = cameraMain.WorldToScreenPoint(locusTransform.position);
				targetScreenPosition.z = 0f;
				animationTransform.position = targetScreenPosition;
			}
		}
		else
		{
			animationCanvasGroup.alpha = 0f;
			animationTransform.localScale = Vector3.one;
			bAnimatingTarget = false;
		}
	}

	public void SetTargetBoxEnabled(bool value)
	{
		bUpdating = value;
		targetBoxCanvasGroup.alpha = value ? 1f : 0f;
	}
}
