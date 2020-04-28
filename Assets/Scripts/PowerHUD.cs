using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerHUD : MonoBehaviour
{
	public RectTransform powerPanel;
	public RectTransform powerBar;
	public Text powerText;

	private float powerPanelWidth = 0f;
	private float powerPanelHeight = 0f;

	public void SetPowerSource(Powerplant plant) { powerPlant = plant; }
	private Powerplant powerPlant = null;

    void Start()
    {
		powerPanelWidth = powerPanel.sizeDelta.x;
		powerPanelHeight = powerPanel.sizeDelta.y;
	}
    
    void Update()
    {
        if (powerPlant != null)
		{
			UpdatePowerBarValue();
		}
    }

	void UpdatePowerBarValue()
	{
		float power = powerPlant.GetPower();
		float currentPowerPercent = power / powerPlant.maxPower;
		float percentBarSize = powerPanelWidth * currentPowerPercent;
		Vector2 barSizeDelta = new Vector2(percentBarSize, powerPanelHeight);
		powerBar.sizeDelta = barSizeDelta;
		string decimalPlaces = "F1";
		if (currentPowerPercent >= 0.99f)
			decimalPlaces = "F0";
		powerText.text = power.ToString(decimalPlaces);
	}
}
