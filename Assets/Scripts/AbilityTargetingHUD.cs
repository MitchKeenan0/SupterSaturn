using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityTargetingHUD : MonoBehaviour
{
	public GameObject targetPanelPrefab;

	private List<AbilityTargetPanel> targetPanelList;
	private bool bActive = false;

    void Start()
    {
		targetPanelList = new List<AbilityTargetPanel>();
	}

    void Update()
    {
		if (bActive)
		{
			UpdateTargets();
		}
    }

	void UpdateTargets()
	{
		int numTargets = targetPanelList.Count;
		for(int i = 0; i < numTargets; i++)
		{

		}
	}

	GameObject SpawnTargetPanel()
	{
		GameObject panel = Instantiate(targetPanelPrefab, transform);
		AbilityTargetPanel atp = panel.GetComponent<AbilityTargetPanel>();
		targetPanelList.Add(atp);
		return panel;
	}
}
