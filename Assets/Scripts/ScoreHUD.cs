using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreHUD : MonoBehaviour
{
	public GameObject scorePopupPrefab;
	public Text valueText;

	private ScoreComboManager comboManager;
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
		comboManager = FindObjectOfType<ScoreComboManager>();
		cameraMain = Camera.main;
		for (int i = 0; i < expectedPopupLoad; i++)
			SpawnPopup();
		valueText.text = score.ToString();
	}

	void Update()
	{
		if (!cameraMain)
			cameraMain = Camera.main;
		if ((cameraMain != null) && (popupPanelList != null) && (popupPanelList.Count > 0))
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
		if (!battleOutcome)
			battleOutcome = FindObjectOfType<BattleOutcome>();
		if ((battleOutcome != null) && (score >= maxScore))
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
		newScorePanel.SetActive(false, 0, Vector3.zero, Color.white);
		return newScorePanel;
	}

	public void InitScore(int max)
	{
		UpdateScore(0, max);
	}

	public void PopupScore(Vector3 worldPosition, int scoreValue, int maxValue)
	{
		ScorePopup scoreo = GetScorePanel();
		int comboScore = comboManager.Score(scoreValue);
		Color comboColor = comboManager.GetTierColor();
		scoreo.SetActive(true, comboScore, worldPosition, comboColor);
		Vector3 positionOnScreen = cameraMain.WorldToScreenPoint(worldPosition);
		scoreo.transform.position = positionOnScreen;
		UpdateScore(comboScore, maxValue);
	}

	public int GetScore() { return score; }
}
