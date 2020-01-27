using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FleetPanelSlot : MonoBehaviour, IDeselectHandler
{
	public Image slotImage;
	public Text slotNameText;
	public Text costText;

	private FleetCreator fleetCreator;
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
	public int GetCost() { return (slotCard != null) ? slotCard.cardCost : 0; }

	void Awake()
	{
		game = FindObjectOfType<Game>();
		healthBar = GetComponentInChildren<HealthBar>(true);
		originalNameColor = slotNameText.color;
		originalCostColor = costText.color;
	}

	void Start()
	{
		fleetCreator = GetComponentInParent<FleetCreator>();
		button = GetComponent<Button>();
		button.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick()
	{
		int myIndex = transform.GetSiblingIndex();
		fleetCreator.SelectSlot(myIndex);
	}

	public void OnDeselect(BaseEventData data)
	{
		fleetCreator.DeselectSlot();
		PreviewSlot(null, null, 0);
	}

	public void PreviewSlot(Sprite sprite, string name, int cost)
	{
		if (sprite != null)
		{
			slotImage.enabled = true;
			slotImage.sprite = sprite;
			slotNameText.text = name;
			costText.text = cost.ToString();
			Color previewColor = new Color(1f, 1f, 1f, 0.5f);
			slotNameText.color = costText.color = previewColor;
		}
		else
		{
			slotImage.sprite = originalSprite;
			slotNameText.text = originalSlotName;
			costText.text = originalSlotCost.ToString();
			slotNameText.color = originalNameColor;
			costText.color = originalCostColor;
		}
	}

	public void SetSlot(Card card, int slotIndex)
	{
		slotCard = card;
		if (slotCard != null)
		{
			slotImage.sprite = slotCard.cardSprite;
			originalImageColor = slotImage.color;
			originalSprite = slotCard.cardSprite;

			slotNameText.enabled = true;
			slotNameText.text = slotCard.cardName;
			slotNameText.color = originalNameColor;
			originalSlotName = slotCard.cardName;

			costText.text = slotCard.cardCost.ToString();
			costText.color = originalCostColor;
			originalSlotCost = slotCard.cardCost;

			game.RemoveSpacecraft(slotIndex);
			game.SetSelectedCard(slotIndex, card);
			game.AddSpacecraft(null, card);

			int maxHealth = slotCard.cardObjectPrefab.GetComponent<Health>().maxHealth;
			int currentHealth = game.GetSavedHealth(slotIndex);
			if (currentHealth < 0)
				currentHealth = maxHealth;
			///Debug.Log("card health: " + currentHealth);
			healthBar.InitHeath(maxHealth, currentHealth);
			healthBar.enabled = true;
		}
		else
		{
			slotImage.enabled = false;
			slotNameText.text = "";
			slotNameText.text = "";
			costText.text = "";
			slotNameText.color = Color.clear;

			healthBar.InitHeath(-1, -1);
			healthBar.enabled = false;

			game.RemoveSelectedCard(slotIndex);
		}
	}
}
