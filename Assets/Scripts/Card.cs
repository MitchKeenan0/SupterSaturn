using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
	public GameObject cardObjectPrefab;
	public Sprite cardSprite;
	public string cardName = "";
	public int cardLevel = 1;
	public int cardCost = 100;

	public GameObject CashIn()
	{
		GameObject cardObject = Instantiate(cardObjectPrefab, null);
		return cardObject;
	}
}
