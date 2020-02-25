using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelector : MonoBehaviour
{
	public GameObject cardPrefab;

	private Game game;
	private SelectionPanelCard selectedCard = null;
	private List<SelectionPanelCard> cardList;

	public List<SelectionPanelCard> GetCards() { return cardList; }
	public SelectionPanelCard GetSelectedCard() { return selectedCard; }

	void Start()
    {
		game = FindObjectOfType<Game>();
		cardList = new List<SelectionPanelCard>();
    }

	public void InitSelectionPanel()
	{
		List<Card> cardLibrary = new List<Card>(game.cardLibrary);
		int numCards = cardLibrary.Count;
		for (int i = 0; i < numCards; i++)
		{
			Card card = cardLibrary[i];
			if (card != null)
			{
				SelectionPanelCard selectionCard = CreateSelectionCard();
				if (selectionCard != null)
					selectionCard.SetCard(card);
			}
		}
	}

	SelectionPanelCard CreateSelectionCard()
	{
		GameObject cardObj = Instantiate(cardPrefab, transform);
		SelectionPanelCard card = cardObj.GetComponent<SelectionPanelCard>();
		cardList.Add(card);
		return card;
	}

	public void SelectCard(int buttonIndex)
	{
		if (buttonIndex < 0)
			selectedCard = null;
		else if (buttonIndex < cardList.Count)
			selectedCard = cardList[buttonIndex];
	}
}
