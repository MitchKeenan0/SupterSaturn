using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRoster : MonoBehaviour
{
	public GameObject cardRosterSlotPrefab;
	public int maxSlotsAvailable = 8;

	private Game game;
	private CardSelector cardSelector;
	private CardRosterSlot selectedSlot = null;
	private List<CardRosterSlot> rosterSlots;

	public List<CardRosterSlot> GetSlots() { return rosterSlots; }
	public CardRosterSlot GetSelectedSlot() { return selectedSlot; }

	void Awake()
    {
		game = FindObjectOfType<Game>();
		cardSelector = FindObjectOfType<CardSelector>();
		rosterSlots = new List<CardRosterSlot>();
	}

	public void InitFleetPanel()
	{
		// spawning card slots
		for (int i = 0; i < maxSlotsAvailable; i++)
			CreateFleetPanelSlot();

		// load cards
		Card selectedCard = game.GetSelectedCard();
		if (selectedCard == null)
			selectedCard = game.initialSpacecraftCards[0];

		// assign slots
		CardRosterSlot[] cardSlotsArray = GetComponentsInChildren<CardRosterSlot>();
		int numSlots = cardSlotsArray.Length;
		for (int i = 0; i < numSlots; i++)
		{
			CardRosterSlot panelSlot = cardSlotsArray[i];
			Card spacecraftCard = null;
			if ((i < 1) && (i < maxSlotsAvailable))
				spacecraftCard = selectedCard;
			if (spacecraftCard != null)
				panelSlot.SetSlot(spacecraftCard, i, false);
			else
				panelSlot.SetSlot(null, i, false);
		}
	}

	CardRosterSlot CreateFleetPanelSlot()
	{
		GameObject slotObj = Instantiate(cardRosterSlotPrefab, transform);
		CardRosterSlot panelSlot = slotObj.GetComponent<CardRosterSlot>();
		rosterSlots.Add(panelSlot);
		return panelSlot;
	}

	public void SelectSlot(int buttonIndex)
	{
		if (buttonIndex < 0)
			selectedSlot = null;
		else if (buttonIndex < rosterSlots.Count)
			selectedSlot = rosterSlots[buttonIndex];
	}

	public void DemoSlotSprite(Sprite demoSprite, int index)
	{
		if (selectedSlot != null)
		{
			if (demoSprite != null)
			{
				SelectionPanelCard relevantCard = cardSelector.GetCards()[index];
				selectedSlot.PreviewSlot(demoSprite, relevantCard.cardName, relevantCard.cardCost);
			}
			else
			{
				selectedSlot.PreviewSlot(null, null, 0);
			}
		}
	}

	public void EmptySelectedSlot()
	{
		if (selectedSlot != null)
		{
			int slotIndex = selectedSlot.transform.GetSiblingIndex();
			selectedSlot.SetSlot(null, slotIndex, false);
			game.SetSelectedCard(null);
		}
		selectedSlot = null;
	}

	public void EmptyAllSlots()
	{
		int numSlots = rosterSlots.Count;
		for (int i = 0; i < numSlots; i++)
		{
			CardRosterSlot ps = rosterSlots[i];
			if (ps != null)
				ps.SetSlot(null, i, false);
		}
		game.SetSelectedCard(null);
		selectedSlot = null;
	}
}
