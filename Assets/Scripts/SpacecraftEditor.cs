﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpacecraftEditor : MonoBehaviour
{
	public GameObject mainEditorPanel;
	public Text playerNameText;
	public Text placeholderNameText;
	public Text chevronValueText;
	public Text chevronBalanceText;
	public Image chevronDividerImage;
	public Color moneyColor;
	public Color bankruptColor;
	//public Button startGameButton;
	public Button emptyButton;
	public Button emptyAllButton;

	private Game game;
	private Player player;
	private SpacecraftViewer spacecraftViewer;
	private CardRoster cardRoster;
	private CardSelector cardSelector;
	private SoundManager soundManager;
	private CanvasGroup editorPanelGroup;
	private IEnumerator deselectGraceCoroutine;
	private IEnumerator loadGraceCoroutine;

	void Awake()
	{
		Time.timeScale = 1;
		player = FindObjectOfType<Player>();
		game = FindObjectOfType<Game>();
		soundManager = FindObjectOfType<SoundManager>();
		cardRoster = GetComponentInChildren<CardRoster>();
		cardSelector = GetComponentInChildren<CardSelector>();
		emptyButton.interactable = false;
	}

	void Start()
    {
		spacecraftViewer = GetComponent<SpacecraftViewer>();
		editorPanelGroup = mainEditorPanel.GetComponent<CanvasGroup>();
		loadGraceCoroutine = LoadWait(0.3f);
		StartCoroutine(loadGraceCoroutine);
	}

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		cardRoster.InitFleetPanel();
		cardSelector.InitSelectionPanel();
		UpdateChevronBalance();

		Card firstCard = cardRoster.GetSlots()[0].GetCard();
		if (firstCard != null)
		spacecraftViewer.DisplaySpacecraft(firstCard.numericID);

		string playerName = player.playerName;
		if (playerName == "")
			playerName = "No Name";
		playerNameText.text = player.playerName;
		placeholderNameText.text = player.playerName;
	}

	void UpdateChevronBalance()
	{
		int total = 0;
		Card selectedCard = game.GetSelectedCard();
		if (selectedCard != null)
		{
			int thisCost = selectedCard.cost;
			total += thisCost;
		}
		chevronBalanceText.text = total.ToString();

		if ((total <= game.GetChevrons()) && (total > 0))
		{
			if (game.GetGameMode() != 0)
			{
				//startGameButton.interactable = true;
				chevronBalanceText.color = moneyColor;
				chevronDividerImage.color = moneyColor;
			}
		}
		else
		{
			//startGameButton.interactable = false;
			chevronBalanceText.color = bankruptColor;
			chevronDividerImage.color = bankruptColor;
		}

		chevronValueText.text = game.GetChevrons().ToString();
		///Debug.Log("game chevrons " + game.GetChevrons());
	}

	private IEnumerator DeselectAfterTime(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		cardRoster.SelectSlot(-1);
		cardSelector.SelectCard(-1);
		emptyButton.interactable = false;
	}

	public void SelectSlot(int buttonIndex)
	{
		cardRoster.SelectSlot(buttonIndex);

		// choose slot for selected card
		if ((cardSelector.GetSelectedCard() != null) && 
			(cardRoster.GetSelectedSlot().GetCard() == null))
		{
			SelectSpacecraftCard(buttonIndex);
			emptyButton.interactable = true;
		}

		// update spacecraft viewer
		Card selectedRosterCard = cardRoster.GetSelectedSlot().GetCard();
		if (selectedRosterCard != null)
		{
			int displayID = selectedRosterCard.numericID;
			spacecraftViewer.DisplaySpacecraft(displayID);
			emptyButton.interactable = true;
		}
	}

	public void DeselectSlot()
	{
		deselectGraceCoroutine = DeselectAfterTime(0.2f);
		StartCoroutine(deselectGraceCoroutine);
	}

	public void DemoSlotSprite(Sprite demoSprite, int index)
	{
		cardRoster.DemoSlotSprite(demoSprite, index);
	}

	public void SelectSpacecraftCard(int buttonIndex)
	{
		spacecraftViewer.DisplaySpacecraft(buttonIndex);

		CardRosterSlot selectedRosterSlot = cardRoster.GetSelectedSlot();
		if (selectedRosterSlot != null)
		{
			int selectionIndex = selectedRosterSlot.transform.GetSiblingIndex();
			Card spacecraftCard = cardSelector.GetCards()[buttonIndex].GetCard();
			if ((selectedRosterSlot.GetCard() == null) || (spacecraftCard.numericID != selectedRosterSlot.GetCard().numericID))
			{
				selectedRosterSlot.SetSlot(spacecraftCard, selectionIndex, true);
				UpdateChevronBalance();
				game.SaveGame();
			}
		}

		cardSelector.SelectCard(buttonIndex);
	}

	public void EmptySlot()
	{
		cardRoster.EmptySelectedSlot();
		UpdateChevronBalance();
		game.SaveGame();
	}

	public void EmptyAll()
	{
		cardRoster.EmptyAllSlots();
		UpdateChevronBalance();
		game.SaveGame();
	}

	public void SetName(string value)
	{
		player.SetName(value);
	}

	public void Editor(bool value)
	{
		float alpha = (value) ? 1f : 0f;
		editorPanelGroup.alpha = alpha;
		editorPanelGroup.blocksRaycasts = value;
	}
}
