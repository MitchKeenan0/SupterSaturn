using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
	public int numericID = 0;
	public GameObject cardObjectPrefab;
	public Sprite cardSprite;
	public string cardName = "";
	public int cardLevel = 1;
	public int cardCost = 100;
	public int cardHealth = -1;

	public void SetHealth(int value)
	{
		cardHealth = value;
	}

	public GameObject CashIn()
	{
		GameObject cardObject = Instantiate(cardObjectPrefab, null);
		return cardObject;
	}
}
