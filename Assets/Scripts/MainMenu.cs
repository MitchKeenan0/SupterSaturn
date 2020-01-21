using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	public GameObject loadingPanel;

    void Start()
    {
		loadingPanel.SetActive(false);
    }

	public void ShowLoading()
	{
		loadingPanel.SetActive(true);
	}
}
