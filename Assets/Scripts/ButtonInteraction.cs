﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private TeamFleetHUD teamFleetHUD;
	private Button button;

	void Start()
    {
		teamFleetHUD = FindObjectOfType<TeamFleetHUD>();
		button = GetComponent<Button>();
		button.onClick.AddListener(TaskOnClick);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		int siblingIndex = transform.GetSiblingIndex();
		teamFleetHUD.ButtonHovered(true, siblingIndex);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		int siblingIndex = transform.GetSiblingIndex();
		teamFleetHUD.ButtonHovered(false, siblingIndex);
		Debug.Log("pointer exit");
	}

	void TaskOnClick()
	{
		int siblingIndex = transform.GetSiblingIndex();
		teamFleetHUD.ButtonPressed(siblingIndex);
	}
}
