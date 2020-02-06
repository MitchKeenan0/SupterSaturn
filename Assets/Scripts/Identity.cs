using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identity : MonoBehaviour
{
	public string identityName = "No Name";
	public Color identityColor = Color.green;

	private NameLibrary nameLibrary;
	private float likelyhoodOfAttackingPlayer = 0f;
	private float likelyhoodOfDefendingPlayer = 0f;
	private float identityWealth = 0f;

	void Awake()
    {
		nameLibrary = FindObjectOfType<NameLibrary>();
		InitIdentity();
    }

	public void InitIdentity()
	{
		identityName = nameLibrary.GetIdentityName();

		Color color = new Color();
		color.a = 1;

		likelyhoodOfAttackingPlayer = Random.Range(0f, 1f);
		color.r = likelyhoodOfAttackingPlayer;

		likelyhoodOfDefendingPlayer = Random.Range(0f, 1f);
		color.b = likelyhoodOfDefendingPlayer;

		identityWealth = Random.Range(0f, 1f);
		color.g = identityWealth;

		identityColor = color;
	}
}
