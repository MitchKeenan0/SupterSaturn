using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreHUD : MonoBehaviour
{
	public GameObject scorePopupPrefab;
	public Text valueText;
	public GameObject scoreContextPanel;
	public float contextDuration = 1.5f;

	private ScoreComboManager comboManager;
	private Camera cameraMain;
	private ExpCanvas expCanvas;
	private CanvasGroup scoreContextCG;
	private Text scoreContextText;
	private int score = 0;
	private int expectedPopupLoad = 40;
	private List<ScorePopup> popupPanelList;
	private IEnumerator scoreContextCoroutine;

	void Start()
	{
		popupPanelList = new List<ScorePopup>();
		scoreContextCG = scoreContextPanel.GetComponent<CanvasGroup>();
		scoreContextCG.alpha = 0f;
		scoreContextText = scoreContextPanel.GetComponentInChildren<Text>();
		expCanvas = FindObjectOfType<ExpCanvas>();
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

	public void UpdateScore(int value)
	{
		score += value;
		valueText.text = score.ToString();
		expCanvas.AddExp(value);
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

	//public void UpdateMaxScore(int max)
	//{
	//	maxScore += max;
	//	UpdateScore(0, maxScore);
	//}

	public void PopupScore(Vector3 worldPosition, int scoreValue)
	{
		ScorePopup scoreo = GetScorePanel();
		int comboScore = comboManager.Score(scoreValue);
		Color comboColor = comboManager.GetTierColor();
		scoreo.SetActive(true, comboScore, worldPosition, comboColor);
		Vector3 positionOnScreen = cameraMain.WorldToScreenPoint(worldPosition);
		scoreo.transform.position = positionOnScreen;
		UpdateScore(comboScore);
	}

	public int GetScore() { return score; }

	public void ToastContext(string context)
	{
		scoreContextText.text = context;
		scoreContextCoroutine = ContextToast(contextDuration);
		StartCoroutine(scoreContextCoroutine);
	}

	private IEnumerator ContextToast(float duration)
	{
		scoreContextCG.alpha = 1f;
		yield return new WaitForSeconds(duration);
		scoreContextText.text = "";
		scoreContextCG.alpha = 0f;
	}
}
