﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftIconHUD : MonoBehaviour
{
	public GameObject iconParent;
	public GameObject iconPrefab;

	private Camera cameraMain;
	private ObjectManager objectManager;
	private List<Spacecraft> spacecraftList;
	private List<Image> iconImageList;

	void Start()
    {
		cameraMain = Camera.main;
		objectManager = FindObjectOfType<ObjectManager>();
		InitIconFleet();
    }

	void InitIconFleet()
	{
		spacecraftList = objectManager.GetSpacecraftList();
		iconImageList = new List<Image>();
		Image[] images = GetComponentsInChildren<Image>();
		foreach (Image img in images)
		{
			iconImageList.Add(img);
			img.gameObject.SetActive(false);
		}
		int numCrafts = spacecraftList.Count;
		for (int i = 0; i < numCrafts; i++)
		{
			if (spacecraftList[i] != null)
			{
				Spacecraft sp = spacecraftList[i];
				Image img = null;
				if (i < iconImageList.Count)
					img = iconImageList[i];
				else
					img = CreateSpacecraftIcon().GetComponent<Image>();

				Color iconColor = img.color;
				switch (sp.GetAgent().teamID)
				{
					case 0:
						iconColor = Color.white;
						break;
					case 1:
						iconColor = Color.red;
						break;
					case 2:
						iconColor = Color.blue;
						break;
					case 3:
						iconColor = Color.green;
						break;
					default:
						iconColor = img.color;
						break;
				}

				img.color = iconColor;

				sp.SetHUDIcon(img.gameObject);
				img.gameObject.SetActive(true);
			}
		}
	}

	GameObject CreateSpacecraftIcon()
	{
		GameObject product = null;
		if (iconPrefab != null)
		{
			product = Instantiate(iconPrefab, iconParent.transform);
			iconImageList.Add(product.GetComponent<Image>());
		}

		return product;
	}

	void OnGUI()
	{
		UpdateCraftIcons();
	}

	void UpdateCraftIcons()
	{
		spacecraftList = objectManager.GetSpacecraftList();
		if (spacecraftList.Count > 0)
		{
			for (int i = 0; i < spacecraftList.Count; i++)
			{
				if (i < spacecraftList.Count && (spacecraftList[i] != null))
				{
					Spacecraft sp = spacecraftList[i];
					Image img;
					if (sp != null && sp.GetHUDIcon() != null)
					{
						img = sp.GetHUDIcon().GetComponent<Image>();

						if ((sp.GetMarks() > 0) || (sp.GetAgent().teamID == 0))
						{
							img.enabled = true;
							if (!img.gameObject.activeInHierarchy)
								img.gameObject.SetActive(true);
							img.sprite = sp.craftIcon;
							img.rectTransform.localScale = Vector3.one * sp.iconScale;
							Vector3 craftScreenPosition = cameraMain.WorldToScreenPoint(sp.transform.position);
							Vector3 craftToScreen = (sp.transform.position - cameraMain.transform.position).normalized;
							float dotToCraft = Vector3.Dot(cameraMain.transform.forward, craftToScreen);
							if (dotToCraft > 0f)
							{
								craftScreenPosition.x = Mathf.Clamp(craftScreenPosition.x, 0f, Screen.width);
								craftScreenPosition.y = Mathf.Clamp(craftScreenPosition.y, 0f, Screen.height);
								img.transform.position = craftScreenPosition;
							}
							else
							{
								img.sprite = null;
								img.gameObject.SetActive(false);
							}

							Vector3 toCamera = cameraMain.transform.position - sp.transform.position;
							Color iconColor = img.color;
							iconColor.a = Mathf.Clamp(toCamera.magnitude * 0.1f, 0.1f, 1f);
							img.color = iconColor;
						}
						else
						{
							img.enabled = false;
							if (img.gameObject.activeInHierarchy)
								img.gameObject.SetActive(false);
						}
					}
				}
			}
		}
	}

	public void ModifySpacecraftList(Spacecraft sp, bool stays)
	{
		if (stays)
		{
			spacecraftList.Add(sp);
		}
		else
		{
			if (sp.GetHUDIcon() != null)
				sp.GetHUDIcon().SetActive(false);
			sp.SetHUDIcon(null);
			spacecraftList.Remove(sp);
		}
	}
}
