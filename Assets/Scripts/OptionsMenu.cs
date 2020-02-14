using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
	private MainMenu menu;

	void Awake()
    {
		DontDestroyOnLoad(gameObject);
		menu = FindObjectOfType<MainMenu>();
    }

	public void EnterOptions()
	{
		if (menu != null)
			menu.SetMenuPanelVisible(false);

		gameObject.SetActive(true);
		this.enabled = true;
	}

	public void ExitOptions()
	{
		if (menu != null)
			menu.SetMenuPanelVisible(true);

		this.enabled = false;
		gameObject.SetActive(false);
	}
}
