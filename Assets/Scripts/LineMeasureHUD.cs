using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMeasureHUD : MonoBehaviour
{
	public GameObject measurementPanelPrefab;
	public int marks = 10;

	private LineRenderer line;
	private Camera cameraMain;
	private List<GameObject> markList;
	private IEnumerator loadWaitCoroutine;
	private Transform originalParent = null;
	private bool bLoaded = false;
	private bool bCleared = true;

    void Start()
    {
		markList = new List<GameObject>();
		cameraMain = Camera.main;
		loadWaitCoroutine = LoadWait(0.2f);
		StartCoroutine(loadWaitCoroutine);
    }

	void Update()
	{
		if (bLoaded)
			UpdateLine();
	}

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		AquireLine();
		bLoaded = true;
	}

	void AquireLine()
	{
		line = GetComponent<LineRenderer>();
		if (!line && (transform.parent != null))
			line = transform.parent.GetComponentInChildren<LineRenderer>();
		if (line != null)
		{
			if (line.transform.parent != null)
				originalParent = line.transform.parent;
			line.enabled = false;
			if (markList.Count < marks)
			{
				int num = marks - markList.Count;
				for (int i = 0; i < num; i++)
					CreateMark();
			}
		}
	}

	public void UpdateLine()
	{
		if (line == null)
			AquireLine();

		if ((line != null) && (line.positionCount > 1))
		{
			if (line.gameObject.activeInHierarchy && line.enabled)
			{
				if (line.transform.parent != null)
					line.transform.SetParent(null);

				Vector3 originPos = line.transform.position;
				Vector3 lineVector = (line.GetPosition(1) - line.GetPosition(0));

				for (int i = 0; i < marks; i++)
				{
					if ((markList != null) && (markList.Count > 0) && (markList[i] != null))
					{
						GameObject markObj = markList[i];
						Vector3 markPosition = markList[i].transform.position;
						Vector3 markVector = ((lineVector / marks) * (i + 1));
						if (transform.parent != null)
							markPosition = originPos + markVector;

						LineMeasurement lm = markObj.GetComponent<LineMeasurement>();
						if (lm != null)
						{
							float distFromOrigin = markVector.magnitude;
							lm.SetDistance(distFromOrigin);

							Vector3 markToCameraNormal = (markPosition - cameraMain.transform.position).normalized;
							if (Vector3.Dot(cameraMain.transform.forward, markToCameraNormal) > 0.1f)
							{
								Vector3 screenPosition = cameraMain.WorldToScreenPoint(markPosition) + (cameraMain.transform.forward * 10);
								markObj.transform.position = screenPosition;

								float scaleFactor = (0.1f / Vector3.Distance(markPosition, cameraMain.transform.position));
								scaleFactor = Mathf.Clamp(scaleFactor, 1f, 0.001f);
								markObj.transform.localScale = Vector3.one * scaleFactor;

								if (!markObj.activeInHierarchy)
									markObj.SetActive(true);
							}
							else if (markObj.activeInHierarchy)
							{
								markObj.SetActive(false);
							}
						}
					}
				}

				bCleared = false;
			}
			else if (!bCleared)
			{
				ClearMarks();
			}
		}
	}

	public void SetPosition(Vector3 value)
	{
		if (!line)
			AquireLine();
		line.transform.position = value;
	}

	public void ClearMarks()
	{
		int numMarks = markList.Count;
		if (numMarks > 0)
		{
			for(int i = 0; i < numMarks; i++)
			{
				GameObject go = markList[i];
				go.SetActive(false);
			}
		}

		if (line.transform.parent == null)
		{
			line.transform.position = Vector3.zero;
			line.transform.SetParent(originalParent);
			line.transform.localPosition = Vector3.zero;
		}

		bCleared = true;
	}

	GameObject CreateMark()
	{
		if (!line)
			AquireLine();
		
		GameObject product = Instantiate(measurementPanelPrefab, transform);
		markList.Add(product);
		product.SetActive(false);
		return product;
	}
}
