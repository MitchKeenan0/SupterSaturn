using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
	public Text statusText;
	public Button navigationModeButton;
	public Button cameraModeButton;
	public Button stopButton;
	public Color standbyColor;
	public Color activeColor;

	private MouseSelection mouseSelection;
	private CameraTargetFinder cameraTargetFinder;
	private TouchOrbit touchOrbit;
	private bool bNavigationMode = false;
	private bool bTargetCameraMode = false;
	private bool bFreeCameraMode = false;
	private bool bStopped = false;

	void Start()
	{
		mouseSelection = FindObjectOfType<MouseSelection>();
		cameraTargetFinder = FindObjectOfType<CameraTargetFinder>();
		touchOrbit = FindObjectOfType<TouchOrbit>();
		statusText.text = "";
		ActivateButton(navigationModeButton, false, true);
		ActivateButton(cameraModeButton, false, true);
		ActivateButton(stopButton, false, true);
	}

	void ActivateButton(Button button, bool value, bool finished)
	{
		if (finished)
			EventSystem.current.SetSelectedGameObject(null);

		Color color = standbyColor;
		if (value && !finished)
			color = activeColor;
		ColorBlock buttonColorBlock = button.colors;
		buttonColorBlock.normalColor = color;
		buttonColorBlock.highlightedColor = color;
		buttonColorBlock.pressedColor = Color.black;
		buttonColorBlock.selectedColor = color;
		button.colors = buttonColorBlock;

		if (!value)
			FallbackCheck();
	}

	void UpdateStatusText(string value)
	{
		statusText.text = value;
	}

	void FallbackCheck()
	{
		if (!bNavigationMode && !bTargetCameraMode && !bFreeCameraMode)
			EnableCameraControl(true);
	}

	public void Begin()
	{
		AllStop();
		TargetCameraMode(true);
	}

	public void AllStop()
	{
		bool bTurningOff = bStopped;
		bStopped = !bStopped;

		if (bStopped)
		{
			List<Spacecraft> selectedSpacecraft = mouseSelection.GetSelectedSpacecraft();
			foreach (Spacecraft sp in selectedSpacecraft)
			{
				Autopilot autopilot = sp.GetComponent<Autopilot>();
				autopilot.AllStop();
			}
		}

		if (bStopped)
			UpdateStatusText("All stop");
		else
			UpdateStatusText("Intertial stabilizers off");
		ActivateButton(stopButton, bStopped, bTurningOff);
	}

	public void NavigationMode(bool value)
	{
		bool bTurningOff = bNavigationMode;
		if (value == false)
			bTurningOff = true;
		if (bTurningOff)
			value = false;

		if (bStopped)
			AllStop();
		EnableCameraControl(bTurningOff);
		bNavigationMode = value;
		List<Spacecraft> selectedSpacecraft = mouseSelection.GetSelectedSpacecraft();
		foreach (Spacecraft sp in selectedSpacecraft)
			sp.GetComponentInChildren<SpacecraftController>().SetActive(bNavigationMode);

		if (bNavigationMode)
			UpdateStatusText("Navigation mode");
		else
			UpdateStatusText("");
		ActivateButton(navigationModeButton, bNavigationMode, bTurningOff);
	}

	public void TargetCameraMode(bool value)
	{
		bool bTurningOff = bTargetCameraMode;
		if (value == false)
			bTurningOff = true;
		if (bTurningOff)
			value = false;

		if (bNavigationMode)
			NavigationMode(false);
		EnableCameraControl(true);
		bTargetCameraMode = value;
		cameraTargetFinder.SetActive(bTargetCameraMode);

		if (bTargetCameraMode)
			UpdateStatusText("Target camera mode");
		else
			UpdateStatusText("");
		ActivateButton(cameraModeButton, bTargetCameraMode, bTurningOff);
	}

	public void EnableCameraControl(bool value)
	{
		bFreeCameraMode = value;
		touchOrbit.SetActive(bFreeCameraMode);
		UpdateStatusText("Free camera " + (value ? ("on") : ("off")));
	}

	public void SetCameraTarget(Transform target)
	{
		cameraTargetFinder.SetTarget(target);
	}
}
