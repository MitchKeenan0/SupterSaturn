using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
	public GameObject tooltipPanel;
	public Text spacecraftNameText;
	public Text descriptionText;

	private Camera cameraMain;

    void Awake()
    {
		cameraMain = Camera.main;
		tooltipPanel.SetActive(false);
	}

	public void SetCard(Card card)
	{
		spacecraftNameText.text = card.cardName;
		descriptionText.text = card.cardDescription;
	}

	public void ShowTooltip(bool value)
	{
		tooltipPanel.SetActive(value);
	}
}
