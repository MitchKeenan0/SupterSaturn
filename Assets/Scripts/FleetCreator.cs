using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FleetCreator : MonoBehaviour
{
	public GameObject backupPlayerPrefab;
	public GameObject selectionPanel;
	public GameObject selectionPanelCardPrefab;
	public GameObject fleetPanel;
	public GameObject fleetPanelSlotPrefab;
	public GameObject loadingPanel;
	public Text chevronValueText;
	public Color availableSlotColor;
	public Color lockedSlotColor;

	private Player player;
	private SpacecraftViewer spacecraftViewer;
	private int maxSlotsAvailable = 3;

    void Start()
    {
		loadingPanel.SetActive(false);
		spacecraftViewer = GetComponent<SpacecraftViewer>();
		player = FindObjectOfType<Player>();
		if (player == null)
		{
			GameObject backupPlayerObject = Instantiate(backupPlayerPrefab, null);
			player = backupPlayerObject.GetComponent<Player>();
		}
		chevronValueText.text = player.GetChevrons().ToString();
		InitSelectionPanel();
		InitFleetPanel();
	}

	void InitSelectionPanel()
	{
		Card[] playerSpacecraftCards = player.spacecraftCards;
		int numCards = playerSpacecraftCards.Length;
		for(int i = 0; i < numCards; i++)
		{
			Card playerCard = playerSpacecraftCards[i];
			if (playerCard != null)
			{
				SelectionPanelCard selectionCard = CreateSpacecraftCard();
				if (selectionCard != null)
					selectionCard.SetCard(playerCard.cardSprite, playerCard.cardLevel);
			}
		}
	}

	void InitFleetPanel()
	{
		for(int i = 0; i < 8; i++)
			CreateFleetPanelSlot();

		FleetPanelSlot[] fleetPanelSlots = fleetPanel.GetComponentsInChildren<FleetPanelSlot>();
		int numSlots = fleetPanelSlots.Length;
		for(int i = 0; i < numSlots; i++)
		{
			FleetPanelSlot panelSlot = fleetPanelSlots[i];
			Card spacecraftCard = null;
			if (i < player.spacecraftCards.Length)
				spacecraftCard = player.spacecraftCards[i];
			if (spacecraftCard != null)
				panelSlot.SetSlot(player.spacecraftCards[i].cardSprite, player.spacecraftCards[i].cardName);
			else
				panelSlot.SetSlot(null, null);

			Image panelImage = panelSlot.GetComponent<Image>();
			if (i < maxSlotsAvailable)
				panelImage.color = availableSlotColor;
			else
				panelImage.color = lockedSlotColor;
		}
	}

	SelectionPanelCard CreateSpacecraftCard()
	{
		GameObject cardObj = Instantiate(selectionPanelCardPrefab, selectionPanel.transform);
		SelectionPanelCard card = cardObj.GetComponent<SelectionPanelCard>();
		return card;
	}

	FleetPanelSlot CreateFleetPanelSlot()
	{
		GameObject slotObj = Instantiate(fleetPanelSlotPrefab, fleetPanel.transform);
		FleetPanelSlot panelSlot = slotObj.GetComponent<FleetPanelSlot>();
		return panelSlot;
	}

	public void Select(int buttonIndex)
	{
		spacecraftViewer.DisplaySpacecraft(buttonIndex);
	}

	public void StartGame()
	{
		loadingPanel.SetActive(true);
		SceneManager.LoadScene("CampaignScene");
	}

	public void BackToMenu()
	{
		SceneManager.LoadScene("BaseScene");
	}
}
