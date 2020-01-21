using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FleetCreator : MonoBehaviour
{
	public GameObject backupPlayerPrefab;
	public GameObject fleetPanel;
	public GameObject fleetPanelSlotPrefab;
	public GameObject selectionPanel;
	public GameObject selectionPanelCardPrefab;
	public GameObject loadingPanel;
	public Text chevronValueText;
	public Text chevronBalanceText;
	public Image chevronDividerImage;
	public Color availableSlotColor;
	public Color lockedSlotColor;
	public Color moneyColor;
	public Color bankruptColor;
	public Button startGameButton;

	private Player player;
	private SpacecraftViewer spacecraftViewer;
	private FleetPanelSlot selectedPanelSlot; // :'D
	private int maxSlotsAvailable = 3;

	private List<FleetPanelSlot> panelSlots;
	private List<SelectionPanelCard> spacecraftCardList;

	private IEnumerator deselectGraceCoroutine;

	void Start()
    {
		loadingPanel.SetActive(false);
		panelSlots = new List<FleetPanelSlot>();
		spacecraftCardList = new List<SelectionPanelCard>();
		spacecraftViewer = GetComponent<SpacecraftViewer>();
		player = FindObjectOfType<Player>();
		if (player == null)
		{
			GameObject backupPlayerObject = Instantiate(backupPlayerPrefab, null);
			player = backupPlayerObject.GetComponent<Player>();
		}
		chevronValueText.text = player.GetChevrons().ToString();
		InitFleetPanel();
		InitSelectionPanel();
	}

	// also shares 1st time Player fleet
	void InitFleetPanel()
	{
		for (int i = 0; i < 8; i++)
			CreateFleetPanelSlot();

		FleetPanelSlot[] fleetPanelSlots = fleetPanel.GetComponentsInChildren<FleetPanelSlot>();
		int numSlots = fleetPanelSlots.Length;
		for (int i = 0; i < numSlots; i++)
		{
			FleetPanelSlot panelSlot = fleetPanelSlots[i];
			Card spacecraftCard = null;
			if (i < player.spacecraftCards.Length)
				spacecraftCard = player.spacecraftCards[i];

			panelSlot.SetSlot(spacecraftCard);

			Image panelImage = panelSlot.GetComponent<Image>();
			if (i < maxSlotsAvailable)
				panelImage.color = availableSlotColor;
			else
				panelImage.color = lockedSlotColor;
		}

		UpdateChevronBalance();
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
				SelectionPanelCard selectionCard = CreateSelectionCard();
				if (selectionCard != null)
					selectionCard.SetCard(playerCard);
			}
		}
	}

	SelectionPanelCard CreateSelectionCard()
	{
		GameObject cardObj = Instantiate(selectionPanelCardPrefab, selectionPanel.transform);
		SelectionPanelCard card = cardObj.GetComponent<SelectionPanelCard>();
		spacecraftCardList.Add(card);
		return card;
	}

	FleetPanelSlot CreateFleetPanelSlot()
	{
		GameObject slotObj = Instantiate(fleetPanelSlotPrefab, fleetPanel.transform);
		FleetPanelSlot panelSlot = slotObj.GetComponent<FleetPanelSlot>();
		panelSlots.Add(panelSlot);
		return panelSlot;
	}

	void UpdateChevronBalance()
	{
		int total = 0;
		for(int i = 0; i < maxSlotsAvailable; i++)
		{
			int thisCost = panelSlots[i].GetCost();
			total += thisCost;
		}

		chevronBalanceText.text = total.ToString();
		if (total > player.GetChevrons())
		{
			startGameButton.interactable = false;
			chevronBalanceText.color = bankruptColor;
			chevronDividerImage.color = bankruptColor;
		}
		else
		{
			startGameButton.interactable = true;
			chevronBalanceText.color = moneyColor;
			chevronDividerImage.color = moneyColor;
		}
	}

	private IEnumerator DeselectAfterTime(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		selectedPanelSlot = null;
	}

	// UI Button functions
	public void SelectSlot(int buttonIndex)
	{
		selectedPanelSlot = panelSlots[buttonIndex];
		spacecraftViewer.DisplaySpacecraft(buttonIndex);
	}

	public void DeselectSlot()
	{
		deselectGraceCoroutine = DeselectAfterTime(0.2f);
		StartCoroutine(deselectGraceCoroutine);
	}

	public void DemoSlotSprite(Sprite demoSprite, int index)
	{
		if (selectedPanelSlot != null)
		{
			if (demoSprite != null)
			{
				SelectionPanelCard relevantCard = spacecraftCardList[index];
				selectedPanelSlot.PreviewSlot(demoSprite, relevantCard.cardName, relevantCard.cardCost);
			}
			else
			{
				selectedPanelSlot.PreviewSlot(null, null, 0);
			}
		}
	}

	public void SelectSpacecraft(int buttonIndex)
	{
		spacecraftViewer.DisplaySpacecraft(buttonIndex);

		if (selectedPanelSlot != null)
		{
			Card spacecraftCard = spacecraftCardList[buttonIndex].GetCard();
			selectedPanelSlot.SetSlot(spacecraftCard);

			UpdateChevronBalance();
		}
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
