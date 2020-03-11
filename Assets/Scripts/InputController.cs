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
	public Color standbyColor;
	public Color activeColor;

	private MouseSelection mouseSelection;
	private CameraTargetFinder cameraTargetFinder;
	private TouchOrbit touchOrbit;
	private bool bNavigationMode = false;
	private bool bTargetCameraMode = false;
	private bool bFreeCameraMode = false;

	void Start()
	{
		mouseSelection = FindObjectOfType<MouseSelection>();
		cameraTargetFinder = FindObjectOfType<CameraTargetFinder>();
		touchOrbit = FindObjectOfType<TouchOrbit>();
		statusText.text = "";
		ActivateButton(navigationModeButton, false, true);
		ActivateButton(cameraModeButton, false, true);
		EnableCameraControl(true);
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

	public void NavigationMode(bool value)
	{
		if (bFreeCameraMode)
			EnableCameraControl(false);
		bool bTurningOff = bNavigationMode;
		if (value == false)
			bTurningOff = true;
		if (bTurningOff)
			value = false;
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
		if (bNavigationMode)
			NavigationMode(false);
		bool bTurningOff = bTargetCameraMode;
		if (value == false)
			bTurningOff = true;
		if (bTurningOff)
			value = false;
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
