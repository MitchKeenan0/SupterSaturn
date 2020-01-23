using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CampaignHud : MonoBehaviour
{
	private Game game;

    void Start()
    {
		game = FindObjectOfType<Game>();
    }

	public void FleetCreator()
	{
		SceneManager.LoadScene("FleetScene");
	}
}
