using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CampaignBattle : MonoBehaviour
{
	public GameObject battlePanel;
	public Vector3 battlePanelOffset = new Vector3(0f, 0f, 0f);

	private Camera cameraMain;
	private CampaignLocation battleLocation;
	private List<Fleet> fleetList;
	private bool bOnScreen = false;

    void Start()
    {
		fleetList = new List<Fleet>();
		battlePanel.SetActive(false);
		cameraMain = Camera.main;
    }

	void Update()
	{
		if (bOnScreen)
			UpdateScreenPosition();
	}

	public void HeraldBattle(CampaignLocation location, Fleet[] fleets)
	{
		bOnScreen = true;
		battlePanel.SetActive(true);
		battleLocation = location;
		fleetList.Clear();
		foreach (Fleet f in fleets)
			fleetList.Add(f);
	}

	public void BeginBattle()
	{
		bOnScreen = false;
		battlePanel.SetActive(false);
		SceneManager.LoadSceneAsync("BattleScene");
	}

	void UpdateScreenPosition()
	{
		Vector3 battleScreenPosition = cameraMain.WorldToScreenPoint(battleLocation.transform.position);
		battlePanel.transform.position = battleScreenPosition + battlePanelOffset;
	}
}
