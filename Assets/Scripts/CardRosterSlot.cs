﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardRosterSlot : MonoBehaviour, IDeselectHandler
{
	public Image slotImage;
	public Text slotNameText;
	public Text costText;

	private SpacecraftEditor spacecraftEditor;
	private Game game;
	private Button button;
	private Sprite originalSprite;
	private HealthBar healthBar;
	private Card slotCard;
	private Color originalImageColor;
	private Color originalCostColor;
	private Color originalNameColor;
	private string originalSlotName = "";
	private int originalSlotCost = 0;

	public Card GetCard() { return slotCard; }
	public int GetCost() { return (slotCard != null) ? slotCard.cost : 0; }

	void Awake()
	{
		game = FindObjectOfType<Game>();
		healthBar = GetComponentInChildren<HealthBar>(true);
		originalNameColor = slotNameText.color;
		originalCostColor = costText.color;
	}

	void Start()
	{
		spacecraftEditor = GetComponentInParent<SpacecraftEditor>();
		button = GetComponent<Button>();
		button.onClick.AddListener(TaskOnClick);
		slotImage.preserveAspect = true;
	}

	void TaskOnClick()
	{
		int myIndex = transform.GetSiblingIndex();
		spacecraftEditor.SelectSlot(myIndex);
	}

	public void OnDeselect(BaseEventData data)
	{
		spacecraftEditor.DeselectSlot();
		PreviewSlot(null, null, 0);
	}

	public void PreviewSlot(Sprite sprite, string name, int cost)
	{
		if (sprite != null)
		{
			slotImage.enabled = true;
			slotImage.sprite = sprite;
			slotNameText.enabled = true;
			slotNameText.text = name;
			costText.text = cost.ToString();
			costText.enabled = true;
			Color previewColor = new Color(1f, 1f, 1f, 0.5f);
			slotNameText.color = costText.color = previewColor;
		}
		else
		{
			slotImage.sprite = originalSprite;
			if (originalSprite == null)
				slotImage.enabled = false;
			slotNameText.text = originalSlotName;
			costText.text = originalSlotCost.ToString();
			slotNameText.color = originalNameColor;
			costText.color = originalCostColor;
			costText.enabled = false;
		}
	}

	public void SetSlot(Card card, int index, bool newCard)
	{
		slotCard = card;
		if (slotCard != null)
		{
			slotImage.sprite = slotCard.cardSprite;
			slotImage.enabled = true;
			originalImageColor = slotImage.color;
			originalSprite = slotCard.cardSprite;

			slotNameText.enabled = true;
			slotNameText.text = slotCard.cardName;
			slotNameText.color = originalNameColor;
			originalSlotName = slotCard.cardName;

			costText.text = slotCard.cost.ToString();
			costText.color = originalCostColor;
			costText.enabled = true;
			originalSlotCost = slotCard.cost;

			int maxHealth = slotCard.health;
			int currentHealth = game.GetSavedHealth();
			if (newCard)
			{
				currentHealth = card.health;
				game.SetSavedHealth(card.health);
			}

			if (currentHealth < 0)
				currentHealth = maxHealth;
			healthBar.InitHeath(maxHealth, currentHealth);
			healthBar.enabled = true;
		}
		else
		{
			slotImage.enabled = false;
			slotNameText.text = "";
			slotNameText.enabled = false;
			slotNameText.text = "";
			costText.text = "";
			costText.enabled = false;
			slotNameText.color = Color.clear;

			healthBar.InitHeath(-1, -1);
			healthBar.enabled = false;
		}

		game.SetSelectedCard(card);

		Spacecraft sp = null;
		if (card != null)
			sp = card.cardObjectPrefab.GetComponent<Spacecraft>();

		//game.SetPlayerSpacecraft(sp);

		SetSlotActive(card != null);
	}

	public void SetSlotActive(bool value)
	{
		slotImage.enabled = value;
		slotNameText.enabled = value;
		costText.enabled = value;
		healthBar.gameObject.SetActive(value);
	}
}
