using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FleetPanelSlot : MonoBehaviour, IDeselectHandler
{
	public Card slotCard;
	public Image slotImage;
	public Text slotNameText;
	public Text costText;

	private FleetCreator fleetCreator;
	private Button button;
	private Sprite originalSprite;
	private Color originalImageColor;
	private Color originalCostColor;
	private Color originalNameColor;
	private string originalSlotName = "";
	private int originalSlotCost = 0;

	public Card GetCard() { return slotCard; }
	public int GetCost() { return (slotCard != null) ? slotCard.cardCost : 0; }

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

	public void SetSlot(Card card)
	{
		slotCard = card;
		if (card != null)
		{
			slotImage.sprite = card.cardSprite;
			originalImageColor = slotImage.color;
			originalSprite = card.cardSprite;

			slotNameText.text = card.cardName;
			originalSlotName = card.cardName;
			originalNameColor = slotNameText.color;

			costText.text = card.cardCost.ToString();
			originalCostColor = costText.color;
			originalSlotCost = card.cardCost;
		}
		else
		{
			slotImage.enabled = false;
			slotNameText.text = "";
			slotNameText.text = "";
			costText.text = "";
		}
	}
}
