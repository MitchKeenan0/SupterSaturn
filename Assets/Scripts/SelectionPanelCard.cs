using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectionPanelCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public Image cardImage;
	public string cardName;
	public Text cardLevelText;
	public int cardCost;

	private Tooltip tooltip;
	private Card card;
	private FleetCreator fleetCreator;
	private Button button;

	public Card GetCard() { return card; }

	void Awake()
	{
		tooltip = GetComponent<Tooltip>();
		fleetCreator = GetComponentInParent<FleetCreator>();
		button = GetComponentInChildren<Button>();
		button.onClick.AddListener(TaskOnClick);
	}

    public void SetCard(Card c)
	{
		card = c;

		cardImage.sprite = c.cardSprite;
		cardImage.preserveAspect = true;
		cardCost = c.cost;

		if (c != null)
		{
			cardName = c.cardName;
			cardLevelText.text = GetRomanNumeral(c.level);
		}
		else
		{
			cardName = "";
			cardLevelText.text = "";
		}

		tooltip.SetCard(c);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		int siblingIndex = transform.GetSiblingIndex();
		fleetCreator.DemoSlotSprite(cardImage.sprite, siblingIndex);
		tooltip.ShowTooltip(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		fleetCreator.DemoSlotSprite(null, 0);
		tooltip.ShowTooltip(false);
	}

	void TaskOnClick()
	{
		int siblingIndex = transform.GetSiblingIndex();
		fleetCreator.SelectSpacecraftCard(siblingIndex);
	}

	string GetRomanNumeral(int value)
	{
		string numeral = "";
		switch(value)
		{
			case 1:
				numeral = "I"; break;
			case 2:
				numeral = "II"; break;
			case 3:
				numeral = "III"; break;
			case 4:
				numeral = "IV"; break;
			case 5:
				numeral = "V"; break;
			default: break;
		}
		return numeral;
	}
}
