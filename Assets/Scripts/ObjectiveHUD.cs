using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveHUD : MonoBehaviour
{
	public GameObject objectivePanelPrefab;

	private OrbitController orbitController;
	private Camera cameraMain;
	private List<ObjectiveTrigger> objectiveList;
	private List<ObjectivePanel> panelList;
	private IEnumerator triggerSearchCoroutine;
	private int numTriggers = 0;

	void Awake()
    {
		objectiveList = new List<ObjectiveTrigger>();
		panelList = new List<ObjectivePanel>();
		cameraMain = Camera.main;
		orbitController = FindObjectOfType<OrbitController>();
	}

	void Start()
	{
		triggerSearchCoroutine = UpdateTriggers(0.5f);
		StartCoroutine(triggerSearchCoroutine);
	}

	void Update()
    {
		numTriggers = objectiveList.Count;
		if (numTriggers > 0)
		{
			for (int i = 0; i < numTriggers; i++)
			{
				if ((objectiveList.Count > i) && (panelList.Count > i))
				{
					ObjectiveTrigger obj = objectiveList[i];
					ObjectivePanel panel = panelList[i];

					if (obj.IsDestroyed())
					{
						panel.gameObject.SetActive(false);
					}
					else
					{
						Vector3 objScreenPosition = cameraMain.WorldToScreenPoint(obj.transform.position);
						Vector3 objToScreen = (obj.transform.position - cameraMain.transform.position).normalized;
						float dotToObj = Vector3.Dot(cameraMain.transform.forward, objToScreen);
						if (dotToObj > 0f)
						{
							objScreenPosition.x = Mathf.Clamp(objScreenPosition.x, 0f, Screen.width);
							objScreenPosition.y = Mathf.Clamp(objScreenPosition.y, 0f, Screen.height);
							panel.transform.position = objScreenPosition;

							panel.SetObjective(obj.objectiveName);

							if (orbitController.GetAutopilot() != null)
							{
								float distanceToObj = Vector3.Distance(obj.transform.position, orbitController.GetAutopilot().transform.position);
								panel.SetDistance(distanceToObj * 10);
							}
						}
					}
				}
			}
		}
    }

	IEnumerator UpdateTriggers(float intervalTime)
	{
		while(true)
		{
			ObjectiveTrigger[] objArray = FindObjectsOfType<ObjectiveTrigger>();
			if (objArray.Length > 0)
			{
				foreach(ObjectiveTrigger obj in objArray)
				{
					if (!objectiveList.Contains(obj))
					{
						objectiveList.Add(obj);
						ObjectivePanel objPanel = GetObjectivePanel();
						if (objPanel != null)
							panelList.Add(objPanel);
					}
				}
			}

			yield return new WaitForSeconds(intervalTime);
		}
	}

	ObjectivePanel GetObjectivePanel()
	{
		ObjectivePanel panel = null;
		GameObject panelObj = Instantiate(objectivePanelPrefab, transform);
		panel = panelObj.GetComponent<ObjectivePanel>();
		return panel;
	}
}
