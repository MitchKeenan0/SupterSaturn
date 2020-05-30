using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveSurroundingsIcon : MonoBehaviour
{
	public Sprite icon;
	public int gameValue = 100;
	public float iconScale = 1f;
	public Color iconColor;
	public bool bSecondary = false;
	public bool bSalient = false;
	public float rarity = 1f;
	public bool bOrbital = false;
	public Sprite orbitRingSprite;
	public Vector3 startOrbitOffset = Vector3.zero;
	public Quaternion startOrbitRotation = Quaternion.identity;
	public ObjectiveElement objectiveElementPrefab;

	private ObjectiveElement element;
	private Image iconImage;

	void Awake()
    {
		iconImage = GetComponent<Image>();
		if (bOrbital)
			iconImage = GetComponentInChildren<Image>();
    }

	public void InitIcon()
	{
		Vector2 iconSize = Vector2.one * iconScale;
		iconImage.sprite = icon;
		iconImage.rectTransform.sizeDelta = iconSize;
		iconImage.color = iconColor;
	}
}
