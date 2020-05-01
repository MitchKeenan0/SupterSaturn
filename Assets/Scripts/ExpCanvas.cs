using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpCanvas : MonoBehaviour
{
	public float levelSizeScaling = 1.6f;
	public RectTransform expPanel;
	public RectTransform expBar;
	public Text expText;
	public RectTransform levelUpPanel;
	public Text levelUpText;
	public Upgrade[] upgradePrefabs;

	private List<UpgradePanel> upgradePanelList;
	private List<Upgrade> usedUpgrades;
	private CanvasGroup levelUpPanelCanvasGroup;
	private int level = 1;
	private int exp = 0;
	private float currentLevelExpSize = 10f;

    void Start()
    {
		upgradePanelList = new List<UpgradePanel>();
		usedUpgrades = new List<Upgrade>();
		levelUpPanelCanvasGroup = levelUpPanel.GetComponent<CanvasGroup>();
		SetLevelPanelActive(false);
		UpgradePanel[] ups = levelUpPanel.GetComponentsInChildren<UpgradePanel>();
		int numUps = ups.Length;
		for(int i = 0; i < numUps; i++)
		{
			upgradePanelList.Add(ups[i]);
		}
	}

	void SetLevelPanelActive(bool value)
	{
		if (value)
			RefreshUpgradePanel();
		levelUpPanelCanvasGroup.alpha = value ? 1f : 0f;
		levelUpPanelCanvasGroup.blocksRaycasts = value;
		levelUpPanelCanvasGroup.interactable = value;
	}

	void RefreshUpgradePanel()
	{
		foreach(UpgradePanel up in upgradePanelList)
		{
			Upgrade upgrade = GetUnusedUpgrade();
			if (upgrade != null)
				up.SetUpgrade(upgrade);
		}
	}

	void LevelUp()
	{
		level++;
		expText.text = "Lv " + level.ToString();
		levelUpText.text = level.ToString();
		exp = 0;
		UpdateExpBar();
		float nextLevelSize = currentLevelExpSize * levelSizeScaling;
		currentLevelExpSize = nextLevelSize;
		Time.timeScale = 0.001f;
		SetLevelPanelActive(true);
	}

	void UpdateExpBar()
	{
		float expPercentOfLevel = (exp / currentLevelExpSize);
		float barWidth = expPanel.sizeDelta.x * expPercentOfLevel;
		Vector2 expSize = new Vector2(barWidth, expBar.sizeDelta.y);
		expBar.sizeDelta = expSize;
	}

	Upgrade GetUnusedUpgrade()
	{
		Upgrade up = null;
		int index = 0;
		while ((up == null) && (index < 100))
		{
			index++;
			int indo = Random.Range(0, upgradePrefabs.Length);
			if ((upgradePrefabs.Length > indo) && (upgradePrefabs[indo] != null))
			{
				Upgrade thisUpg = upgradePrefabs[indo];
				if (!usedUpgrades.Contains(thisUpg))
				{
					up = thisUpg;
					//usedUpgrades.Add(up);
					break;
				}
			}
		}
		return up;
	}

	public void AddExp(int value)
	{
		exp += value;
		if (exp >= currentLevelExpSize)
			LevelUp();
		else
			UpdateExpBar();
	}

	public void SelectUpgrade(Upgrade up)
	{
		SetLevelPanelActive(false);
		Time.timeScale = 1f;
		up.EffectUpgrade();
	}
}
