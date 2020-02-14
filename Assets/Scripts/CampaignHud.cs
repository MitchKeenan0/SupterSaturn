using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CampaignHud : MonoBehaviour
{
	public GameObject campaignPanel;

	private Game game;

	private IEnumerator fleetJumpDelay;

    void Start()
    {
		game = FindObjectOfType<Game>();
    }

	public void FleetCreator()
	{
		game.SaveGame();
		fleetJumpDelay = FleetReturn(0.3f);
		StartCoroutine(fleetJumpDelay);
	}

	public void SetPanelActive(bool value)
	{
		campaignPanel.SetActive(value);
		this.enabled = value;
	}

	private IEnumerator FleetReturn(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		SceneManager.LoadScene("FleetScene");
	}
}
