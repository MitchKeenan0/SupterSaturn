using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
	public Image upgImage;
	public Text upgText;

	private ExpCanvas expCanvas;
	private Upgrade upgrade;

    void Start()
    {
		expCanvas = FindObjectOfType<ExpCanvas>();
    }

    public void SetUpgrade(Upgrade upg)
	{
		upgrade = upg;
		upgImage.sprite = upg.upgradeSprite;

		float value = upg.value;
		string sign = "+";
		if (value < 0)
			sign = "";
		string valueString = upg.value.ToString("F1");
		if (Mathf.Approximately(value, Mathf.RoundToInt(value)))
			valueString = upg.value.ToString("F0");


		upgText.text = sign + value + " " + upg.upgradeName;
	}

	public void Select()
	{
		expCanvas.SelectUpgrade(upgrade);
	}
}
