using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CampaignHud : MonoBehaviour
{
	public GameObject campaignPanel;

	private Game game;
	private Campaign campaign;
	private SoundManager soundManager;

	private IEnumerator fleetJumpDelay;

    void Start()
    {
		game = FindObjectOfType<Game>();
		campaign = FindObjectOfType<Campaign>();
		soundManager = FindObjectOfType<SoundManager>();
    }

	public void SetPanelActive(bool value)
	{
		campaignPanel.SetActive(value);
		this.enabled = value;
	}

	public void FleetCreator()
	{
		soundManager.NegativeButton();
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
