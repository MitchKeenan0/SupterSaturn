﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePopup : MonoBehaviour
{
	public Text valueText;
	public bool bActive = false;
	public AnimationCurve popupScaleCurve;
	public Vector3 position = Vector3.zero;

	private IEnumerator lifetimeCoroutine;
	private CanvasGroup canvasGroup;

	void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0f;
	}

    public void SetActive(bool value, int score, Vector3 pos, Color col)
	{
		valueText.text = ("+" + score.ToString());
		valueText.color = col;
		valueText.enabled = value;
		canvasGroup.alpha = value ? 1f : 0f;
		bActive = value;
		if (bActive)
		{
			position = pos;
			lifetimeCoroutine = Lifetime(1f);
			StartCoroutine(lifetimeCoroutine);
		}
		else
		{
			valueText.fontSize = 0;
		}
	}

	IEnumerator Lifetime(float waitTime)
	{
		float time = 0f;
		while (time < waitTime)
		{
			yield return new WaitForSeconds(Time.deltaTime);
			time += Time.deltaTime;
			int size = Mathf.FloorToInt(popupScaleCurve.Evaluate(time) * 80);
			valueText.fontSize = size;
		}
		SetActive(false, 0, Vector3.zero, Color.white);
	}
}
