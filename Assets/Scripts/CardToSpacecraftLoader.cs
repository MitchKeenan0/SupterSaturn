﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardToSpacecraftLoader : MonoBehaviour
{
	public Spacecraft UploadCardToSpacecraft(Card card, Spacecraft sp)
	{
		Spacecraft csp = sp;
		csp.mainEnginePower = card.mainEnginePower;
		csp.turningPower = card.turningPower;
		csp.maneuverPower = card.maneuverPower;
		csp.spacecraftName = card.cardName;
		csp.craftIcon = card.iconSprite;
		csp.craftDiagram = card.cardSprite;
		csp.numericID = card.numericID;

		if (csp.GetComponent<Health>())
		{
			csp.GetComponent<Health>().maxHealth = card.health;
		}

		if (csp.GetComponent<Weapon>() && csp.GetComponent<Weapon>().munitionPrefab.GetComponent<Projectile>())
		{
			Weapon cspWeapon = csp.GetComponent<Weapon>();
			cspWeapon.munitionPrefab.GetComponent<Projectile>().damage = card.damage;
			cspWeapon.fireRate = card.rateOfFire;
			cspWeapon.weaponSprite = card.weaponSprite;
		}

		/*
		public int level = 1;
		public int cost = 10;
		*/

		return csp;
	}
}