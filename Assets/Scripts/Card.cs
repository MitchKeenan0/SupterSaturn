using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
	public int numericID = 0;
	public string cardName = "";
	public string cardDescription = "";
	public GameObject cardObjectPrefab;
	public Sprite cardSprite;
	public Sprite iconSprite;

	public int level = 1;
	public int cost = 10;
	public int health = 1;
	public int damage = 1;
	public float rateOfFire = 1;
	public Sprite weaponSprite;

	public float mainEnginePower = 1f;
	public float turningPower = 1f;
	public float maneuverPower = 0.6f;
}
