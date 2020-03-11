﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreHUD : MonoBehaviour
{
	public GameObject scorePopupPrefab;
	public Text valueText;

	private Camera cameraMain;
	private BattleOutcome battleOutcome;
	private int score = 0;
	private int maxScore = 0;
	private int expectedPopupLoad = 40;
	private List<ScorePopup> popupPanelList;

	void Start()
	{
		popupPanelList = new List<ScorePopup>();
		battleOutcome = FindObjectOfType<BattleOutcome>();
		cameraMain = Camera.main;
		for (int i = 0; i < expectedPopupLoad; i++)
			SpawnPopup();
		valueText.text = score.ToString();
	}

	void Update()
	{
		if (popupPanelList != null && popupPanelList.Count > 0)
		{
			foreach (ScorePopup sp in popupPanelList)
			{
				if (sp.bActive)
				{
					Vector3 screenPos = cameraMain.WorldToScreenPoint(sp.position);
					sp.transform.position = screenPos;
				}
			}
		}
	}

	void UpdateScore(int value, int maxValue)
	{
		maxScore = maxValue;
		score += value;
		valueText.text = score.ToString() + " / " + maxScore.ToString();
		if (score == maxScore)
			battleOutcome.BattleOver(true);
	}

	ScorePopup GetScorePanel()
	{
		ScorePopup listedObject = null;
		foreach(ScorePopup sp in popupPanelList)
		{
			if (!sp.bActive)
			{
				listedObject = sp;
				break;
			}
		}
		if (listedObject == null)
			listedObject = SpawnPopup();
		return listedObject;
	}

	ScorePopup SpawnPopup()
	{
		ScorePopup newScorePanel = Instantiate(scorePopupPrefab, transform).GetComponent<ScorePopup>();
		popupPanelList.Add(newScorePanel);
		newScorePanel.SetActive(false, Vector3.zero);
		return newScorePanel;
	}

	public void PopupScore(Vector3 worldPosition, int scoreValue, int maxValue)
	{
		ScorePopup scoreo = GetScorePanel();
		scoreo.SetActive(true, worldPosition);
		Vector3 positionOnScreen = cameraMain.WorldToScreenPoint(worldPosition);
		scoreo.transform.position = positionOnScreen;
		UpdateScore(scoreValue, maxValue);
	}

	public int GetScore() { return score; }
}
