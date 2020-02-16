using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CampaignHud : MonoBehaviour
{
	public GameObject campaignPanel;

	private Game game;
	private Campaign campaign;

	private IEnumerator fleetJumpDelay;

    void Start()
    {
		game = FindObjectOfType<Game>();
		campaign = FindObjectOfType<Campaign>();
    }

	public void SetPanelActive(bool value)
	{
		campaignPanel.SetActive(value);
		this.enabled = value;
	}

	public void FleetCreator()
	{
		campaign.SaveCampaign();
		game.SaveGame();
		fleetJumpDelay = FleetReturn(0.3f);
		StartCoroutine(fleetJumpDelay);
	}

	private IEnumerator FleetReturn(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		SceneManager.LoadScene("FleetScene");
	}
}
