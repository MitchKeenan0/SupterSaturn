using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrottleHUD : MonoBehaviour
{
	public GameObject throttleControlPanel;

	private Spacecraft spacecraft;
	private CanvasGroup throttleCanvasGroup;
	private InputController inputController;

    void Start()
    {
		throttleCanvasGroup = throttleControlPanel.GetComponent<CanvasGroup>();
		SetThrottleControlActive(false);
		inputController = FindObjectOfType<InputController>();
	}

	public void SetSpacecraft(Spacecraft sp)
	{
		spacecraft = sp;
	}

	public void SetThrottleControlActive(bool value)
	{
		throttleCanvasGroup.alpha = value ? 1f : 0f;
		throttleCanvasGroup.blocksRaycasts = value;
	}

	public void ThrottleLevel(float percent)
	{
		spacecraft.SetThrottle(percent);
		inputController.UpdateStatusText("Throttle " + percent * 100 + "%");
	}
}
