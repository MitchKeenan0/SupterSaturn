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
		if (!line)
			line = transform.parent.GetComponentInChildren<LineRenderer>();
		if (line != null)
		{
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
				Vector3 originPos = line.transform.position;
				Vector3 lineVector = (line.GetPosition(1) - line.GetPosition(0));

				for (int i = 0; i < marks; i++)
				{
					if ((markList != null) && (markList.Count > 0) && (markList[i] != null))
					{
						GameObject markObj = markList[i];
						Vector3 measureIndexVector = originPos + ((lineVector / marks) * i);
						Vector3 screenPosition = cameraMain.WorldToScreenPoint(measureIndexVector) + (cameraMain.transform.forward * 10);
						markObj.transform.position = screenPosition;

						markObj.transform.localScale = Vector3.one * (10f / Vector3.Distance(measureIndexVector, cameraMain.transform.position));

						LineMeasurement lm = markObj.GetComponent<LineMeasurement>();
						if (lm != null)
						{
							float distFromOrigin = ((lineVector / marks) * i).magnitude;
							lm.SetDistance(distFromOrigin);
						}

						if (!markObj.activeInHierarchy)
							markObj.SetActive(true);
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

		bCleared = true;
	}

	GameObject CreateMark()
	{
		GameObject product = Instantiate(measurementPanelPrefab, transform);
		markList.Add(product);
		product.SetActive(false);
		return product;
	}
}
