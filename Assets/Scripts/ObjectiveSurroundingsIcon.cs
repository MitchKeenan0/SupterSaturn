using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveSurroundingsIcon : MonoBehaviour
{
	public Sprite icon;
	public float iconScale = 1f;
	public Color iconColor;
	public bool bSecondary = false;
	public bool bSalient = false;
	public float rarity = 1f;

	private Image iconImage;

    void Awake()
    {
		iconImage = GetComponent<Image>();
    }

	public void InitIcon()
	{
		Vector2 iconSize = Vector2.one * iconScale;
		iconImage.sprite = icon;
		iconImage.rectTransform.sizeDelta = iconSize;
		iconImage.color = iconColor;
	}
}
