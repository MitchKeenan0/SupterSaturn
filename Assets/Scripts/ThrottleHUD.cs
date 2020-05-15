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
	private OrbitController orbitController;

    void Start()
    {
		throttleCanvasGroup = throttleControlPanel.GetComponent<CanvasGroup>();
		inputController = FindObjectOfType<InputController>();
		orbitController = FindObjectOfType<OrbitController>();
	}

	public void SetSpacecraft(Spacecraft sp)
	{
		spacecraft = sp;
	}

	public void ThrottleLevel(float percent)
	{
		spacecraft.SetThrottle(percent);
		inputController.UpdateStatusText("Throttle " + percent * 100 + "%");
		inputController.StopButton();
		if (percent == 0f)
			inputController.AllStop(true);
	}
}
