using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveName : MonoBehaviour
{
	public string objHudPreface = "";
	public string objPanelPreface = "";

	private ObjectiveHUD objHud = null;
	private ObjectivePanel objPanel = null;
	private ObjectiveTrigger objTrigger = null;
	private NameLibrary nameLibrary;
	private IEnumerator loadCoroutine;

	void Start()
    {
		objHud = FindObjectOfType<ObjectiveHUD>();
		objPanel = FindObjectOfType<ObjectivePanel>();
		objTrigger = FindObjectOfType<ObjectiveTrigger>();
		nameLibrary = FindObjectOfType<NameLibrary>();
		loadCoroutine = LoadWait(Time.deltaTime * 1.5f);
		StartCoroutine(loadCoroutine);
	}

	IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		string objName = nameLibrary.GetIdentityName();
		if (objHud != null)
			objHud.SetObjectiveName(objHudPreface + objName);
		if (objPanel != null)
			objPanel.SetObjective(objPanelPreface + objName);
	}
}
